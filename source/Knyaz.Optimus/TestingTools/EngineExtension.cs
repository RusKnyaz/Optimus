using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.TestingTools
{
	public static class EngineExtension
	{
		private const int DefaultTimeout = 20000;

		public static void WaitDocumentLoad(this Engine engine)
		{
			if(engine.Document.ReadyState != DocumentReadyStates.Loading)
				return;
			
			var signal = new ManualResetEventSlim(false);

			var handler = (Action<IDocument>)(document =>
			{
				signal.Set(); 
			});

			engine.Document.DomContentLoaded += handler;

			if (engine.Document.ReadyState == DocumentReadyStates.Loading)
			{
				signal.Wait(DefaultTimeout);
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
			for (int i = 0; i < timeout / timespan; i++)
			{
				var doc = engine.Document;
				lock (doc)
				{
					var elt = doc.GetElementById(id);
					if (elt != null)
						return elt;
				}

				Thread.Sleep(timespan);
			}
			return engine.Document.GetElementById(id);
		}

		/// <summary>
		/// Wait while element with specified id appears in document.
		/// </summary>
		public static Element WaitId(this Document document, string id, int timeout = DefaultTimeout)
		{
			var timespan = 100;
			for (int i = 0; i < timeout / timespan; i++)
			{
				var elt = document.GetElementById(id);
				if (elt != null)
					return elt;

				Thread.Sleep(timespan);
			}
			return document.GetElementById(id);
		}


		public static IEnumerable<IElement> Select(this Engine engine, string query)
		{
			return engine.Document.Select(query);
		}

		public static IElement First(this Engine engine, string query)
		{
			return engine.Select(query).First();
		}

		public static HtmlElement FirstElement(this Engine engine, string query)
		{
			return engine.Select(query).OfType<HtmlElement>().First();
		}

		public static IEnumerable<IElement> WaitSelector(this Engine engine, string query, int timeout = DefaultTimeout)
		{
			engine.WaitDocumentLoad();
            			var timespan = 100;
			for (int i = 0; i < timeout / timespan; i++)
			{
				var doc = engine.Document;
				lock (doc)
				{
					var elt = doc.Select(query).ToList();
					if (elt.Any())
						return elt;
				}

				Thread.Sleep(timespan);
			}
			return engine.Select(query);
		}
	}
}
