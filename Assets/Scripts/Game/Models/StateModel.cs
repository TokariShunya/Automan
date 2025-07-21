#nullable enable

namespace Automan.Game.Model
{
    /// <summary>
    /// 状態のModel
    /// </summary>
    public sealed class StateModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// 受理状態かどうか
        /// </summary>
        public bool IsPositive { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="isPositive">受理状態かどうか</param>
        public StateModel(int id, bool isPositive)
        {
            Id = id;
            IsPositive = isPositive;
        }
    }
}