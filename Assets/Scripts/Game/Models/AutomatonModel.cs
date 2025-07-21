using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

#nullable enable

namespace Automan.Game.Model
{
    /// <summary>
    /// オートマトンのModel
    /// </summary>
    public sealed class AutomatonModel
    {
        private readonly StateModel[] _states;
        private readonly Dictionary<(StateModel State, AutomatonCharacter Character), StateModel> _transitions;

        private StateModel _initialState;
        private StateModel _currentState;

        /// <summary>
        /// 現在の状態
        /// </summary>
        public int CurrentState => _currentState.Id;

        /// <summary>
        /// 現在受理状態にあるか
        /// </summary>
        public bool IsInPositiveState => _currentState.IsPositive;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="stateCount">状態の数</param>
        /// <param name="initialState">開始状態</param>
        public AutomatonModel(int stateCount, int initialState)
        {
            _states = new StateModel[stateCount];

            for (int i = 0; i < stateCount; i++)
            {
                _states[i] = new StateModel(i, false);
            }

            _initialState = _states[initialState];
            _currentState = _initialState;

            _transitions = new Dictionary<(StateModel State, AutomatonCharacter Character), StateModel>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="stateCount">状態の数</param>
        /// <param name="initialState">開始状態</param>
        /// <param name="positiveStates">受理状態の集合</param>
        /// <param name="transitions">遷移辞書</param>
        public AutomatonModel(int stateCount, int initialState, ICollection<int> positiveStates, IReadOnlyDictionary<(int State, AutomatonCharacter Character), int> transitions)
        {
            _states = new StateModel[stateCount];

            for (int i = 0; i < stateCount; i++)
            {
                _states[i] = new StateModel(i, positiveStates.Contains(i));
            }

            _initialState = _states[initialState];
            _currentState = _initialState;

            _transitions = new(transitions.Select(pair => new KeyValuePair<(StateModel, AutomatonCharacter), StateModel>((_states[pair.Key.State], pair.Key.Character), _states[pair.Value])));
        }

        /// <summary>
        /// 状態遷移する
        /// </summary>
        /// <param name="currentString">現在の文字列</param>
        /// <returns>取り除いた文字の正負</returns>
        /// <exception cref="InvalidOperationException">現在の文字列が空文字列，または遷移先が存在しない</exception>
        public bool? Transition(AutomatonString currentString)
        {
            try
            {
                AutomatonCharacter character = currentString.Pop();

                if (!character.IsPositive.HasValue)
                {
                    _currentState = _transitions[(_currentState, character)];
                }
                
                return character.IsPositive;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("空文字列です");
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidOperationException("遷移先が存在しません");
            }
        }

        /// <summary>
        /// 状態を変更する
        /// </summary>
        /// <param name="state">状態</param>
        /// <param name="isPositive">受理状態かどうか</param>
        public void ChangeState(int state, bool isPositive)
        {
            _states[state].IsPositive = isPositive;
        }

        /// <summary>
        /// 遷移を変更する
        /// </summary>
        /// <param name="state">状態</param>
        /// <param name="character">文字</param>
        /// <param name="destination">遷移先の状態</param>
        public void ChangeTransition(int state, AutomatonCharacter character, int destination)
        {
            _transitions[(_states[state], character)] = _states[destination];
        }

        /// <summary>
        /// 現在の状態を開始状態に戻す
        /// </summary>
        public void ResetState()
        {
            _currentState = _initialState;
        }
    }
}