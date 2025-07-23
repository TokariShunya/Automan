using System;
using UnityEngine;
using TMPro;

namespace Automan.Game.View
{
    /// <summary>
    /// 時間のView
    /// </summary>
    public sealed class TimeView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _timeText;

        /// <summary>
        /// 時間表示を設定
        /// </summary>
        /// <param name="time">経過時間</param>
        public void SetTime(float time)
        {
            _timeText.SetText(TimeSpan.FromSeconds(time).ToString(@"'<mspace=0.37em>'mm'</mspace>:<mspace=0.37em>'ss'</mspace>.<mspace=0.37em>'fff'</mspace>'"));
        }
    }
}