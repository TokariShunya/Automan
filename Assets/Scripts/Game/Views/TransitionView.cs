using UnityEngine;
using R3;

namespace Automan.Game.View
{
    /// <summary>
    /// 遷移のView
    /// </summary>
    public sealed class TransitionView : MonoBehaviour
    {
        [SerializeReference, SubclassSelector] private AutomatonCharacter _character;
        [SerializeField] private SpriteRenderer _transition;
        [SerializeField] private SpriteRenderer _selfTransition;
        [SerializeField] private SpriteRenderer _ghost;
        [SerializeField] private SpriteRenderer _selfGhost;
        [SerializeField] private BoxCollider2D _collider;
        [SerializeField] private BoxCollider2D _selfCollider;

        [SerializeField] private float _length;
        [SerializeField] private StateView _stateView;

        /// <summary>
        /// 状態のView
        /// </summary>
        public StateView StateView => _stateView;

        [SerializeField] private SerializableReactiveProperty<StateView> _destinationStateView;

        /// <summary>
        /// 遷移先の状態のView
        /// </summary>
        public ReactiveProperty<StateView> DestinationStateView => _destinationStateView;

        private readonly ReactiveProperty<StateView> _ghostDestinationStateView = new ();

        /// <summary>
        /// ゴーストの遷移先の状態のView
        /// </summary>
        public ReactiveProperty<StateView> GhostDestinationStateView => _ghostDestinationStateView;

        /// <summary>
        /// 遷移のキー
        /// </summary>
        public (int State, AutomatonCharacter Character) Key => (_stateView.Id, _character);

        private void Start()
        {
            var transitionSize = _transition.size;
            transitionSize.y = _length;
            _transition.size = transitionSize;

            var selfTransitionSize = _selfTransition.size;
            selfTransitionSize.y = _length;
            _selfTransition.size = selfTransitionSize;

            var ghostSize = _ghost.size;
            ghostSize.y = _length;
            _ghost.size = ghostSize;

            var selfGhostSize = _selfGhost.size;
            selfGhostSize.y = _length;
            _selfGhost.size = selfGhostSize;

            var colliderSize = _collider.size;
            colliderSize.y = _length - 1;
            _collider.size = colliderSize;

            var selfColliderSize = _selfCollider.size;
            selfColliderSize.y = _length - 1;
            _selfCollider.size = selfColliderSize;

            _destinationStateView
                .Subscribe(destination =>
                {
                    if (destination == _stateView)
                    {
                        var vector = _stateView.LocalPosition.normalized * _length;

                        _selfTransition.transform.SetPositionAndRotation(_stateView.Position + vector / 2, Quaternion.FromToRotation(Vector2.up, vector));
                        _transition.gameObject.SetActive(false);
                        _selfTransition.gameObject.SetActive(true);
                    }
                    else
                    {
                        var vector = Vector3.Normalize(destination.Position - _stateView.Position) * _length;

                        _transition.transform.SetPositionAndRotation(_stateView.Position + vector / 2, Quaternion.FromToRotation(Vector2.up, vector));
                        _transition.gameObject.SetActive(true);
                        _selfTransition.gameObject.SetActive(false);
                    }
                })
                .RegisterTo(destroyCancellationToken);

            _ghostDestinationStateView
                .Subscribe(destination =>
                {
                    if (destination != null)
                    {
                        if (destination == _stateView)
                        {
                            var vector = _stateView.LocalPosition.normalized * _length;

                            _selfGhost.transform.SetPositionAndRotation(_stateView.Position + vector / 2, Quaternion.FromToRotation(Vector2.up, vector));
                            _ghost.gameObject.SetActive(false);
                            _selfGhost.gameObject.SetActive(true);
                        }
                        else
                        {
                            var vector = Vector3.Normalize(destination.Position - _stateView.Position) * _length;

                            _ghost.transform.SetPositionAndRotation(_stateView.Position + vector / 2, Quaternion.FromToRotation(Vector2.up, vector));
                            _ghost.gameObject.SetActive(true);
                            _selfGhost.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        _ghost.gameObject.SetActive(false);
                        _selfGhost.gameObject.SetActive(false);
                    }
                })
                .RegisterTo(destroyCancellationToken);
        }

        private void OnDestroy()
        {
            _destinationStateView.Dispose();
            _ghostDestinationStateView.Dispose();
        }
    }
}