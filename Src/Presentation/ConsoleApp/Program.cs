using Microsoft.Extensions.DependencyInjection;

using ConsoleApp;

 new ServiceCollection()
    .AddLoggingInfra()
    .AddGeneralServices()
    .AddGesjaaktGame()
    .AddTakeFiveGame()
    .AddDynamicBehaviourToChooseGame()
    .AddWinFormsVisualization()
    .BuildServiceProvider()
    .GetRequiredService<App>()
    .Start();