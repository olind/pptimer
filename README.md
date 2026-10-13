# Pair programming timer 

A pair programming timer that sits in the system tray / menu bar. Set a number of minutes, and when time is up it gongs and asks for the next round, so it's time to switch driver.

Originally a C# WinForms app developed 2009 (devblog: https://pptimer.wordpress.com/). Ported in 2026 to .NET 10 + [Avalonia](https://avaloniaui.net/), so it now runs on macOS, Windows and Linux.

## Running

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```sh
dotnet run --project PPTimer
```

Look for the clock icon in the menu bar (macOS) or system tray (Windows/Linux) and use its menu: **Start timer**, **Stop timer**, **Set time**, **Quit**. Hover the icon to see how much time is left.

To build a standalone app, e.g. for Apple Silicon Macs:

```sh
dotnet publish PPTimer -c Release -r osx-arm64 --self-contained
```

(Use `win-x64`, `linux-x64`, etc. for other platforms.)

Sound is played with the OS's own player: `PlaySound` on Windows, `afplay` on macOS, `paplay`/`aplay` on Linux. Tray icons on Linux need a desktop with StatusNotifier/AppIndicator support.

## Screenshots (original 2009 version)
![PPTimer screenshots](pptimer-screenshots.png "Pair programming timer screenshots")
