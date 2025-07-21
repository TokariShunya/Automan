using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Automan.Root;
using Automan.Root.Model;
using Automan.Game.Model;
using Automan.Game.Service;
using Automan.Game.View;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using R3;
using VContainer;
using VContainer.Unity;

#nullable enable

namespace Automan.Game.Presenter
{
    /// <summary>
    /// パズルモードのPresenter
    /// </summary>
    public sealed class PuzzleModePresenter : IStartable, IDisposable
    {
        private readonly TransitionManager _transitionManager;
        private readonly LifeModel _lifeModel;
        private readonly StageModel _stageModel;
        private readonly StringGeneratorService _stringGeneratorService;
        private readonly StringViewFactory _stringViewFactory;
        private readonly AutomatonView _automatonView;
        private readonly LifeView _lifeView;
        private readonly FrameView _frameView;
        private readonly InformationTextView _informationTextView;
        private readonly Button _startButton;
        private readonly GameoverButtonsView _gameoverButtonsView;

        private readonly CancellationTokenSource _cancellationTokenSource = new ();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="transitionManager">シーン遷移マネージャー</param>
        /// <param name="lifeModel">ライフのModel</param>
        /// <param name="stageModel">ステージのModel</param>
        /// <param name="stringGeneratorService">文字列Model生成器</param>
        /// <param name="stringViewFactory">文字列View生成器</param>
        /// <param name="automatonView">オートマトンのView</param>
        /// <param name="lifeView">ライフのView</param>
        /// <param name="frameView">枠のView</param>
        /// <param name="informationTextView">テキスト情報のView</param>
        /// <param name="startButton">スタートボタン</param>
        /// <param name="gameoverButtonsView">ゲームオーバー時のボタンのView</param>
        [Inject]
        public PuzzleModePresenter(TransitionManager transitionManager, LifeModel lifeModel, StageModel stageModel, StringGeneratorService stringGeneratorService, StringViewFactory stringViewFactory, AutomatonView automatonView, LifeView lifeView, FrameView frameView, InformationTextView informationTextView, Button startButton, GameoverButtonsView gameoverButtonsView)
        {
            _transitionManager = transitionManager;
            _lifeModel = lifeModel;
            _stageModel = stageModel;
            _stringGeneratorService = stringGeneratorService;
            _stringViewFactory = stringViewFactory;
            _automatonView = automatonView;
            _lifeView = lifeView;
            _frameView = frameView;
            _informationTextView = informationTextView;
            _startButton = startButton;
            _gameoverButtonsView = gameoverButtonsView;
        }

        /// <summary>
        /// VContainerから呼ばれるStartメソッド
        /// </summary>
        public void Start()
        {
            IDisposable? disposable = default;

            // ライフの変更を監視し，Viewに反映
            disposable = _lifeModel.Life
                .Subscribe(life =>
                {
                    _lifeView.SetLife(life);
                    if (life == 0)
                    {
                        disposable?.Dispose();
                    }
                });

            disposable.RegisterTo(_cancellationTokenSource.Token);

            // エントリーポイントをFire and Forgetで実行
            EntryPoint().Forget();
        }

        private async UniTaskVoid EntryPoint()
        {
            // オートマトンを生成
            var stateCount = _automatonView.StateCount;

            AutomatonModel automaton = new (stateCount, 0);

            // Viewの状態変更イベントを購読し，Modelに反映
            _automatonView.OnStateChanged
                .Subscribe(state =>
                {
                    automaton.ChangeState(state.State, state.IsPositive);
                })
                .RegisterTo(_cancellationTokenSource.Token);

            // Viewの遷移変更イベントを購読し，Modelに反映
            _automatonView.OnTransitionChanged
                .Subscribe(transition =>
                {
                    automaton.ChangeTransition(transition.State, transition.Character, transition.Destination);
                })
                .RegisterTo(_cancellationTokenSource.Token);

            // 文字列生成の設定を取得
            var stringConfiguration = _stageModel.StageStringConfiguration;

            // 文字列のモデルをランダムに生成
            AutomatonString[] currentStrings = _stringGeneratorService.Generate(stringConfiguration.StringCount, stateCount, stringConfiguration.StringLengthRange, stringConfiguration.CheckedCharacterCountRange);

            // 文字列のViewを作成
            StringView[] stringViews = currentStrings.Select(currentString => _stringViewFactory.Create(currentString, _automatonView.CenterPosition)).ToArray();

            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: _cancellationTokenSource.Token);

            // ステージ番号を表示
            await _informationTextView.ShowStageInformationAsync(1f, _cancellationTokenSource.Token);

            _stringViewFactory.Show();

            // スタートボタンが押されるまで待機
            await _startButton.OnClickAsync(_cancellationTokenSource.Token);
            _startButton.gameObject.SetActive(false);

            // 状態を固定
            _automatonView.FixStates();

            // 枠を明るくする
            await _frameView.IlluminateAsync(_cancellationTokenSource.Token);
            _frameView.Flicker();

            // 順番にオートマトンを実行
            for (int i = 0; i < currentStrings.Length; i++)
            {
                await RunAutomatonAsync(automaton, currentStrings[i], _automatonView.StatePositions, stringViews[i], _cancellationTokenSource.Token);
            }

            // ライフが残っているか
            if (_lifeModel.IsSurvived)
            {
                // クリア演出
                _frameView.Shine();
                await _informationTextView.ShowClearInformationAsync(_stageModel.IsPerfect, _cancellationTokenSource.Token);

                await UniTask.Delay(System.TimeSpan.FromSeconds(2f), cancellationToken: _cancellationTokenSource.Token);

                if (_stageModel.CurrentStageNumber >= _stageModel.StageCount)
                {
                    // オールクリア
                    await _transitionManager.TransitionTo(TransitionManager.Scene.Result, Color.white);
                }
                else
                {
                    // 次のステージへ遷移
                    await _transitionManager.TransitionTo(_stageModel.CurrentStageNumber + 1, Color.white);
                } 
            }
            else
            {
                // ゲームオーバー演出
                _frameView.Blink();
                await _informationTextView.ShowGameoverInformationAsync(_cancellationTokenSource.Token);

                await _gameoverButtonsView.Show(_cancellationTokenSource.Token);

                // リトライボタン処理
                _gameoverButtonsView.OnRetryButtonClick
                    .Subscribe(async _ =>
                    {
                        _gameoverButtonsView.Hide();
                        _lifeModel.Initialize();
                        await _transitionManager.TransitionTo(1);
                    })
                    .RegisterTo(_cancellationTokenSource.Token);

                // タイトルボタン処理
                _gameoverButtonsView.OnTitleButtonClick
                    .Subscribe(async _ =>
                    {
                        _gameoverButtonsView.Hide();
                        await _transitionManager.TransitionTo(TransitionManager.Scene.Title);
                    })
                    .RegisterTo(_cancellationTokenSource.Token);
            } 
        }

        private async UniTask RunAutomatonAsync(AutomatonModel automaton, AutomatonString currentString, IReadOnlyDictionary<int, Vector3> statePositions, StringView stringView, CancellationToken cancellationToken = default)
        {
            // オートマトンのModelを初期状態にリセット
            automaton.ResetState();

            // 文字列のViewを初期状態に遷移
            stringView.InitialTransition(statePositions[automaton.CurrentState]);

            while (!currentString.IsEmpty)
            {
                // 現在の文字のViewが状態に達するまで待機
                await stringView.WaitForCurrentCharacterToReachStateAsync(cancellationToken);

                // オートマトンのModelの状態を更新
                var popped = automaton.Transition(currentString);

                if (!popped.HasValue)
                {
                    // 文字列のViewを遷移
                    stringView.Transition(statePositions[automaton.CurrentState]);
                }

                // 先頭の文字が取り除かれるまで待機する
                await stringView.WaitForOnPopFirstCharacterCompletedAsync(cancellationToken);

                if (popped.HasValue)
                {
                    // 判定
                    var isCorrect = popped == automaton.IsInPositiveState;

                    _automatonView.HighlightState(automaton.CurrentState, isCorrect);

                    if (!isCorrect)
                    {
                        _lifeModel.Damage();
                        _stageModel.ErrorCount++;
                    }
                }
            }
        }

        /// <summary>
        /// Presenterの破棄
        /// </summary>
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}