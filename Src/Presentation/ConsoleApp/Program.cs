using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Application;
using Application.Gesjaakt;
using Application.TakeFive;
using ConsoleApp;
using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;
using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.BaseGame;
using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;
using Presentation.ConsoleApp.Helpers;
using Visualization;

var serviceCollection = new ServiceCollection();

var loglevel = LogLevel.Warning;
serviceCollection.AddLogging(config =>
{
    config.AddSimpleConsole(options =>
    {
        options.IncludeScopes = true;
    });
    // TODO: move this to the class which is actually doing something with it? or also show it there?
    config.SetMinimumLevel(loglevel);
});
serviceCollection.AddSingleton(typeof(Domain.Interfaces.ILogger<>), typeof(Infrastructure.Logging.Logger<>));

serviceCollection.AddSingleton<IPlayerInputProvider, ConsoleInputService>();
serviceCollection.AddTransient<GameRunnerFactory>();

// Gesjaakt
serviceCollection.AddSingleton<GameRunner<IGesjaaktPlayer>>();
serviceCollection.AddTransient<IGame<IGesjaaktPlayer>, GesjaaktGame>();
serviceCollection.AddSingleton<IPlayerFactory<IGesjaaktPlayer>, GesjaaktPlayerFactory>();
serviceCollection.AddTransient<IGesjaaktGameDealer, GesjaaktGameDealer>();
serviceCollection.AddTransient<IVisualizer, GesjaaktVisualizer>();
serviceCollection.AddTransient<GesjaaktGameEventOrchestrator>();

// TakeFive
serviceCollection.AddSingleton<GameRunner<ITakeFivePlayer>>();
serviceCollection.AddTransient<IGame<ITakeFivePlayer>, TakeFiveGame>();
serviceCollection.AddSingleton<IPlayerFactory<ITakeFivePlayer>, TakeFivePlayerFactory>();

// REFACTOR - gamerunner<T> and game<T> seems a bit double (or it is just different responsibilites with both need a generic)
// Note: allow to dynamically retrieve the correct gamerunner
serviceCollection.AddSingleton(sp => new Dictionary<Type, Func<IGameRunner>>
{
    // TODO: dont'use general type, use somethign specific related to Game
    [typeof(GesjaaktGame)] = () => sp.GetRequiredService<GameRunner<IGesjaaktPlayer>>(),
    [typeof(TakeFiveGame)] = () => sp.GetRequiredService<GameRunner<ITakeFivePlayer>>()
});

// REFACTOR - Create a specific GameOption which uses an IGame (which has a name)
serviceCollection.AddSingleton(sp => new List<Option>
{
    new GameOption<GesjaaktGame>(),
    new GameOption<TakeFiveGame>()
});

serviceCollection
    .AddTransient<App>()
    .BuildServiceProvider()
    .GetRequiredService<App>()
    .Start();