using Application;
using ConsoleApp;
using ConsoleApp.Helpers;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// TODO: IOC
var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging(configure => configure.AddConsole()
    .SetMinimumLevel(LogLevel.Information)
);

//serviceCollection.AddSingleton<ILogger, Logger>();// Use custom loggers

// Application
serviceCollection.AddSingleton<IPlayerInputProvider, ConsoleInputService>();
serviceCollection.AddSingleton<IPlayerFactory, PlayerFactory>();
serviceCollection.AddSingleton<App>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting application...");

var app = serviceProvider.GetRequiredService<App>();
app.Run();

