using Application.Interfaces;
using Application;
using Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Presentation.ConsoleApp.Helpers;
using Application.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;
using Domain.Interfaces.Games.TakeFive;
using Application.TakeFive;
using Application.Qwixx;
using Domain.Interfaces.Games.Qwixx;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces;
using Visualization;
using UserInterface;

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
            [typeof(TakeFiveGame)] = () => sp.GetRequiredService<GameRunner<ITakeFivePlayer>>(),
            [typeof(QwixxGame)] = () => sp.GetRequiredService<GameRunner<IQwixxPlayer>>()
        });

        // Note: allows user to choose game by injecting this in App.cs
        serviceCollection.AddSingleton(sp => new List<GameOption>
        {
            sp.GetRequiredService<GesjaaktGame>(),
            sp.GetRequiredService<TakeFiveGame>(),
            sp.GetRequiredService<QwixxGame>(),
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

    // No event collector is registered here - QwixxGameDealer deliberately raises no
    // console/UI events yet (see docs/qwixx/status-report.md open items).
    public static IServiceCollection AddQwixxGame(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<GameRunner<IQwixxPlayer>>();

        serviceCollection.AddTransient<IGame<IQwixxPlayer>, QwixxGame>();
        serviceCollection.AddTransient<QwixxGame>();

        serviceCollection.AddSingleton<IPlayerFactory<IQwixxPlayer>, QwixxPlayerFactory>();

        return serviceCollection;
    }

    public static IServiceCollection AddConsoleVisualization(this IServiceCollection services, bool useLiveDisplay)
    {
        // Singleton because the pinned status line is a single, shared piece of console state
        services.AddSingleton<IDisplay>(_ => new ConsoleDisplay(useLiveDisplay));

        return services;
    }
}
