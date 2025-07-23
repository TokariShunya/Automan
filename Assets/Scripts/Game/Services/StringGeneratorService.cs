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
            // 文字を列挙
            AutomatonCharacter[] characters = _characterList.ToArray();

            // オートマトンをランダムに生成
            AutomatonModel automaton = GenerateRandomAutomaton(stateCount, characters);

            // 正例の生成数
            var positiveCount = stringCount / 2 + (stringCount % 2 == 1 ? Random.Range(0, 2) : 0);

            List<(AutomatonCharacter[] Characters, bool[] CheckedPoints)> positiveStrings = new ();
            List<(AutomatonCharacter[] Characters, bool[] CheckedPoints)> negativeStrings = new ();

            foreach ((AutomatonCharacter[] s, bool[] points) in GenerateStrings(automaton, characters, stringLengthRange))
            {
                if (points.Last())
                {
                    positiveStrings.Add((s, points));
                }
                else {
                    negativeStrings.Add((s, points));
                }
            }

            Debug.Log($"Positive Strings: {positiveStrings.Count}");
            Debug.Log($"Negative Strings: {negativeStrings.Count}");
            Debug.Log($"Total Strings: {positiveStrings.Count + negativeStrings.Count}");

            Debug.Log($"Positive Example: {positiveCount}");
            Debug.Log($"Negative Example: {stringCount - positiveCount}");

            List<AutomatonString> newStrings = new ();

            // 正例を生成
            while (positiveStrings.Count > 0 && newStrings.Count < positiveCount)
            {
                foreach ((AutomatonCharacter[] s, bool[] points) in positiveStrings.Shuffle())
                {
                    List<AutomatonCharacter> newCharacters = new ();
                    var checkedCharacterCount = Random.Range(checkedCharacterCountRange.Min, checkedCharacterCountRange.Max + 1);
                    HashSet<int> checkedIndexes = Enumerable.Range(0, points.Length - 1).Shuffle().Take(checkedCharacterCount - 1).Append(points.Length - 1).ToHashSet();
                    string p = $"P: {(points[0] ? "+" : "-")}";

                    if (checkedIndexes.Contains(0))
                    {
                        newCharacters.Add(points[0] ? _characterList.PositiveCharacter : _characterList.NegativeCharacter);
                    }

                    for (int i = 0; i < s.Length; i++)
                    {
                        newCharacters.Add(s[i]);

                        if (checkedIndexes.Contains(i + 1))
                        {
                            newCharacters.Add(points[i + 1] ? _characterList.PositiveCharacter : _characterList.NegativeCharacter);
                        }

                        p += $"{s[i]}{(points[i + 1] ? "+" : "-")}";
                    }

                    newStrings.Add(new AutomatonString(newCharacters));
                    Debug.Log(p);

                    if (newStrings.Count >= positiveCount) break;
                }
            }
            
            // 負例を生成
            while (negativeStrings.Count > 0 && newStrings.Count < stringCount)
            {
                foreach ((AutomatonCharacter[] s, bool[] points) in negativeStrings.Shuffle())
                {
                    List<AutomatonCharacter> newCharacters = new ();
                    var checkedCharacterCount = Random.Range(checkedCharacterCountRange.Min, checkedCharacterCountRange.Max + 1);
                    HashSet<int> checkedIndexes = Enumerable.Range(0, points.Length - 1).Shuffle().Take(checkedCharacterCount - 1).Append(points.Length - 1).ToHashSet();
                    string n = $"N: {(points[0] ? "+" : "-")}";

                    if (checkedIndexes.Contains(0))
                    {
                        newCharacters.Add(points[0] ? _characterList.PositiveCharacter : _characterList.NegativeCharacter);
                    }

                    for (int i = 0; i < s.Length; i++)
                    {
                        newCharacters.Add(s[i]);

                        if (checkedIndexes.Contains(i + 1))
                        {
                            newCharacters.Add(points[i + 1] ? _characterList.PositiveCharacter : _characterList.NegativeCharacter);
                        }

                        n += $"{s[i]}{(points[i + 1] ? "+" : "-")}";
                    }

                    newStrings.Add(new AutomatonString(newCharacters));
                    Debug.Log(n);

                    if (newStrings.Count >= stringCount) break;
                }
            }

            return newStrings.ToArray();
        }

        private AutomatonModel GenerateRandomAutomaton(int stateCount, IEnumerable<AutomatonCharacter> characters)
        {
            HashSet<int> randomSet = Enumerable.Range(0, stateCount).Shuffle().Take(Random.Range(1, stateCount)).ToHashSet();

            // 受理状態をランダムに設定
            int[] positiveStates = Enumerable.Range(0, stateCount).Where(value => randomSet.Contains(value)).ToArray();
            
            Dictionary<(int State, AutomatonCharacter Character), int> transitions = new ();

            // 遷移をランダムに設定
            for (int i = 0; i < stateCount; i++)
            {
                foreach (AutomatonCharacter character in characters)
                {
                    transitions[(i, character)] = Random.Range(0, stateCount);
                }
            }

            // オートマトンを生成
            return new (stateCount, 0, positiveStates, transitions);
        }

        private IEnumerable<(AutomatonCharacter[] Characters, bool[] CheckedPoints)> GenerateStrings(AutomatonModel automaton, IList<AutomatonCharacter> characters, (int Min, int Max) lengthRange)
        {
            for (var length = lengthRange.Min; length <= lengthRange.Max; length++)
            {
                foreach (AutomatonCharacter[] s in GenerateRecursive(characters, length, Enumerable.Empty<AutomatonCharacter>()))
                {
                    AutomatonString automatonString = new (s);
                    List<bool> newPoints = new ();

                    automaton.ResetState();

                    newPoints.Add(automaton.IsInPositiveState);

                    while (!automatonString.IsEmpty)
                    {
                        automaton.Transition(automatonString);
                        newPoints.Add(automaton.IsInPositiveState);
                    }

                    yield return (s, newPoints.ToArray());
                }
            }
        }

        private IEnumerable<AutomatonCharacter[]> GenerateRecursive(IList<AutomatonCharacter> characters, int length, IEnumerable<AutomatonCharacter> current)
        {
            if (current.Count() == length)
            {
                yield return current.ToArray();
                yield break;
            }

            foreach (AutomatonCharacter character in characters)
            {
                foreach (AutomatonCharacter[] s in GenerateRecursive(characters, length, current.Append(character)))
                {
                    yield return s;
                }
            }
        }
    }
}