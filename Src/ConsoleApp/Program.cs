using Application;
using ConsoleApp;
using ConsoleApp.Helpers;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Domain.Entities.Components;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.Gesjaakt;
using Domain.Entities.Game.Gesjaakt;

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

// Application
serviceCollection.AddSingleton<IPlayerInputProvider, ConsoleInputService>();
serviceCollection.AddSingleton<IPlayerFactory, PlayerFactory>();
serviceCollection.AddSingleton<IGameDealerFactory, GameDealerFactory>();
serviceCollection.AddSingleton<ISimulator, Simulator>();
serviceCollection.AddSingleton<App>();
serviceCollection.AddTransient<IDeck>(sp => new Deck(3, 35)); // TODO: Add to rule object
serviceCollection.AddTransient<IGesjaaktGameState, GesjaaktGameState>();
serviceCollection.AddTransient<Func<IGesjaaktGameState>>(sp => () => sp.GetRequiredService<IGesjaaktGameState>());

var serviceProvider = serviceCollection.BuildServiceProvider();

// Get logger for the Program class
var logger = serviceProvider.GetRequiredService<Domain.Interfaces.ILogger<Program>>();
logger.LogInformation("Starting application...");

var app = serviceProvider.GetRequiredService<App>();
app.Run();

