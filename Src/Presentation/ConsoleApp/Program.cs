using Microsoft.Extensions.DependencyInjection;

using ConsoleApp;

 new ServiceCollection()
    .AddLoggingInfra()
    .AddGeneralServices()
    .AddDynamicBehaviourToChooseGame()
    .AddGesjaaktGame()
    .AddTakeFiveGame()
    .BuildServiceProvider()
    .GetRequiredService<App>()
    .Start();