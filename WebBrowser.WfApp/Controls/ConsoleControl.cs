using System;
using System.Windows.Forms;

namespace WebBrowser.WfApp.Controls
{
	public partial class ConsoleControl : UserControl
	{
		private Engine _engine;

		public ConsoleControl()
		{
			InitializeComponent();
		}

		public Engine Engine
		{
			get
			{
				return _engine;
			}
			set
			{
				if (_engine != null)
				{
					_engine.Console.OnLog -= ConsoleOnOnLog;
					_engine.ResourceProvider.OnRequest += ResourceProviderOnOnRequest;
				}

				_engine = value;

				if (_engine != null)
				{
					_engine.Console.OnLog += ConsoleOnOnLog;
					_engine.ResourceProvider.OnRequest  += ResourceProviderOnOnRequest;
				}
			}
		}

		private void ResourceProviderOnOnRequest(string s)
		{
			this.SafeInvoke(() => textBoxLog.Text += "Request: " + s + "\r\n");
		}

		private void ConsoleOnOnLog(object o)
		{
			this.SafeInvoke(() => textBoxLog.Text += o.ToString() + "\r\n");
		}
	}

	public static class ControlExtension
	{
		public static void SafeInvoke(this Control obj, Action action)
		{
			if (obj.InvokeRequired)
			{
				var args = new object[0];
				obj.BeginInvoke(action, args);
			}
			else
			{
				action();
			}
		}
	}
}
