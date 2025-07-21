#nullable enable

namespace Automan.Game.Model
{
    /// <summary>
    /// ステージのModel
    /// </summary>
    public sealed class StageModel
    {
        /// <summary>
        /// ステージ数
        /// </summary>
        public int StageCount => 5;

        /// <summary>
        /// 現在のステージ番号
        /// </summary>
        public int CurrentStageNumber { get; }

        /// <summary>
        /// ステージの文字列生成設定
        /// </summary>
        public StageStringConfiguration StageStringConfiguration { get; }

        /// <summary>
        /// ミスの回数
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// パーフェクトかどうか
        /// </summary>
        public bool IsPerfect => ErrorCount == 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="stageNumber">ステージ番号</param>
        /// <param name="stageStringConfiguration">ステージの文字列生成設定</param>
        public StageModel(int stageNumber, StageStringConfiguration stageStringConfiguration)
        {
            CurrentStageNumber = stageNumber;
            StageStringConfiguration = stageStringConfiguration;
            ErrorCount = 0;
        }
    }
}