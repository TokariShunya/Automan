using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using LitMotion;

namespace Automan.Game.View
{
    /// <summary>
    /// 枠のView
    /// </summary>
    public sealed class FrameView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Material _material;
        [SerializeField] private Material _rainbowMaterial;
        [SerializeField] private Material _redMaterial;
        [SerializeField] private float _illuminateDuration;
        [SerializeField] private float _flickerMinMultiplication;
        [SerializeField] private float _flickerDuration;
        [SerializeField] private float _rainbowRotateDuration;
        [SerializeField] private float _blinkMinMultiplication;
        [SerializeField] private float _blinkDuration;

        private readonly CompositeMotionHandle _motionHandles = new ();

        private readonly int _multiplicationId = Shader.PropertyToID("_Multiplication");
        private readonly int _rainbowOffsetId = Shader.PropertyToID("_RainbowOffset");

        /// <summary>
        /// 枠を明るくする
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask IlluminateAsync(CancellationToken cancellationToken = default)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);

            _motionHandles.Cancel();

            if (_spriteRenderer.material != _material)
            {
                _spriteRenderer.material = _material;
            }

            await LMotion.Create(_spriteRenderer.material.GetFloat(_multiplicationId), 1f, _illuminateDuration)
                .WithEase(Ease.InOutQuad)
                .Bind(value => _spriteRenderer.material.SetFloat(_multiplicationId, value))
                .AddTo(_motionHandles)
                .ToUniTask(source.Token);
        }

        /// <summary>
        /// 枠を明滅させる
        /// </summary>
        public void Flicker()
        {
            _motionHandles.Cancel();

            if (_spriteRenderer.material != _material)
            {
                _spriteRenderer.material = _material;
            }

            LMotion.Create(1f, _flickerMinMultiplication, _flickerDuration)
                .WithEase(Ease.InOutQuad)
                .WithLoops(-1, LoopType.Yoyo)
                .Bind(value => _spriteRenderer.material.SetFloat(_multiplicationId, value))
                .AddTo(_motionHandles);
        }

        /// <summary>
        /// 枠を虹色に輝かせる
        /// </summary>
        public void Shine()
        {
            _motionHandles.Cancel();

            if (_spriteRenderer.material != _rainbowMaterial)
            {
                _spriteRenderer.material = _rainbowMaterial;
            }

            LMotion.Create(0f, 1f, _rainbowRotateDuration)
                .WithEase(Ease.Linear)
                .WithLoops(-1, LoopType.Restart)
                .Bind(value => _spriteRenderer.material.SetFloat(_rainbowOffsetId, value))
                .AddTo(_motionHandles);
        }

        /// <summary>
        /// 枠を赤色に点滅させる
        /// </summary>
        public void Blink()
        {
            _motionHandles.Cancel();

            if (_spriteRenderer.material != _redMaterial)
            {
                _spriteRenderer.material = _redMaterial;
            }

            LMotion.Create(1f, _blinkMinMultiplication, _blinkDuration)
                .WithEase(Ease.InOutQuad)
                .WithLoops(-1, LoopType.Yoyo)
                .Bind(value => _spriteRenderer.material.SetFloat(_multiplicationId, value))
                .AddTo(_motionHandles);
        }

        private void OnDestroy()
        {
            _motionHandles.Cancel();
        }
    }
}