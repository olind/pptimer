using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PPTimer {
	internal static class Program {
		private static Timer timer;
		private static NotifyIcon trayIcon;
		private static ContextMenu menu;
		private static int minutes = 10;

		private static Icon clockIcon;
		private static Icon alertIcon;
		private static Icon runningIcon;

		[STAThread]
		private static void Main() {
			try {
				clockIcon = new Icon("Clock.ico");
				alertIcon = new Icon("Alert.ico");
				runningIcon = new Icon("Running.ico");

				timer = new Timer();
				timer.Tick += (s, e) => tickTimer();

				menu = new ContextMenu();
				menu.MenuItems.Add(new MenuItem("Start timer", (s, e) => startTimer()));
				menu.MenuItems.Add(new MenuItem("Stop timer", (s, e) => stopTimer()));
				menu.MenuItems.Add(new MenuItem("Set time", (s, e) =>
				{
					stopTimer();
					setTime();
				}));
				menu.MenuItems.Add("-", (s, e) => { });
				menu.MenuItems.Add(new MenuItem("Quit", (s, e) => Application.Exit()));

				trayIcon = new NotifyIcon();
				trayIcon.Icon = clockIcon;
				trayIcon.Text = "Start me up!";
				trayIcon.Visible = true;
				trayIcon.ContextMenu = menu;

				Application.Run();
			} catch (Exception ex) {
				MessageBox.Show("Ooops!\n" + ex);
			} finally {
				if (timer != null) {
					timer.Stop();
				}
				if (trayIcon != null) {
					trayIcon.Visible = false;
				}
			}
		}

		private static void setTime() {
			var minutesInput = minutes.ToString();
			var result = InputBox.Show("Time", "Minutes", clockIcon, ref minutesInput);
			if (result == DialogResult.OK) {
				minutes = int.Parse(minutesInput);
				startTimer();
			} else {
				stopTimer();
			}
		}

		private static void startTimer() {
			timer.Interval = minutes * 60 * 1000;
			timer.Start();
			trayIcon.Icon = runningIcon;
			trayIcon.Text = string.Format("Tick tack tick tack ({0}ms)...", timer.Interval);
		}

		private static void stopTimer() {
			timer.Stop();
			trayIcon.Icon = clockIcon;
			trayIcon.Text = "Idle";
		}

		private static void tickTimer() {
			timer.Stop();
			trayIcon.Icon = alertIcon;
			trayIcon.Text = "Ding dong!";
			var player = new System.Media.SoundPlayer();
			player.SoundLocation = "gong-chinese.wav";
			player.Play();
			setTime();
		}
	}
}