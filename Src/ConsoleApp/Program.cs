using Application;
using ConsoleApp;
using ConsoleApp.Helpers;
using Domain.Entities.Cards;
using Domain.Entities.Game;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

serviceCollection.AddSingleton(typeof(Domain.Interfaces.ILogger<>), typeof(ConsoleApp.Logging.Logger<>));


// Application
serviceCollection.AddSingleton<IPlayerInputProvider, ConsoleInputService>();
serviceCollection.AddSingleton<IPlayerFactory, PlayerFactory>();
serviceCollection.AddSingleton<IGameDealerFactory, GameDealerFactory>();
serviceCollection.AddSingleton<App>();
serviceCollection.AddTransient<IDeck>(sp => new Deck(3, 35));
serviceCollection.AddTransient<IGameState, GameState>();
serviceCollection.AddTransient<Func<IGameState>>(sp => () => sp.GetRequiredService<IGameState>());

var serviceProvider = serviceCollection.BuildServiceProvider();

// Get logger for the Program class
var logger = serviceProvider.GetRequiredService<Domain.Interfaces.ILogger<Program>>();
logger.LogInformation("Starting application...");

var app = serviceProvider.GetRequiredService<App>();
app.Run();

