using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Automan.Game.View;
using UnityEngine;

namespace Automan.Game
{
    /// <summary>
    /// 文字のリスト
    /// </summary>
    [CreateAssetMenu]
    public sealed class CharacterList : ScriptableObject, IEnumerable<AutomatonCharacter>
    {
        [System.Serializable]
        private struct CharacterViewPair
        {
            [SerializeReference, SubclassSelector] private AutomatonCharacter _character;
            public readonly AutomatonCharacter Character => _character;

            [SerializeField] private CharacterView _characterView;
            public readonly CharacterView CharacterView => _characterView;
        }

        [SerializeField] private CharacterViewPair[] _characterViewPairs;

        private Dictionary<AutomatonCharacter, CharacterView> _characterDictionary;

        private readonly PositiveCharacter _positiveCharacter = new ();

        /// <summary>
        /// 正の文字のインスタンス
        /// </summary>
        public PositiveCharacter PositiveCharacter => _positiveCharacter;
        

        private readonly NegativeCharacter _negativeCharacter = new ();

        /// <summary>
        /// 負の文字のインスタンス
        /// </summary>
        public NegativeCharacter NegativeCharacter => _negativeCharacter;
        

        /// <summary>
        /// インデクサ
        /// </summary>
        /// <param name="character">文字モデル</param>
        /// <returns>文字のView</returns>
        public CharacterView this[AutomatonCharacter character]
        {
            get
            {
                _characterDictionary ??= _characterViewPairs.ToDictionary(pair => pair.Character, pair => pair.CharacterView);
                return _characterDictionary[character];
            }
        }

        public IEnumerator<AutomatonCharacter> GetEnumerator()
        {
            foreach (var pair in _characterViewPairs)
            {
                if (pair.Character.IsPositive.HasValue) continue;
                yield return pair.Character;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}