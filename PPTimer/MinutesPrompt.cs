using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Platform;

namespace PPTimer;

/// <summary>Asks for a number of minutes, using the native dialog where one is available.</summary>
internal static class MinutesPrompt {
	public static Task<int?> Ask(string message, int current, WindowIcon icon) =>
		OperatingSystem.IsMacOS() ? askMac(message, current) : InputBox.Show("PPTimer", message, icon, current);

	// AppleScript's "display dialog" is a real AppKit alert, so it looks and behaves like any macOS dialog.
	private static readonly string[] script = [
		"on run argv",
		"  tell me to activate",
		"  set r to display dialog (item 1 of argv) default answer (item 2 of argv) with title \"PPTimer\" " +
		"buttons {\"Cancel\", \"Start\"} default button \"Start\" cancel button \"Cancel\" with icon POSIX file (item 3 of argv)",
		"  return text returned of r",
		"end run",
	];

	private static async Task<int?> askMac(string message, int current) {
		var answer = current.ToString();
		while (true) {
			var info = new ProcessStartInfo("osascript") { RedirectStandardOutput = true, RedirectStandardError = true };
			foreach (var line in script) {
				info.ArgumentList.Add("-e");
				info.ArgumentList.Add(line);
			}
			info.ArgumentList.Add(message);
			info.ArgumentList.Add(answer);
			info.ArgumentList.Add(iconPath());

			using var process = Process.Start(info)!;
			var output = await process.StandardOutput.ReadToEndAsync();
			await process.WaitForExitAsync();
			if (process.ExitCode != 0) return null; // Cancel (or osascript failed)

			answer = output.Trim();
			if (int.TryParse(answer, out var minutes) && minutes is >= 1 and <= 999) return minutes;
			message = "Please enter a number of minutes (1–999):";
		}
	}

	private static string? icon;

	// The dialog needs the icon as a real file, so copy the embedded one to temp once.
	private static string iconPath() {
		if (icon != null) return icon;
		icon = Path.Combine(Path.GetTempPath(), "pptimer-PPTimer.icns");
		using var source = AssetLoader.Open(new Uri("avares://PPTimer/Assets/PPTimer.icns"));
		using var target = File.Create(icon);
		source.CopyTo(target);
		return icon;
	}
}
