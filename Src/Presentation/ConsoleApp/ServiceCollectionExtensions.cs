using Application.Interfaces;
using Application;
using Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Presentation.ConsoleApp.Helpers;
using Application.Gesjaakt;
using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.Gesjaakt;
using Domain.Interfaces.Games.TakeFive;
using Application.TakeFive;
using Domain.Interfaces.Games.BaseGame;
using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;
using Visualization;

namespace ConsoleApp;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLoggingInfra(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(typeof(Application.Interfaces.ILogger<>), typeof(Infrastructure.Logging.Logger<>));
        // NOTE: A custom GameEventHandler is used to share events (i.e. log) with user. Info is the minimum used. This handler decides which events are logged and not

        serviceCollection.AddLogging(config =>
        {
            config.AddSimpleConsole(options => options.IncludeScopes = true);
            config.SetMinimumLevel(LogLevel.Information);
        });
        return serviceCollection;
    }

    public static IServiceCollection AddGeneralServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<App>();
        serviceCollection.AddSingleton<IPlayerInputProvider, CLIPlayerInputProvider>();
        serviceCollection.AddTransient<IGameRunnerEventCollector, GameRunnerEventCollector>();
        serviceCollection.AddSingleton<IGameEventHandler, GameEventHandler>();
        return serviceCollection;
    }

    // REFACTOR - add documentation/ explanation
    public static IServiceCollection AddDynamicBehaviourToChooseGame(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<GameRunnerFactory>();

        // Note: this singleton allows to dynamically retrieve the correct gamerunner
        serviceCollection.AddSingleton(sp => new Dictionary<Type, Func<IGameRunner>>
        {
            [typeof(GesjaaktGame)] = () => sp.GetRequiredService<GameRunner<IGesjaaktPlayer>>(),
            [typeof(TakeFiveGame)] = () => sp.GetRequiredService<GameRunner<ITakeFivePlayer>>()
        });
        // Note: to allow user to choose game
        serviceCollection.AddSingleton(sp => new List<IGameOption>
        {
            new GameOption<GesjaaktGame>(3,7), // REFACTOR - create gesjaakt rule 
            new GameOption<TakeFiveGame>(TakeFiveRules.MinNumberOfPlayers,TakeFiveRules.MaxNumberOfPlayers)
        });
        return serviceCollection;
    }

    public static IServiceCollection AddGesjaaktGame(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<GameRunner<IGesjaaktPlayer>>();
        serviceCollection.AddTransient<IGame<IGesjaaktPlayer>, GesjaaktGame>();
        serviceCollection.AddSingleton<IPlayerFactory<IGesjaaktPlayer>, GesjaaktPlayerFactory>();
        serviceCollection.AddTransient<IGesjaaktGameEventCollector, GesjaaktGameEventCollector>();
        serviceCollection.AddTransient<IStatisticsCreator, GesjaaktVisualizer>();

        return serviceCollection;
    }

    public static IServiceCollection AddTakeFiveGame(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<GameRunner<ITakeFivePlayer>>();
        serviceCollection.AddTransient<IGame<ITakeFivePlayer>, TakeFiveGame>();
        serviceCollection.AddSingleton<IPlayerFactory<ITakeFivePlayer>, TakeFivePlayerFactory>();
        //serviceCollection.AddTransient<ITakeFiveGameDealer, TakeFiveGameDealer>();
        serviceCollection.AddTransient<ITakeFiveGameEventCollector, TakeFiveGameEventCollector>();

        return serviceCollection;
    }
}
