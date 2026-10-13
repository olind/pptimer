using Avalonia;
using Avalonia.Controls;

namespace PPTimer;

internal static class Program {
	[STAThread]
	private static void Main(string[] args) {
		AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.With(new MacOSPlatformOptions { ShowInDock = false })
			.StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
	}
}
