using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;

namespace Automan.Game.View
{
    /// <summary>
    /// 文字のView
    /// </summary>
    public sealed class CharacterView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private float _transitionDuration;
        [SerializeField] private float _fadeDuration;
        [SerializeField] private float _stateDelay;
        [SerializeField] private float _selfTransitionLength;

        [SerializeField] private bool _isChecked;
        [SerializeField] private bool _isPositive;

        private int _index;
        private Vector3 _centerPosition;

        private Queue<Vector3> _transitionQueue;
        private bool _isAlive;

        private readonly UniTaskCompletionSource _onStart = new ();

        private readonly UniTaskCompletionSource _onStateReached = new ();

        /// <summary>
        /// 状態到達イベント
        /// </summary>
        public UniTask OnStateReached => _onStateReached.Task;

        private readonly UniTaskCompletionSource _onCompleted = new ();

        /// <summary>
        /// 完了イベント
        /// </summary>
        public UniTask OnCompleted => _onCompleted.Task;

        /// <summary>
        /// 文字の正負
        /// </summary>
        public bool? IsPositive => _isChecked ? _isPositive : null;

        private void Awake()
        {
            _transitionQueue = new Queue<Vector3>();
            _isAlive = true;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <param name="order">描画順</param>
        /// <param name="centerPosition">オートマトンの中心の位置</param>
        public void Initialize(int index, int order, Vector3 centerPosition)
        {
            _index = index;
            _centerPosition = centerPosition;
            _spriteRenderer.sortingOrder = order * 2;

            if (_text != null)
            {
                _text.sortingOrder = order * 2 + 1;
            }
        }

        /// <summary>
        /// 初期遷移する
        /// </summary>
        /// <param name="initialDelay">遷移開始時の遅延時間</param>
        public async UniTaskVoid InitialTransition(float initialDelay)
        {
            if (!_isAlive) return;

            await UniTask.Delay(System.TimeSpan.FromSeconds(initialDelay), cancellationToken: destroyCancellationToken);

            _onStart.TrySetResult();
        }

        /// <summary>
        /// 遷移をキューに追加する
        /// </summary>
        /// <param name="to">遷移先</param>
        public void EnqueueTransition(Vector3 to)
        {
            if (!_isAlive) return;

            _transitionQueue.Enqueue(to);
        }

        /// <summary>
        /// 文字を非表示にする
        /// </summary>
        /// <returns></returns>
        public async UniTaskVoid Hide()
        {
            _isAlive = false;

            await LMotion.Create(1f, 0f, _fadeDuration)
                .WithEase(Ease.InOutQuad)
                .BindToColorA(_spriteRenderer)
                .AddTo(gameObject)
                .ToUniTask(destroyCancellationToken);

            gameObject.SetActive(false);
        }

        private void Start()
        {
            // Fire and Forgetで実行
            UniTask.Void(async (token) =>
            {

                await _onStart.Task;

                var count = 0;

                while (true)
                {
                    await UniTask.WaitUntil(() => _transitionQueue.Count > 0, cancellationToken: token);

                    var from = transform.position;
                    var to = _transitionQueue.Dequeue();

                    if (from == to)
                    {
                        await TransitionSelf(from, from + Vector3.Normalize(from - _centerPosition) * _selfTransitionLength, token);
                    }
                    else
                    {
                        await Transition(from, to, token);
                    }

                    if (count > _index)
                    {
                        _onStateReached.TrySetResult();
                        _onCompleted.TrySetResult();
                        return;
                    }
                    else
                    {
                        await UniTask.Delay(System.TimeSpan.FromSeconds(_stateDelay), cancellationToken: token);

                        if (count == _index)
                        {
                            _onStateReached.TrySetResult();
                        }
                    }

                    count++;
                }

            }, destroyCancellationToken);
        }

        private async UniTask Transition(Vector3 from, Vector3 to, CancellationToken cancellationToken = default)
        {
            var transition = LMotion.Create(from, to, _transitionDuration)
                        .WithEase(Ease.InOutQuad)
                        .BindToPosition(transform)
                        .AddTo(gameObject)
                        .ToUniTask(cancellationToken);

            var delta = to - from;

            var rotationZTo = 0f;

            if (delta.x != 0)
            {
                rotationZTo = -Mathf.Sign(delta.x) * 360;
            }
            else
            {
                if (delta.y != 0)
                {
                    rotationZTo = Mathf.Sign(delta.y) * 360;
                }
            }

            var rotation = LMotion.Create(0f, rotationZTo, _transitionDuration)
                .WithEase(Ease.InOutQuad)
                .BindToLocalEulerAnglesZ(transform)
                .AddTo(gameObject)
                .ToUniTask(cancellationToken);

            await UniTask.WhenAll(transition, rotation);
        }

        private async UniTask TransitionSelf(Vector3 from, Vector3 to, CancellationToken cancellationToken = default)
        {
            var transition = LMotion.Create(from, to, _transitionDuration / 2)
                        .WithEase(Ease.InQuad)
                        .BindToPosition(transform)
                        .AddTo(gameObject)
                        .ToUniTask(cancellationToken);

            var delta = to - from;

            var rotationZTo = 0f;

            if (delta.x != 0)
            {
                rotationZTo = -Mathf.Sign(delta.x) * 180;
            }
            else
            {
                if (delta.y != 0)
                {
                    rotationZTo = Mathf.Sign(delta.y) * 180;
                }
            }

            var rotation = LMotion.Create(0f, rotationZTo, _transitionDuration / 2)
                .WithEase(Ease.InQuad)
                .BindToLocalEulerAnglesZ(transform)
                .AddTo(gameObject)
                .ToUniTask(cancellationToken);

            await UniTask.WhenAll(transition, rotation);

            transition = LMotion.Create(to, from, _transitionDuration / 2)
                        .WithEase(Ease.OutQuad)
                        .BindToPosition(transform)
                        .AddTo(gameObject)
                        .ToUniTask(cancellationToken);

            rotation = LMotion.Create(rotationZTo, 0f, _transitionDuration / 2)
                .WithEase(Ease.OutQuad)
                .BindToLocalEulerAnglesZ(transform)
                .AddTo(gameObject)
                .ToUniTask(cancellationToken);

            await UniTask.WhenAll(transition, rotation);
        }

        private void OnDestroy()
        {
            _onStart.TrySetCanceled();
            _onStateReached.TrySetCanceled();
            _onCompleted.TrySetCanceled();
        }
    }
}