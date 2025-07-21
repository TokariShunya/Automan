using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Automan.Game.View
{
    /// <summary>
    /// 文字列のView生成器
    /// </summary>
    public sealed class StringViewFactory : MonoBehaviour
    {
        [SerializeField] private StringView _stringView;
        [SerializeField] private CharacterList _characterList;
        [SerializeField] private float _lineSpacing;

        private int _stringCount;

        /// <summary>
        /// 文字列を表示する
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 文字列のViewを生成
        /// </summary>
        /// <param name="automatonString">文字列のModel</param>
        /// <param name="centerPosition">オートマトンの中心位置</param>
        /// <returns></returns>
        public StringView Create(AutomatonString automatonString, Vector3 centerPosition)
        {
            var stringView = Instantiate(_stringView, transform);

            stringView.transform.localPosition = new Vector3(0f, -_lineSpacing * _stringCount, 0f);

            var characterViews = new List<CharacterView>();

            foreach (var character in automatonString)
            {
                characterViews.Add(_characterList[character]);
            }

            stringView.Initialize(characterViews, centerPosition);
            _stringCount++;

            return stringView;
        }
    }
}

