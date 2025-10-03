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
        serviceCollection.AddTransient<SimulationConfiguration>();
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

        // Note: allows user to choose game by injecting this in App.cs
        serviceCollection.AddSingleton(sp => new List<GameOption>
        {
            sp.GetRequiredService<GesjaaktGame>(),
            sp.GetRequiredService<TakeFiveGame>(),
        });
        return serviceCollection;
    }

    public static IServiceCollection AddGesjaaktGame(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<GameRunner<IGesjaaktPlayer>>();

        serviceCollection.AddTransient<IGame<IGesjaaktPlayer>, GesjaaktGame>();
        serviceCollection.AddTransient<GesjaaktGame>();
        
        serviceCollection.AddSingleton<IPlayerFactory<IGesjaaktPlayer>, GesjaaktPlayerFactory>();
        serviceCollection.AddTransient<IGesjaaktGameEventCollector, GesjaaktGameEventCollector>();
        
        serviceCollection.AddTransient<IStatisticsCreator, GesjaaktVisualizer>();

        return serviceCollection;
    }

    public static IServiceCollection AddTakeFiveGame(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<GameRunner<ITakeFivePlayer>>();

        // REFACTOR - Only define Game Once - it is now required for the GameOption & GameRunner. Maybe solve this by letting IGame extend GameOption instead of Game
        serviceCollection.AddTransient<IGame<ITakeFivePlayer>, TakeFiveGame>();
        serviceCollection.AddTransient<TakeFiveGame>();
        
        serviceCollection.AddSingleton<IPlayerFactory<ITakeFivePlayer>, TakeFivePlayerFactory>();
        serviceCollection.AddTransient<ITakeFiveGameEventCollector, TakeFiveGameEventCollector>();

        return serviceCollection;
    }
}
