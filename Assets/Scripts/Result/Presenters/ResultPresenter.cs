using System;
using System.Threading;
using Automan.Root;
using Automan.Root.Model;
using Automan.Game.View;
using Automan.Result.View;
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
        private readonly LifeModel _lifeModel;
        private readonly AllClearButtonsView _allClearButtonsView;
        private readonly LifeView _lifeView;
        private readonly FrameView _frameView;

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="transitionManager">シーン遷移マネージャー</param>
        /// <param name="lifeModel">ライフのModel</param>
        /// <param name="allClearButtonsView">オールクリア時のボタンのView</param>
        /// <param name="lifeView">ライフのView</param>
        /// <param name="frameView">枠のView</param>
        [Inject]
        public ResultPresenter(TransitionManager transitionManager, LifeModel lifeModel, AllClearButtonsView allClearButtonsView, LifeView lifeView, FrameView frameView)
        {
            _transitionManager = transitionManager;
            _lifeModel = lifeModel;
            _allClearButtonsView = allClearButtonsView;
            _lifeView = lifeView;
            _frameView = frameView;
        }

        /// <summary>
        /// VContainerから呼ばれるStartメソッド
        /// </summary>
        public void Start()
        {
            _lifeView.SetLife(_lifeModel.Life.CurrentValue);

            _frameView.Shine();

            // リトライボタン処理
            _allClearButtonsView.OnRetryButtonClick
                .Subscribe(async _ =>
                {
                    _allClearButtonsView.Hide();
                    _lifeModel.Initialize();
                    await _transitionManager.TransitionTo(1);
                })
                .RegisterTo(_cancellationTokenSource.Token);

            // タイトルボタン処理
            _allClearButtonsView.OnTitleButtonClick
                .Subscribe(async _ =>
                {
                    _allClearButtonsView.Hide();
                    await _transitionManager.TransitionTo(TransitionManager.Scene.Title);
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