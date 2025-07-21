using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Automan.Game
{
    /// <summary>
    /// 文字列のModel
    /// </summary>
    public sealed class AutomatonString : IEnumerable<AutomatonCharacter>
    {
        private readonly Queue<AutomatonCharacter> _characters;

        /// <summary>
        /// 文字列の長さを取得する
        /// </summary>
        public int Length => _characters.Count;

        /// <summary>
        /// 空文字列かどうか
        /// </summary>
        public bool IsEmpty => Length == 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="characters">文字とその正負</param>
        public AutomatonString(IEnumerable<AutomatonCharacter> characters)
        {
            _characters = new Queue<AutomatonCharacter>(characters);
        }

        /// <summary>
        /// 先頭の文字を取得し，取り除く
        /// </summary>
        /// <returns>先頭の文字とその正負</returns>
        /// <exception cref="InvalidOperationException">文字列が空文字列</exception>
        public AutomatonCharacter Pop()
        {
            try
            {
                return _characters.Dequeue();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("空文字列です");
            }
        }
        
        /// <summary>
        /// 先頭の文字を取得する
        /// </summary>
        /// <returns>先頭の文字とその正負</returns>
        /// <exception cref="InvalidOperationException">文字列が空文字列</exception>
        public AutomatonCharacter Peek()
        {
            try
            {
                return _characters.Peek();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("空文字列です");
            }
        }

        /// <summary>
        /// 列挙子を取得
        /// </summary>
        /// <returns></returns>
        public IEnumerator<AutomatonCharacter> GetEnumerator()
        {
            return _characters.GetEnumerator();
        }

        /// <summary>
        /// 文字列化する
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return string.Concat(_characters.Select(character => character.ToString()));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}