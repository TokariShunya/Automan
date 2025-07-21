using Automan.Root.View;
using Automan.Title.Presenter;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Automan.DI
{
    /// <summary>
    /// タイトルのLifetimeScope
    /// </summary>
    public sealed class TitleLifetimeScope : LifetimeScope
    {
        [SerializeField] private HoverButton _playButton;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<TitlePresenter>(Lifetime.Singleton);
    
            builder.RegisterComponent(_playButton);
        }
    }
}