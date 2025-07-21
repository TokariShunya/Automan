using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using R3;
using LitMotion;
using LitMotion.Extensions;

namespace Automan.Root.View
{
    /// <summary>
    /// ホバーボタン
    /// </summary>
    public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image _fill;
        [SerializeField] private TMP_Text _text;

        [SerializeField] private Color _hoverTextColor;
        [SerializeField] private float _duration;

        private Color _initialTextColor;

        private readonly CompositeMotionHandle _motionHandles = new();

        private readonly Subject<Unit> _onClick = new();

        /// <summary>
        /// クリックイベント
        /// </summary>
        public Observable<Unit> OnClick => _onClick.AsObservable();

        private void Awake()
        {
            _initialTextColor = _text.color;
        }

        private void OnDestroy()
        {
            _motionHandles.Cancel();
            _onClick.Dispose();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick.OnNext(Unit.Default);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _motionHandles.Cancel();

            LMotion.Create(_fill.fillAmount, 1f, _duration)
                .WithEase(Ease.InOutQuad)
                .BindToFillAmount(_fill)
                .AddTo(_motionHandles);

            LMotion.Create(_text.color, _hoverTextColor, _duration)
                .WithEase(Ease.InOutQuad)
                .BindToColor(_text)
                .AddTo(_motionHandles);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _motionHandles.Cancel();

            LMotion.Create(_fill.fillAmount, 0f, _duration)
                .WithEase(Ease.InOutQuad)
                .BindToFillAmount(_fill)
                .AddTo(_motionHandles);

            LMotion.Create(_text.color, _initialTextColor, _duration)
                .WithEase(Ease.InOutQuad)
                .BindToColor(_text)
                .AddTo(_motionHandles);
        }
    }
}