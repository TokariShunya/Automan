using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R3;
using R3.Triggers;

namespace Automan.Game.View
{
    /// <summary>
    /// オートマトンのView
    /// </summary>
    public sealed class AutomatonView : MonoBehaviour
    {
        [SerializeField] private Transform _states;
        [SerializeField] private Transform _transitions;
        [SerializeField] private float _selfTransitionRadius;

        private StateView[] _stateViews;
        private TransitionView[] _transitionViews;

        private StateView[] StateViews
        {
            get
            {
                _stateViews ??= _states.GetComponentsInChildren<StateView>();
                return _stateViews;
            }
        }

        private TransitionView[] TransitionViews
        {
            get
            {
                _transitionViews ??= _transitions.GetComponentsInChildren<TransitionView>();
                return _transitionViews;
            }
        }

        /// <summary>
        /// 状態数
        /// </summary>
        public int StateCount => StateViews.Length;

        /// <summary>
        /// 各状態の位置
        /// </summary>
        public Dictionary<int, Vector3> StatePositions => StateViews.ToDictionary(stateView => stateView.Id, stateView => stateView.Position);

        /// <summary>
        /// 中心の位置
        /// </summary>
        public Vector3 CenterPosition => _states.position;

        private readonly Subject<(int State, bool IsPositive)> _onStateChanged = new ();

        /// <summary>
        /// 状態変更イベント
        /// </summary>
        public Observable<(int State, bool IsPositive)> OnStateChanged => _onStateChanged.AsObservable();

        private readonly Subject<(int State, AutomatonCharacter Character, int Destination)> _onTransitionChanged = new ();

        /// <summary>
        /// 遷移変更イベント
        /// </summary>
        public Observable<(int State, AutomatonCharacter Character, int Destination)> OnTransitionChanged => _onTransitionChanged.AsObservable();

        private void Start()
        {
            foreach (var stateView in StateViews)
            {
                stateView.IsPositive
                    .Select(isPositive => ((stateView.Id, isPositive)))
                    .Subscribe(state =>
                    {
                        _onStateChanged.OnNext(state);
                    })
                    .RegisterTo(destroyCancellationToken);
            }

            foreach (var transitionView in TransitionViews)
            {
                transitionView.DestinationStateView
                    .Select(destination => (transitionView.Key.State, transitionView.Key.Character, destination.Id))
                    .Subscribe(transition =>
                    {
                        _onTransitionChanged.OnNext(transition);
                    })
                    .RegisterTo(destroyCancellationToken);

                transitionView.gameObject.AddComponent<ObservableBeginDragTrigger>().OnBeginDragAsObservable()
                    .Subscribe(eventData =>
                    {
                        transitionView.GhostDestinationStateView.Value = transitionView.DestinationStateView.CurrentValue;
                    })
                    .RegisterTo(destroyCancellationToken);

                transitionView.gameObject.AddComponent<ObservableDragTrigger>().OnDragAsObservable()
                    .Subscribe(eventData =>
                    {
                        var point = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10f));

                        transitionView.GhostDestinationStateView.Value = CalculateDestinationState(transitionView.StateView, point);
                    })
                    .RegisterTo(destroyCancellationToken);

                transitionView.gameObject.AddComponent<ObservableEndDragTrigger>().OnEndDragAsObservable()
                    .Subscribe(eventData =>
                    {
                        var point = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10f));

                        transitionView.DestinationStateView.Value = CalculateDestinationState(transitionView.StateView, point);
                        transitionView.GhostDestinationStateView.Value = default;
                    })
                    .RegisterTo(destroyCancellationToken);
            }
        }

        private StateView CalculateDestinationState(StateView fromState, Vector3 point)
        {
            var max = float.NegativeInfinity;
            StateView state = default;

            var vectorToPoint = point - fromState.Position;

            if (vectorToPoint.magnitude <= _selfTransitionRadius) return fromState;

            foreach (var stateView in StateViews.Where(stateView => stateView != fromState))
            {
                var vectorToState = stateView.Position - fromState.Position;
                var dot = Vector2.Dot(vectorToPoint.normalized, vectorToState.normalized);

                if (dot > max)
                {
                    state = stateView;
                    max = dot;
                }
            }

            return state;
        }

        /// <summary>
        /// 状態を固定する
        /// </summary>
        public void FixStates()
        {
            foreach (var stateView in StateViews)
            {
                stateView.Fix();
            }
        }

        /// <summary>
        /// 状態をハイライトする
        /// </summary>
        /// <param name="stateId">状態のID</param>
        /// <param name="isCorrect">判定</param>
        public void HighlightState(int stateId, bool isCorrect)
        {
            var stateView = StateViews.FirstOrDefault(state => state.Id == stateId);

            if (stateView != null)
            {
                stateView.HighlightAsync(isCorrect).Forget();
            }
        }

        private void OnDestroy()
        {
            _onStateChanged.Dispose();
            _onTransitionChanged.Dispose();
        }
    }
}