using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Knyaz.Optimus.WfApp.Controls
{
	public partial class AddressBar : UserControl
	{
		private Engine _engine;

		public AddressBar()
		{
			InitializeComponent();
		}

		public Engine Engine
		{
			get { return _engine; }
			set
			{
				if (_engine != null)
				{
					_engine.OnUriChanged -= OnUriChanged;
				}
				_engine = value;
				if (_engine != null)
				{
					_engine.OnUriChanged += OnUriChanged;
				}
			}
		}

		private void OnUriChanged()
		{
			this.SafeInvoke(() => { textBoxUrl.Text = _engine.Uri.ToString();});
		}

		private async void textBoxUrl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				try
				{
					Cursor.Current = Cursors.WaitCursor;
					new Thread(() => Engine.OpenUrl(textBoxUrl.Text)).Start();
					Cursor.Current = DefaultCursor;
				}
				catch (Exception ex)
				{
					Cursor.Current = DefaultCursor;
					MessageBox.Show(ex.ToString());
				}
			}
		}
	}
}
