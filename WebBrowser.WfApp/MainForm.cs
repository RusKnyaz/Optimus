using System;
using System.ComponentModel;
using System.Windows.Forms;
using WebBrowser.Dom.Elements;
using WebBrowser.WfApp.Controls;

namespace WebBrowser.WfApp
{
	public partial class MainForm : Form
	{
		private Engine _engine;

		public MainForm()
		{
			InitializeComponent();

			Engine = new Engine();
			domTreeControl.NodeSelected +=DomTreeControlOnNodeSelected;
		}

		private void DomTreeControlOnNodeSelected(TreeNode treeNode)
		{
			var element = treeNode.Tag as Element;
				this.SafeBeginInvoke(() =>
					{
						textBox1.Text = element != null ? element.InnerHTML : string.Empty;
					});
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			Engine.Dispose();
			base.OnClosing(e);
		}

		private Engine Engine
		{
			get { return _engine; }
			set
			{
				_engine = value;
				consoleControl.Engine = value;
				domTreeControl.Engine = value;
				addressBar.Engine = value;
			}
		}
	}
}
