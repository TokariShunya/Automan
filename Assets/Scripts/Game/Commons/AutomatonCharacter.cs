using System;

#nullable enable

namespace Automan.Game
{
    /// <summary>
    /// 文字のModel
    /// </summary>
    public class AutomatonCharacter : IEquatable<AutomatonCharacter>
    {
        /// <summary>
        /// 文字の正負
        /// </summary>
        public virtual bool? IsPositive => null;

        public virtual bool Equals(AutomatonCharacter other)
        {
            return other is not null;
        }
    }

    /// <summary>
    /// 文字A
    /// </summary>
    [System.Serializable]
    public sealed class CharacterA : AutomatonCharacter
    {
        /// <summary>
        /// 文字列化する
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return "A";
        }

        public override bool Equals(AutomatonCharacter other)
        {
            return other is CharacterA;
        }

        public override bool Equals(object obj)
        {
            return obj is CharacterA;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    /// <summary>
    /// 文字B
    /// </summary>
    [System.Serializable]
    public sealed class CharacterB : AutomatonCharacter
    {
        /// <summary>
        /// 文字列化する
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return "B";
        }

        public override bool Equals(AutomatonCharacter other)
        {
            return other is CharacterB;
        }

        public override bool Equals(object obj)
        {
            return obj is CharacterB;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    /// <summary>
    /// 正の文字
    /// </summary>
    [System.Serializable]
    public sealed class PositiveCharacter : AutomatonCharacter
    {
        public override bool? IsPositive => true;

        /// <summary>
        /// 文字列化する
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return "+";
        }

        public override bool Equals(AutomatonCharacter other)
        {
            return other is PositiveCharacter;
        }

        public override bool Equals(object obj)
        {
            return obj is PositiveCharacter;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    /// <summary>
    /// 負の文字
    /// </summary>
    [System.Serializable]
    public sealed class NegativeCharacter : AutomatonCharacter
    {
        public override bool? IsPositive => false;

        /// <summary>
        /// 文字列化する
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return "-";
        }

        public override bool Equals(AutomatonCharacter other)
        {
            return other is NegativeCharacter;
        }

        public override bool Equals(object obj)
        {
            return obj is NegativeCharacter;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}