using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Automan.Root.View
{
    /// <summary>
    /// フェードのView
    /// </summary>
    public sealed class TransitionFadeView : MonoBehaviour
    {
        [SerializeField] private Image _fadePanel;
        [SerializeField] private float _fadeDuration;

        /// <summary>
        /// フェードアウトする
        /// </summary>
        /// <param name="color">フェードの色</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask FadeOutAsync(Color color, CancellationToken cancellationToken = default)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);

            _fadePanel.color = color;

            gameObject.SetActive(true);

            await LMotion.Create(0f, 1f, _fadeDuration)
                .WithEase(Ease.InOutQuad)
                .BindToColorA(_fadePanel)
                .AddTo(gameObject)
                .ToUniTask(source.Token);
        }

        /// <summary>
        /// フェードインする
        /// </summary>
        /// <param name="color">フェードの色</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask FadeInAsync(Color color, CancellationToken cancellationToken = default)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);

            _fadePanel.color = color;

            await LMotion.Create(1f, 0f, _fadeDuration)
                .WithEase(Ease.InOutQuad)
                .BindToColorA(_fadePanel)
                .AddTo(gameObject)
                .ToUniTask(source.Token);

            gameObject.SetActive(false);
        }
    }
}