using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;

namespace PPTimer;

public class InputBox : Window {
	private readonly NumericUpDown input;
	private readonly TaskCompletionSource<int?> result = new();

	private InputBox(string title, string prompt, WindowIcon icon, int value) {
		Title = title;
		Icon = icon;
		Topmost = true;
		CanResize = false;
		SizeToContent = SizeToContent.WidthAndHeight;
		WindowStartupLocation = WindowStartupLocation.CenterScreen;

		input = new NumericUpDown {
			Value = value,
			Minimum = 1,
			Maximum = 999,
			Increment = 1,
			FormatString = "0",
			HorizontalAlignment = HorizontalAlignment.Stretch,
		};
		var ok = new Button { Content = "Start", IsDefault = true, Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
		var cancel = new Button { Content = "Cancel", IsCancel = true, Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
		ok.Click += (s, e) => finish((int?)input.Value);
		cancel.Click += (s, e) => finish(null);
		Closed += (s, e) => result.TrySetResult(null);

		Content = new StackPanel {
			Margin = new Thickness(16),
			Spacing = 12,
			Children = {
				new TextBlock { Text = prompt },
				input,
				new StackPanel {
					Orientation = Orientation.Horizontal,
					HorizontalAlignment = HorizontalAlignment.Right,
					Spacing = 8,
					Children = { cancel, ok },
				},
			},
		};

		Opened += (s, e) => {
			Activate();
			input.Focus(NavigationMethod.Tab);
		};
	}

	private void finish(int? value) {
		result.TrySetResult(value);
		Close();
	}

	/// <summary>Shows the dialog and returns the entered minutes, or null if cancelled.</summary>
	public static Task<int?> Show(string title, string prompt, WindowIcon icon, int value) {
		var form = new InputBox(title, prompt, icon, value);
		form.Show();
		return form.result.Task;
	}
}
