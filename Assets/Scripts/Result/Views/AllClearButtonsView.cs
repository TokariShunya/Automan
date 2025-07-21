using Automan.Root.View;
using UnityEngine;
using R3;

namespace Automan.Result.View
{
    /// <summary>
    /// オールクリア時のボタンのView
    /// </summary>
    public sealed class AllClearButtonsView : MonoBehaviour
    {
        [SerializeField] private HoverButton _retryButton;
        [SerializeField] private HoverButton _titleButton;

        /// <summary>
        /// リトライボタンクリックイベント
        /// </summary>
        public Observable<Unit> OnRetryButtonClick => _retryButton.OnClick;

        /// <summary>
        /// タイトルボタンクリックイベント
        /// </summary>
        public Observable<Unit> OnTitleButtonClick => _titleButton.OnClick;

        /// <summary>
        /// ボタンを非表示にする
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}