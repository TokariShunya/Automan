using System.Collections.Generic;
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
        [SerializeField] private SerializableDictionary<SoundManager.Sound, AudioClip> _soundDictionary;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private TransitionFadeView _transitionFadeView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<TransitionManager>(Lifetime.Singleton);
            builder.Register<SoundManager>(Lifetime.Singleton);
            builder.Register<LifeModel>(Lifetime.Singleton);
            builder.Register<TimeModel>(Lifetime.Singleton);

            builder.RegisterInstance<IReadOnlyDictionary<SoundManager.Sound, AudioClip>>(_soundDictionary);

            builder.RegisterComponent(_audioSource);
            builder.RegisterComponent(_transitionFadeView);
        }
    }
}