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

        private readonly int _maxAttempt = 1000;

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
            // 文字を列挙
            AutomatonCharacter[] characters = _characterList.ToArray();

            // オートマトンをランダムに生成
            AutomatonModel automaton = GenerateRandomAutomaton(stateCount, characters);
            AutomatonString[] newStrings = new AutomatonString[stringCount];

            HashSet<string> positiveStrings = new ();
            HashSet<string> negativeStrings = new ();

            var positiveCount = stateCount / 2;
            var negativeCount = stateCount / 2;

            var count = 0;

            if (positiveCount + negativeCount < stringCount)
            {
                if (Random.value < 0.5f)
                {
                    positiveCount++;
                }
                else
                {
                    negativeCount++;
                }
            }

            for (int i = 0; i < _maxAttempt; i++)
            {
                var length = Random.Range(stringLengthRange.Min, stringLengthRange.Max + 1);
                var checkedCharacterCount = Random.Range(checkedCharacterCountRange.Min, checkedCharacterCountRange.Max + 1);
                
                (string text, AutomatonString newString, bool isPositive) = GenerateRandomString(automaton, characters, length, checkedCharacterCount);

                if (isPositive)
                {
                    if (!positiveStrings.Contains(text))
                    {
                        if (positiveCount > 0)
                        {
                            newStrings[count++] = newString;
                            positiveCount--;
                            positiveStrings.Add(text);
                        }
                        
                    }
                }
                else
                {
                    if (!negativeStrings.Contains(text))
                    {
                        if (negativeCount > 0)
                        {
                            newStrings[count++] = newString;
                            negativeCount--;
                            negativeStrings.Add(text);
                        }
                    }
                }

                if (count >= stringCount) break;
            }

            for (int i = 0; i < stringCount - count; i++)
            {
                var length = Random.Range(stringLengthRange.Min, stringLengthRange.Max + 1);
                var checkedCharacterCount = Random.Range(checkedCharacterCountRange.Min, checkedCharacterCountRange.Max + 1);

                (_, AutomatonString newString, _) = GenerateRandomString(automaton, characters, length, checkedCharacterCount);
                newStrings[count++] = newString;
            }

            return newStrings;
        }

        private AutomatonModel GenerateRandomAutomaton(int stateCount, IEnumerable<AutomatonCharacter> characters)
        {
            HashSet<int> randomSet = Enumerable.Range(0, stateCount).Shuffle().Take(Random.Range(1, stateCount)).ToHashSet();

            // 受理状態をランダムに設定
            int[] positiveStates = Enumerable.Range(0, stateCount).Where(value => randomSet.Contains(value)).ToArray();

            List<(int State, AutomatonCharacter Character)> transitionKeys = new ();

            // 遷移のキーを列挙
            for (int i = 0; i < stateCount; i++)
            {
                foreach (AutomatonCharacter character in characters)
                {
                    transitionKeys.Add((i, character));
                }
            }

            (int State, AutomatonCharacter Character)[] shuffledKeys = transitionKeys.Shuffle().ToArray();
            Dictionary<(int State, AutomatonCharacter Character), int> transitions = new ();

            // 遷移をランダムに設定
            for (int i = 0; i < shuffledKeys.Length; i++)
            {
                if (i < stateCount)
                {
                    transitions[shuffledKeys[i]] = i;
                }
                else
                {
                    transitions[shuffledKeys[i]] = Random.Range(0, stateCount);
                }
            }

            // オートマトンを生成
            return new (stateCount, 0, positiveStates, transitions);
        }

        private (string stringText, AutomatonString automatonString, bool isPositive) GenerateRandomString(AutomatonModel automaton, IList<AutomatonCharacter> characters, int length, int checkedCharacterCount)
        {
            automaton.ResetState();

            AutomatonCharacter[] randomCharacters = new AutomatonCharacter[length];

            for (int j = 0; j < length; j++)
            {
                randomCharacters[j] = characters[Random.Range(0, characters.Count)];
            }

            HashSet<int> checkedCharacters = Enumerable.Range(0, length).Shuffle().Take(checkedCharacterCount - 1).Append(length).ToHashSet();

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

            return (randomString.ToString(), new AutomatonString(newCharacters), automaton.IsInPositiveState);
        }
    }
}