using Automan.Game;
using Automan.Game.Model;
using Automan.Game.Service;
using Automan.Game.Presenter;
using Automan.Game.View;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Automan.DI
{
    /// <summary>
    /// パズルモードのLifetimeScope
    /// </summary>
    public sealed class PuzzleModeLifetimeScope : LifetimeScope
    {
        [Header("ステージ設定")]
        [SerializeField] private int _stageNumber;
        [SerializeField] private StageStringConfiguration _stageStringConfiguration;
        [SerializeField] private CharacterList _characterList;

        [Header("Viewの参照")]
        [SerializeField] private StringViewFactory _stringViewFactory;
        [SerializeField] private AutomatonView _automatonView;
        [SerializeField] private LifeView _lifeView;
        [SerializeField] private TimeView _timeView;
        [SerializeField] private FrameView _frameView;
        [SerializeField] private InformationTextView _informationTextView;
        [SerializeField] private Button _startButton;
        [SerializeField] private GameoverButtonsView _gameoverButtonsView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<StageModel>(Lifetime.Singleton).WithParameter(_stageNumber);
            builder.Register<StringGeneratorService>(Lifetime.Singleton);

            builder.RegisterInstance(_stageStringConfiguration);

            builder.RegisterEntryPoint<PuzzleModePresenter>(Lifetime.Singleton);

            builder.RegisterComponent(_characterList);
            builder.RegisterComponent(_stringViewFactory);
            builder.RegisterComponent(_automatonView);
            builder.RegisterComponent(_lifeView);
            builder.RegisterComponent(_timeView);
            builder.RegisterComponent(_frameView);
            builder.RegisterComponent(_informationTextView);
            builder.RegisterComponent(_startButton);
            builder.RegisterComponent(_gameoverButtonsView);
        }
    }
}