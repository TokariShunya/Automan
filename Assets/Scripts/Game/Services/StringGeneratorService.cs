using System.Collections.Generic;
using System.Linq;
using Automan.Game.Model;
using UnityEngine;
using VContainer;

#nullable enable

namespace Automan.Game.Service
{
    /// <summary>
    /// 文字列Model生成器
    /// </summary>
    public sealed class StringGeneratorService
    {
        private readonly CharacterList _characterList;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="characterList">文字のリスト</param>
        [Inject]
        public StringGeneratorService(CharacterList characterList)
        {
            _characterList = characterList;
        }

        /// <summary>
        /// 文字列のModelをランダムに生成
        /// </summary>
        /// <param name="stringCount">生成する文字列の数</param>
        /// <param name="stateCount">オートマトンの状態数</param>
        /// <param name="stringLengthRange">生成する文字列の長さの範囲</param>
        /// <param name="checkedCharacterCountRange">生成する文字列の正または負の文字の数の範囲</param>
        /// <returns>文字列Modelの配列</returns>
        public AutomatonString[] Generate(int stringCount, int stateCount, (int Min, int Max) stringLengthRange, (int Min, int Max) checkedCharacterCountRange)
        {
            int[] positiveStates = Enumerable.Range(0, stringCount).Where(_ => Random.value < 0.5f).ToArray();

            Dictionary<(int State, AutomatonCharacter Character), int> transitions = new ();

            AutomatonCharacter[] characters = _characterList.ToArray();

            for (int i = 0; i < stateCount; i++)
            {
                foreach (AutomatonCharacter character in characters)
                {
                    transitions[(i, character)] = Random.Range(0, stateCount);
                }
            }

            AutomatonModel automaton = new (stateCount, 0, positiveStates, transitions);
            AutomatonString[] newStrings = new AutomatonString[stringCount];

            for (int i = 0; i < stringCount; i++)
            {
                automaton.ResetState();

                var length = Random.Range(stringLengthRange.Min, stringLengthRange.Max + 1);
                AutomatonCharacter[] randomCharacters = new AutomatonCharacter[length];

                for (int j = 0; j < length; j++)
                {
                    randomCharacters[j] = characters[Random.Range(0, characters.Length)];
                }

                var checkedCount = Random.Range(checkedCharacterCountRange.Min, checkedCharacterCountRange.Max + 1);
                HashSet<int> checkedCharacters = Enumerable.Range(0, length).Shuffle().Take(checkedCount - 1).Append(length).ToHashSet();

                AutomatonString randomString = new (randomCharacters);

                var index = 0;
                List<AutomatonCharacter> newCharacters = new ();

                if (checkedCharacters.Contains(index++))
                {
                    newCharacters.Add(automaton.IsInPositiveState ? _characterList.PositiveCharacter : _characterList.NegativeCharacter);
                }

                while (!randomString.IsEmpty)
                {
                    newCharacters.Add(randomString.Peek());

                    automaton.Transition(randomString);

                    if (checkedCharacters.Contains(index++))
                    {
                        newCharacters.Add(automaton.IsInPositiveState ? _characterList.PositiveCharacter : _characterList.NegativeCharacter);
                    }
                }

                newStrings[i] = new AutomatonString(newCharacters);
            }

            return newStrings;
        }
    }
}