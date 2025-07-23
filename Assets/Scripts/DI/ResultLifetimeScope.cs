using Automan.Game.View;
using Automan.Result.Presenter;
using Automan.Result.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Automan.DI
{
    /// <summary>
    /// リザルトのLifetimeScope
    /// </summary>
    public sealed class ResultLifetimeScope : LifetimeScope
    {
        [SerializeField] private AllClearButtonsView _allClearButtonsView;
        [SerializeField] private ResultRecordView _resultRecordView;
        [SerializeField] private LifeView _lifeView;
        [SerializeField] private FrameView _frameView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<ResultPresenter>(Lifetime.Singleton);

            builder.RegisterComponent(_allClearButtonsView);
            builder.RegisterComponent(_resultRecordView);
            builder.RegisterComponent(_lifeView);
            builder.RegisterComponent(_frameView);
        }
    }
}