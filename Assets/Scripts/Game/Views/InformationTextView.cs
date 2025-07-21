using System.Threading;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;

namespace Automan.Game.View
{
    /// <summary>
    /// テキスト情報のView
    /// </summary>
    public sealed class InformationTextView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _stageText;
        [SerializeField] private Transform _stageMask;
        [SerializeField] private TextMeshPro _clearText;
        [SerializeField] private Transform _clearMask;
        [SerializeField] private TextMeshPro _perfectClearText;
        [SerializeField] private Transform _perfectClearMask;
        [SerializeField] private TextMeshPro _gameoverText;
        [SerializeField] private Transform _gameoverMask;

        [SerializeField] private float _showDuration;

        private float _initialStageMaskHeight;
        private float _initialClearMaskHeight;
        private float _initialPerfectClearMaskHeight;
        private float _initialGameoverMaskHeight;

        private void Awake()
        {
            _initialStageMaskHeight = _stageMask.localScale.y;
            _initialClearMaskHeight = _clearMask.localScale.y;
            _initialPerfectClearMaskHeight = _perfectClearMask.localScale.y;
            _initialGameoverMaskHeight = _gameoverMask.localScale.y;
        }

        /// <summary>
        /// ステージ情報を表示する
        /// </summary>
        /// <param name="duration">表示時間</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask ShowStageInformationAsync(float duration, CancellationToken cancellationToken = default)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);

            _stageText.gameObject.SetActive(true);
            gameObject.SetActive(true);

            await LMotion.Create(0f, _initialStageMaskHeight, _showDuration)
                .WithEase(Ease.InOutQuad)
                .BindToLocalScaleY(_stageMask)
                .AddTo(gameObject)
                .ToUniTask(source.Token);

            await UniTask.Delay(System.TimeSpan.FromSeconds(duration), cancellationToken: source.Token);

            await LMotion.Create(_initialStageMaskHeight, 0f, _showDuration)
                .WithEase(Ease.InOutQuad)
                .BindToLocalScaleY(_stageMask)
                .AddTo(gameObject)
                .ToUniTask(source.Token);

            _stageText.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// クリアテキストを表示する
        /// </summary>
        /// <param name="isPerfect">パーフェクトかどうか</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask ShowClearInformationAsync(bool isPerfect, CancellationToken cancellationToken = default)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);

            if (isPerfect)
            {
                _perfectClearText.gameObject.SetActive(true);
                gameObject.SetActive(true);

                await LMotion.Create(0f, _initialPerfectClearMaskHeight, _showDuration)
                .WithEase(Ease.InOutQuad)
                .BindToLocalScaleY(_perfectClearMask)
                .AddTo(gameObject)
                .ToUniTask(source.Token);
            }
            else
            {
                _clearText.gameObject.SetActive(true);
                gameObject.SetActive(true);

                await LMotion.Create(0f, _initialClearMaskHeight, _showDuration)
                .WithEase(Ease.InOutQuad)
                .BindToLocalScaleY(_clearMask)
                .AddTo(gameObject)
                .ToUniTask(source.Token);
            }
        }

        /// <summary>
        /// ゲームオーバーテキストを表示する
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask ShowGameoverInformationAsync(CancellationToken cancellationToken = default)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);

            _gameoverText.gameObject.SetActive(true);
            gameObject.SetActive(true);

            await LMotion.Create(0f, _initialGameoverMaskHeight, _showDuration)
                .WithEase(Ease.InOutQuad)
                .BindToLocalScaleY(_gameoverMask)
                .AddTo(gameObject)
                .ToUniTask(source.Token);
        }
    }
}