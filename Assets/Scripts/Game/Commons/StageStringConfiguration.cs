using UnityEngine;

namespace Automan.Game
{
    /// <summary>
    /// ステージの文字列生成設定
    /// </summary>
    [System.Serializable]
    public struct StageStringConfiguration
    {
        [SerializeField] private int _stringCount;

        /// <summary>
        /// 生成する文字列の数
        /// </summary>
        public readonly int StringCount => _stringCount;

        [SerializeField] private int _stringMinLength;
        [SerializeField] private int _stringMaxLength;

        /// <summary>
        /// 文字列の長さの範囲
        /// </summary>
        public readonly (int Min, int Max) StringLengthRange => (_stringMinLength, _stringMaxLength);

        [SerializeField] private int _checkedCharacterMinCount;
        [SerializeField] private int _checkedCharacterMaxCount;

        /// <summary>
        /// 正または負の文字の数の範囲
        /// </summary>
        public readonly (int Min, int Max) CheckedCharacterCountRange => (_checkedCharacterMinCount, _checkedCharacterMaxCount);
    }
}