using Application;
using ConsoleApp;
using ConsoleApp.Helpers;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Domain.Interfaces.Games.Gesjaakt;
using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.BaseGame;
using Visualization;
using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;
using Application.Gesjaakt;
using Application.TakeFive;

var loglevel = LogLevel.Warning;

var bootstrapServices = new ServiceCollection();
bootstrapServices.AddLogging(config =>
{
    config.AddSimpleConsole(options =>
    {
        options.IncludeScopes = true;
    });
    // TODO: move to the EventListners classes, and depending on what we run, we choose what level we share (log)
    config.SetMinimumLevel(loglevel);
});

bootstrapServices.AddSingleton(typeof(Domain.Interfaces.ILogger<>), typeof(Infrastructure.Logging.Logger<>));

// Minimal services needed before game choice
bootstrapServices.AddSingleton<IPlayerInputProvider, ConsoleInputService>();

using var bootstrapProvider = bootstrapServices.BuildServiceProvider();

var input = bootstrapProvider.GetRequiredService<IPlayerInputProvider>();
var logger = bootstrapProvider.GetRequiredService<Domain.Interfaces.ILogger<Program>>();
// TODO: get input by playerInputProvider => move to app
logger.LogCritical("""
Which game do you want to play?
1. Gesjaakt
2. TakeFive
""");

int gameChoice = input.GetPlayerInputAsInt(new[] { 1, 2 });

//-------------------------------------------------


var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton(input);
serviceCollection.AddSingleton(typeof(Domain.Interfaces.ILogger<>), typeof(Infrastructure.Logging.Logger<>));

serviceCollection.AddLogging(config =>
{
    config.AddSimpleConsole(options =>
    {
        options.IncludeScopes = true;
    });
    config.SetMinimumLevel(loglevel);
});

// Application
switch (gameChoice)
{
    case 1:
        serviceCollection.AddSingleton<IGameRunner, GameRunner<IGesjaaktPlayer>>();
        serviceCollection.AddTransient<IGame<IGesjaaktPlayer>, GesjaaktGame>();

        serviceCollection.AddSingleton<IPlayerFactory<IGesjaaktPlayer>, GesjaaktPlayerFactory>();
        serviceCollection.AddTransient<IVisualizer, GesjaaktVisualizer>();
        serviceCollection.AddTransient<IGesjaaktGameDealer, GesjaaktGameDealer>();

        serviceCollection.AddTransient<GesjaaktGameEventOrchestrator>();

        break;

    case 2:
        // TODO: move game choosing to app (=> one serviceCollection)
        serviceCollection.AddSingleton<IGameRunner, GameRunner<ITakeFivePlayer>>();
        serviceCollection.AddTransient<IGame<ITakeFivePlayer>, TakeFiveGame>();

        serviceCollection.AddSingleton<IPlayerFactory<TakeFivePlayer>, TakeFivePlayerFactory>(); // Refactor - door deze generic is het eigenlijk uniek en hoeft het niet in een switch..

        break;
}

serviceCollection
    .AddTransient<App>()
    .BuildServiceProvider()
    .GetRequiredService<App>()
    .Start();