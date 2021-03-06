﻿using System.ComponentModel;
using System.Windows.Forms;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.WfApp.Controls;

namespace Knyaz.Optimus.WfApp
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
			Properties.Settings.Default.Save();
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
				timeLineControl.Engine = value;
			}
		}
	}
}
