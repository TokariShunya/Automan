using R3;

#nullable enable

namespace Automan.Root.Model
{
    /// <summary>
    /// 時間のモデル
    /// </summary>
    public sealed class TimeModel
    {
        private readonly ReactiveProperty<float> _time = new ();

        /// <summary>
        /// 経過時間
        /// </summary>
        public ReadOnlyReactiveProperty<float> Time => _time;

        /// <summary>
        /// 罰
        /// </summary>
        public float Penalty => 10;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TimeModel()
        {
            Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            _time.Value = 0f;
        }

        /// <summary>
        /// 時間を更新
        /// </summary>
        /// <param name="delta">増分</param>
        public void UpdateTime(float delta)
        {
            _time.Value += delta;
        }
    }
}