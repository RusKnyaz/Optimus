using System;
using System.Threading;
using WebBrowser.Dom;

namespace WebBrowser.Tools
{
	public static class EngineExtension
	{
		public static void WaitDocumentLoad(this Engine engine)
		{
			var signal = new ManualResetEvent(false);

			var handler = (Action<IDocument>)(document =>
			{
				signal.Set(); 
			});

			engine.Document.DomContentLoaded += handler;

			if (engine.Document.ReadyState != DocumentReadyStates.Complete)
			{
				signal.WaitOne();
			}

			engine.Document.DomContentLoaded -= handler;
		}
	}
}
