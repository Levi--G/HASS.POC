using Avalonia;
using HASS.Extensions;
using ReactiveUI.Avalonia;
using System;
using System.Linq;

namespace HASS.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Contains("-c", StringComparison.OrdinalIgnoreCase) || args.Contains("--console", StringComparison.OrdinalIgnoreCase))
        {
            //start console mode
            var app = new HaDesktopApp();
            app.Startup();
            //TODO
        }
        else
        {
            BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
