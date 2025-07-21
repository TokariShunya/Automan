using System;
using System.Threading;
using Automan.Root;
using Automan.Root.Model;
using Automan.Root.View;
using Cysharp.Threading.Tasks;
using R3;
using VContainer;
using VContainer.Unity;

#nullable enable

namespace Automan.Title.Presenter
{
    /// <summary>
    /// タイトルのPresenter
    /// </summary>
    public sealed class TitlePresenter : IStartable, IDisposable
    {
        private readonly TransitionManager _transitionManager;
        private readonly LifeModel _lifeModel;
        private readonly HoverButton _playButton;

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="transitionManager">シーン遷移マネージャー</param>
        /// <param name="lifeModel">ライフのModel</param>
        /// <param name="playButton">プレイボタン</param>
        [Inject]
        public TitlePresenter(TransitionManager transitionManager, LifeModel lifeModel, HoverButton playButton)
        {
            _transitionManager = transitionManager;
            _lifeModel = lifeModel;
            _playButton = playButton;
        }

        /// <summary>
        /// VContainerから呼ばれるStartメソッド
        /// </summary>
        public void Start()
        {
            EntryPoint().Forget();
        }

        private async UniTaskVoid EntryPoint()
        {
            try
            {
                await _playButton.OnClick.FirstAsync(_cancellationTokenSource.Token).AsUniTask();
            }
            catch (InvalidOperationException)
            {
                return;
            }
            
            _playButton.gameObject.SetActive(false);

            _lifeModel.Initialize();
            await _transitionManager.TransitionTo(1);
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