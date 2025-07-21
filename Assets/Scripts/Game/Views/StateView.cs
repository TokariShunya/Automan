using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using LitMotion;

namespace Automan.Game.View
{
    /// <summary>
    /// 状態のView
    /// </summary>
    public sealed class StateView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField] private Sprite _positiveStateSprite;
        [SerializeField] private Sprite _negativeStateSprite;

        [SerializeField] private float _highlightFadeDuration;
        [SerializeField] private float _highlightDuration;
        [SerializeField] private float _correctHighlightMultiplication;
        [SerializeField] private float _errorHighlightMultiplication;

        [SerializeField] private int _id;

        /// <summary>
        /// ID
        /// </summary>
        public int Id => _id;

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position => transform.position;

        /// <summary>
        /// ローカルの位置
        /// </summary>
        public Vector3 LocalPosition => transform.localPosition;

        [SerializeField] private SerializableReactiveProperty<bool> _isPositive;

        /// <summary>
        /// 受理状態かどうか
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsPositive => _isPositive;

        private IDisposable _onPointerClickDisposable;

        private readonly int _multiplicationId = Shader.PropertyToID("_Multiplication");

        private void Start()
        {
            _onPointerClickDisposable = gameObject.AddComponent<ObservablePointerClickTrigger>().OnPointerClickAsObservable()
                .Subscribe(_ =>
                {
                    _isPositive.Value = !_isPositive.CurrentValue;
                });

            _onPointerClickDisposable.RegisterTo(destroyCancellationToken);

            _isPositive
                .Subscribe(isPositive =>
                {
                    _spriteRenderer.sprite = isPositive ? _positiveStateSprite : _negativeStateSprite;
                })
                .RegisterTo(destroyCancellationToken);
        }

        /// <summary>
        /// 固定する
        /// </summary>
        public void Fix()
        {
            _onPointerClickDisposable?.Dispose();
        }

        /// <summary>
        /// ハイライトする
        /// </summary>
        /// <param name="isCorrect">判定</param>
        public async UniTaskVoid Highlight(bool isCorrect)
        {
            await LMotion.Create(0.5f, isCorrect ? _correctHighlightMultiplication : _errorHighlightMultiplication, _highlightFadeDuration)
                .WithEase(Ease.InOutQuad)
                .Bind(value => _spriteRenderer.material.SetFloat(_multiplicationId, value))
                .AddTo(gameObject)
                .ToUniTask(destroyCancellationToken);

            await UniTask.Delay(TimeSpan.FromSeconds(_highlightDuration), cancellationToken: destroyCancellationToken);

            await LMotion.Create(_spriteRenderer.material.GetFloat(_multiplicationId), 0.5f, _highlightFadeDuration)
                .WithEase(Ease.InOutQuad)
                .Bind(value => _spriteRenderer.material.SetFloat(_multiplicationId, value))
                .AddTo(gameObject)
                .ToUniTask(destroyCancellationToken);
        }

        private void OnDestroy()
        {
            _isPositive.Dispose();
        }
    }
}