using System;
using UnityEngine;
using TMPro;

namespace Automan.Result.View
{
    /// <summary>
    /// リザルトのView
    /// </summary>
    public sealed class ResultRecordView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _clearTimeText;
        [SerializeField] private TextMeshPro _penaltyText;
        [SerializeField] private TextMeshPro _recordTimeText;

        /// <summary>
        /// 記録を設定
        /// </summary>
        /// <param name="clearTime">クリア時間</param>
        /// <param name="errorCount">ミスの回数</param>
        /// <param name="penalty">罰</param>
        /// <param name="recordTime">記録時間</param>
        public void SetRecords(float clearTime, int errorCount, float penalty, float recordTime)
        {
            _clearTimeText.SetText(TimeSpan.FromSeconds(clearTime).ToString(@"'<mspace=0.37em>'mm'</mspace>:<mspace=0.37em>'ss'</mspace>.<mspace=0.37em>'fff'</mspace>'"));
            _penaltyText.SetText($"<size=0.5>ERROR</size> {errorCount} <size=0.5>x {penalty} sec</size>");
            _recordTimeText.SetText(TimeSpan.FromSeconds(recordTime).ToString(@"'<mspace=0.37em>'mm'</mspace>:<mspace=0.37em>'ss'</mspace>.<mspace=0.37em>'fff'</mspace>'"));
        }
    }
}