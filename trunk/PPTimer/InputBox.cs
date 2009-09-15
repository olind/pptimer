using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PPTimer {
	public partial class InputBox : Form {
		public InputBox() {
			InitializeComponent();
		}

		public static DialogResult Show(string title, string prompt, Icon icon, ref string inputValue) {
			var form = new InputBox();
			form.Icon = icon;
			form.CenterToScreen();
			form.Text = title;
			form.label1.Text = prompt;
			form.textBox1.Text = inputValue;
			var result = form.ShowDialog();
			inputValue = form.textBox1.Text;
			return result;
		}
	}
}