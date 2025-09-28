using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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