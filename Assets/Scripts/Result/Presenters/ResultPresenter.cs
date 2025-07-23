using System;
using System.Threading;
using Automan.Root;
using Automan.Root.Model;
using Automan.Game.View;
using Automan.Result.View;
using UnityEngine;
using R3;
using VContainer;
using VContainer.Unity;

#nullable enable

namespace Automan.Result.Presenter
{
    /// <summary>
    /// リザルトのPresenter
    /// </summary>
    public sealed class ResultPresenter : IStartable, IDisposable
    {
        private readonly TransitionManager _transitionManager;
        private readonly SoundManager _soundManager;
        private readonly LifeModel _lifeModel;
        private readonly TimeModel _timeModel;
        private readonly AllClearButtonsView _allClearButtonsView;
        private readonly ResultRecordView _resultRecordView;
        private readonly LifeView _lifeView;
        private readonly FrameView _frameView;

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="transitionManager">シーン遷移マネージャー</param>
        /// <param name="soundManager">サウンドマネージャー</param>
        /// <param name="lifeModel">ライフのModel</param>
        /// <param name="timeModel">時間のModel</param>
        /// <param name="allClearButtonsView">オールクリア時のボタンのView</param>
        /// <param name="resultRecordView">オールクリア時のボタンのView</param>
        /// <param name="lifeView">ライフのView</param>
        /// <param name="frameView">枠のView</param>
        [Inject]
        public ResultPresenter(TransitionManager transitionManager, SoundManager soundManager, LifeModel lifeModel, TimeModel timeModel, AllClearButtonsView allClearButtonsView, ResultRecordView resultRecordView, LifeView lifeView, FrameView frameView)
        {
            _transitionManager = transitionManager;
            _soundManager = soundManager;
            _lifeModel = lifeModel;
            _timeModel = timeModel;
            _allClearButtonsView = allClearButtonsView;
            _resultRecordView = resultRecordView;
            _lifeView = lifeView;
            _frameView = frameView;
        }

        /// <summary>
        /// VContainerから呼ばれるStartメソッド
        /// </summary>
        public void Start()
        {
            _lifeView.SetLife(_lifeModel.Life.CurrentValue);

            var clearTime = _timeModel.Time.CurrentValue;
            var errorCount = _lifeModel.InitialLife - _lifeModel.Life.CurrentValue;
            var penalty = _timeModel.Penalty;
            var recordTime = clearTime + penalty * errorCount;

            _resultRecordView.SetRecords(clearTime, errorCount, penalty, recordTime);

            _soundManager.Play(SoundManager.Sound.AllClearJingle);
            _frameView.Shine();

            // リトライボタン処理
            _allClearButtonsView.OnRetryButtonClick
                .Subscribe(async _ =>
                {
                    _soundManager.Play(SoundManager.Sound.Button);
                    _allClearButtonsView.Hide();
                    _lifeModel.Initialize();
                    _timeModel.Initialize();
                    await _transitionManager.TransitionToAsync(1, Color.black);
                })
                .RegisterTo(_cancellationTokenSource.Token);

            // タイトルボタン処理
            _allClearButtonsView.OnTitleButtonClick
                .Subscribe(async _ =>
                {
                    _soundManager.Play(SoundManager.Sound.Button);
                    _allClearButtonsView.Hide();
                    await _transitionManager.TransitionToAsync(TransitionManager.Scene.Title, Color.black);
                })
                .RegisterTo(_cancellationTokenSource.Token);
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