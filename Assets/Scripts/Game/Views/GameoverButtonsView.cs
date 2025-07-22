using System.Threading;
using Automan.Root.View;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;
using LitMotion;
using LitMotion.Extensions;

namespace Automan.Game.View
{
    /// <summary>
    /// ゲームオーバー時のボタンのView
    /// </summary>
    public sealed class GameoverButtonsView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private HoverButton _retryButton;
        [SerializeField] private HoverButton _titleButton;
        [SerializeField] private float _duration;

        /// <summary>
        /// リトライボタンクリックイベント
        /// </summary>
        public Observable<Unit> OnRetryButtonClick => _retryButton.OnClick;

        /// <summary>
        /// タイトルボタンクリックイベント
        /// </summary>
        public Observable<Unit> OnTitleButtonClick => _titleButton.OnClick;

        /// <summary>
        /// ボタンを表示する
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);

            _canvasGroup.alpha = 0f;
            gameObject.SetActive(true);

            await LMotion.Create(0f, 1f, _duration)
                .WithEase(Ease.InOutQuad)
                .BindToAlpha(_canvasGroup)
                .AddTo(gameObject)
                .ToUniTask(source.Token);
        }

        /// <summary>
        /// ボタンを非表示にする
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}