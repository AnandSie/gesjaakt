using Application;
using ConsoleApp;
using ConsoleApp.Helpers;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Domain.Interfaces.Games.Gesjaakt;
using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.BaseGame;
using ScottPlot.Statistics;
using Visualization;

// TODO: IOC
var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging(config =>
{
    config.AddSimpleConsole(options =>
    {
        options.IncludeScopes = true;
    });
    config.SetMinimumLevel(LogLevel.Critical);
});

serviceCollection.AddSingleton(typeof(Domain.Interfaces.ILogger<>), typeof(Logging.Logger<>));

// Games TODO: make seperate factory
serviceCollection.AddTransient<Func<int, IGame>>(serviceProvider => key =>
{
    return key switch
    {
        1 => serviceProvider.GetRequiredService<GesjaaktGame>(),
        //"B" => serviceProvider.GetRequiredService<TakeFiveGame>(),
        _ => throw new ArgumentException("Unknown Game Type")
    };
});


// Application
serviceCollection.AddTransient<GesjaaktGame>();
serviceCollection.AddTransient<IVisualizer, Visualizer>();

serviceCollection.AddSingleton<IPlayerInputProvider, ConsoleInputService>();
serviceCollection.AddSingleton<IPlayerFactory, PlayerFactory>();
serviceCollection.AddSingleton<IGameDealerFactory, GameDealerFactory>();
serviceCollection.AddSingleton<ISimulator, Simulator>();
serviceCollection.AddSingleton<App>();
serviceCollection.AddTransient<IGesjaaktGameState, GesjaaktGameState>();
serviceCollection.AddTransient<Func<IGesjaaktGameState>>(sp => () => sp.GetRequiredService<IGesjaaktGameState>());

var serviceProvider = serviceCollection.BuildServiceProvider();

// Get logger for the Program class
var logger = serviceProvider.GetRequiredService<Domain.Interfaces.ILogger<Program>>();
logger.LogInformation("Starting application...");




var app = serviceProvider.GetRequiredService<App>();
app.Start();

