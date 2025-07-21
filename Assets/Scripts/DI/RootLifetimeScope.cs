using Automan.Root;
using Automan.Root.Model;
using Automan.Root.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Automan.DI
{
    /// <summary>
    /// ルートのLifetimeScope
    /// </summary>
    public sealed class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] private TransitionFadeView _transitionFadeView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<TransitionManager>(Lifetime.Singleton);
            builder.Register<LifeModel>(Lifetime.Singleton);
            builder.RegisterComponent(_transitionFadeView);
        }
    }
}