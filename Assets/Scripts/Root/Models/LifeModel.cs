using R3;

#nullable enable

namespace Automan.Root.Model
{
    /// <summary>
    /// ライフのModel
    /// </summary>
    public sealed class LifeModel
    {
        private readonly ReactiveProperty<int> _life = new ();

        /// <summary>
        /// ライフ
        /// </summary>
        public ReadOnlyReactiveProperty<int> Life => _life;

        /// <summary>
        /// 生存しているかどうか
        /// </summary>
        public bool IsSurvived => _life.CurrentValue > 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LifeModel()
        {
            Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            _life.Value = 3;
        }

        /// <summary>
        /// ダメージを与える
        /// </summary>
        public void Damage()
        {
            _life.Value--;
            if (_life.CurrentValue < 0) _life.Value = 0;
        }
    }
}

