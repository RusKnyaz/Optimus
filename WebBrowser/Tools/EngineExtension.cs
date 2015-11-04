using System;
using System.Threading;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;

namespace WebBrowser.Tools
{
	public static class EngineExtension
	{
		private const int DefaultTimeout = 20000;

		public static void WaitDocumentLoad(this Engine engine)
		{
			if(engine.Document.ReadyState == DocumentReadyStates.Complete)
				return;
			
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

		/// <summary>
		/// Wait while element with specified id appears in document.
		/// </summary>
		/// <param name="id">Id of element waiting for.</param>
		/// <returns>Found elemnt if exists</returns>
		public static Element WaitId(this Engine engine, string id, int timeout = DefaultTimeout)
		{
			engine.WaitDocumentLoad();
			var timespan = 100;
			for (int i = 0; i < timeout/timespan; i++)
			{
				var elt = engine.Document.GetElementById(id);
				if (elt != null)
					return elt;

				Thread.Sleep(timespan);
			}
			return engine.Document.GetElementById(id);
		}
	}
}
