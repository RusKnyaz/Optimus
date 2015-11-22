using System;
using System.Threading;
using System.Windows.Forms;

namespace WebBrowser.WfApp.Controls
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
			this.SafeInvoke(() =>
			{
				textBoxUrl.Text = _engine.Uri.ToString();
			});
		}

		private void textBoxUrl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				new Thread(() =>
					{
						try
						{
							Engine.OpenUrl(textBoxUrl.Text);
						}
						catch (Exception ex)
						{
							MessageBox.Show(ex.ToString());
						}
					}
				 ).Start();

			}
		}
	}
}
