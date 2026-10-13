using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;

namespace PPTimer;

public class App : Application {
	private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(1) };
	private TrayIcon trayIcon = null!;
	private readonly NativeMenuItem statusItem = new() { IsEnabled = false };
	private int minutes = 10;
	private DateTime endsAt;

	private WindowIcon clockIcon = null!;
	private WindowIcon alertIcon = null!;
	private WindowIcon runningIcon = null!;

	public override void Initialize() {
		Styles.Add(new FluentTheme());
	}

	public override void OnFrameworkInitializationCompleted() {
		clockIcon = LoadIcon("clock.png");
		alertIcon = LoadIcon("alert.png");
		runningIcon = LoadIcon("running.png");

		timer.Tick += (s, e) => tickTimer();

		var menu = new NativeMenu();
		menu.Add(statusItem);
		menu.Add(new NativeMenuItemSeparator());
		menu.Add(MenuItem("Start timer", startTimer));
		menu.Add(MenuItem("Stop timer", stopTimer));
		menu.Add(MenuItem("Set time", () => {
			stopTimer();
			_ = setTime();
		}));
		menu.Add(new NativeMenuItemSeparator());
		menu.Add(MenuItem("Quit", () => ((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!).Shutdown()));

		trayIcon = new TrayIcon {
			Icon = clockIcon,
			ToolTipText = "Start me up!",
			Menu = menu,
			IsVisible = true,
		};
		TrayIcon.SetIcons(this, new TrayIcons { trayIcon });
		setStatus("Start me up!");

		base.OnFrameworkInitializationCompleted();
	}

	private static NativeMenuItem MenuItem(string header, Action action) {
		var item = new NativeMenuItem(header);
		item.Click += (s, e) => action();
		return item;
	}

	private static WindowIcon LoadIcon(string name) =>
		new(AssetLoader.Open(new Uri($"avares://PPTimer/Assets/{name}")));

	private async Task setTime() {
		var result = await InputBox.Show("Time", "Minutes", clockIcon, minutes);
		if (result is int value) {
			minutes = value;
			startTimer();
		} else {
			stopTimer();
		}
	}

	private void startTimer() {
		endsAt = DateTime.Now.AddMinutes(minutes);
		timer.Start();
		trayIcon.Icon = runningIcon;
		updateStatus();
	}

	private void stopTimer() {
		timer.Stop();
		trayIcon.Icon = clockIcon;
		setStatus("Idle");
	}

	private void updateStatus() {
		var left = endsAt - DateTime.Now;
		if (left < TimeSpan.Zero) left = TimeSpan.Zero;
		setStatus($"Tick tack... {(int)left.TotalMinutes}:{left.Seconds:00} left");
	}

	private void setStatus(string text) {
		statusItem.Header = text;
		trayIcon.ToolTipText = text;
	}

	private void tickTimer() {
		if (DateTime.Now < endsAt) {
			updateStatus();
			return;
		}
		timer.Stop();
		trayIcon.Icon = alertIcon;
		setStatus("Ding dong!");
		Sound.Play("gong.wav");
		_ = setTime();
	}
}
