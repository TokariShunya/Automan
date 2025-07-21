using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;

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
        public async UniTask FadeOutAsync(Color color)
        {
            _fadePanel.color = color;

            gameObject.SetActive(true);

            await LMotion.Create(0f, 1f, _fadeDuration)
                .WithEase(Ease.InOutQuad)
                .BindToColorA(_fadePanel)
                .AddTo(gameObject)
                .ToUniTask(destroyCancellationToken);
        }

        /// <summary>
        /// フェードインする
        /// </summary>
        /// <param name="color">フェードの色</param>
        public async UniTask FadeInAsync(Color color)
        {
            _fadePanel.color = color;

            await LMotion.Create(1f, 0f, _fadeDuration)
                .WithEase(Ease.InOutQuad)
                .BindToColorA(_fadePanel)
                .AddTo(gameObject)
                .ToUniTask(destroyCancellationToken);

            gameObject.SetActive(false);
        }
    }
}