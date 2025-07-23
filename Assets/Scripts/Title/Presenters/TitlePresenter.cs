using System;
using System.Threading;
using Automan.Root;
using Automan.Root.Model;
using Automan.Root.View;
using UnityEngine;
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
        private readonly SoundManager _soundManager;
        private readonly LifeModel _lifeModel;
        private readonly TimeModel _timeModel;
        private readonly HoverButton _playButton;

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="transitionManager">シーン遷移マネージャー</param>
        /// <param name="soundManager">サウンドマネージャー</param>
        /// <param name="lifeModel">ライフのModel</param>
        /// <param name="timeModel">時間のModel</param>
        /// <param name="playButton">プレイボタン</param>
        [Inject]
        public TitlePresenter(TransitionManager transitionManager, SoundManager soundManager, LifeModel lifeModel, TimeModel timeModel, HoverButton playButton)
        {
            _transitionManager = transitionManager;
            _soundManager = soundManager;
            _lifeModel = lifeModel;
            _timeModel = timeModel;
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

            _soundManager.Play(SoundManager.Sound.Button);
            _playButton.gameObject.SetActive(false);

            _lifeModel.Initialize();
            _timeModel.Initialize();
            await _transitionManager.TransitionToAsync(1, Color.black);
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