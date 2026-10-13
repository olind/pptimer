using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Platform;

namespace PPTimer;

/// <summary>Plays a bundled WAV file using whatever the OS provides, no audio packages needed.</summary>
internal static partial class Sound {
	private static readonly Dictionary<string, string> extracted = new();

	public static void Play(string asset) {
		try {
			var path = extract(asset);
			if (OperatingSystem.IsWindows()) {
				PlaySound(path, IntPtr.Zero, SND_FILENAME | SND_ASYNC);
			} else if (OperatingSystem.IsMacOS()) {
				run("afplay", path);
			} else if (!run("paplay", path)) {
				run("aplay", "-q", path);
			}
		} catch {
			// No sound is better than a crash; the tray icon still signals the alert.
		}
	}

	// Players need a real file, so copy the embedded asset to temp once.
	private static string extract(string asset) {
		if (extracted.TryGetValue(asset, out var path)) return path;
		path = Path.Combine(Path.GetTempPath(), $"pptimer-{asset}");
		using (var source = AssetLoader.Open(new Uri($"avares://PPTimer/Assets/{asset}")))
		using (var target = File.Create(path)) {
			source.CopyTo(target);
		}
		extracted[asset] = path;
		return path;
	}

	private static bool run(string command, params string[] args) {
		try {
			var info = new ProcessStartInfo(command) { UseShellExecute = false };
			foreach (var arg in args) info.ArgumentList.Add(arg);
			Process.Start(info);
			return true;
		} catch {
			return false;
		}
	}

	private const uint SND_ASYNC = 0x0001;
	private const uint SND_FILENAME = 0x00020000;

	[LibraryImport("winmm.dll", EntryPoint = "PlaySoundW", StringMarshalling = StringMarshalling.Utf16)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static partial bool PlaySound(string sound, IntPtr module, uint flags);
}
