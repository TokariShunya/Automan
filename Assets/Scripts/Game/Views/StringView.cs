using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Automan.Game.View
{
    /// <summary>
    /// 文字列のView
    /// </summary>
    public sealed class StringView : MonoBehaviour
    {
        [SerializeField] private float _initialDelayStep;
        [SerializeField] private float _characterSpacing;

        private int _count;
        private CharacterView[] _characterViews;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="characterViews">文字のView</param>
        /// <param name="centerPosition">オートマトンの中心位置</param>
        public void Initialize(IEnumerable<CharacterView> characterViews, Vector3 centerPosition)
        {
            _characterViews = characterViews.Select(characterView => Instantiate(characterView, transform)).ToArray();

            var index = -1;
            
            for (int i = 0; i < _characterViews.Length; i++)
            {
                _characterViews[i].transform.localPosition = new Vector2(i * _characterSpacing, 0);

                if (!_characterViews[i].IsPositive.HasValue) index++;

                _characterViews[i].Initialize(index, _characterViews.Length - i - 1, centerPosition);
            }
        }

        /// <summary>
        /// 初期遷移する
        /// </summary>
        /// <param name="to">遷移先</param>
        public void InitialTransition(Vector3 to)
        {
            var delay = 0f;

            foreach (var characterView in _characterViews)
            {
                characterView.EnqueueTransition(to);
                characterView.InitialTransition(delay).Forget();
                delay += _initialDelayStep;
            }
        }

        /// <summary>
        /// 遷移する
        /// </summary>
        /// <param name="to">遷移先</param>
        public void Transition(Vector3 to)
        {
            foreach (var characterView in _characterViews)
            {
                characterView.EnqueueTransition(to);
            }
        }

        /// <summary>
        /// 現在の文字のViewが状態に達するまで待機する
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask WaitForCurrentCharacterToReachStateAsync(CancellationToken cancellationToken = default)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);

            await _characterViews[_count].OnStateReached.AttachExternalCancellation(source.Token);
        }

        /// <summary>
        /// 先頭の文字が取り除かれるまで待機する
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask WaitForOnPopFirstCharacterCompletedAsync(CancellationToken cancellationToken = default)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);

            await _characterViews[_count].OnCompleted.AttachExternalCancellation(source.Token);

            _characterViews[_count++].Hide().Forget();
        }
    }
}