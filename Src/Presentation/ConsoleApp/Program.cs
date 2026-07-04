using Microsoft.Extensions.DependencyInjection;

using ConsoleApp;

// Some console hosts (e.g. Visual Studio's F5 debug console) don't support the
// cursor movement the live status display relies on. Pass --simple-console to
// fall back to plain sequential status lines instead.
bool useLiveDisplay = !args.Contains("--simple-console", StringComparer.OrdinalIgnoreCase);

new ServiceCollection()
    .AddLoggingInfra()
    .AddGeneralServices()
    .AddGesjaaktGame()
    .AddTakeFiveGame()
    .AddDynamicBehaviourToChooseGame()
    .AddConsoleVisualization(useLiveDisplay)
    .BuildServiceProvider()
    .GetRequiredService<App>()
    .Start();