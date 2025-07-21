using UnityEngine;

namespace Automan.Game.View
{
    /// <summary>
    /// ライフのView
    /// </summary>
    public sealed class LifeView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _lifeRenderer;
        [SerializeField] private Sprite[] _lifeSprites;

        /// <summary>
        /// ライフを設定
        /// </summary>
        /// <param name="life">ライフ</param>
        public void SetLife(int life)
        {
            _lifeRenderer.sprite = _lifeSprites[Mathf.Clamp(life, 0, _lifeSprites.Length - 1)];
        }
    }
}