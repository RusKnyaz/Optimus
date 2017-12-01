using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Tools;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.TestingTools
{
	public static class EngineExtension
	{
		public static int DefaultTimeout = 20000;

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

		public static Element WaitId(this Engine engine , string id)
		{
			return WaitId(engine, id, DefaultTimeout);
		}

		/// <summary>
		/// Wait while element with specified id appears in document.
		/// </summary>
		/// <param name="id">Id of element waiting for.</param>
		/// <returns>Found elemnt if exists</returns>
		public static Element WaitId(this Engine engine, string id, int timeout)
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
		/// Locks the current thread until the element with specified id disappears.
		/// </summary>
		/// <param name="engine">Document owner.</param>
		/// <param name="id">Identifier of the item to be disappeared.</param>
		/// <param name="timeout">The timeout</param>
		/// <returns>Element if found, <c>null</c> othervise.</returns>
		public static Element WaitDesappearingOfId(this Engine engine, string id)
		{
			return WaitDesappearingOfId(engine, id, DefaultTimeout);
		}

		/// <summary>
		/// Locks the current thread until the element with specified id disappears.
		/// </summary>
		/// <param name="engine">Document owner.</param>
		/// <param name="id">Identifier of the item to be disappeared.</param>
		/// <param name="timeout">The timeout</param>
		/// <returns>Element if found, <c>null</c> othervise.</returns>
		public static Element WaitDesappearingOfId(this Engine engine, string id, int timeout)
		{
			var timespan = 100;
			for (int i = 0; i < timeout / timespan; i++)
			{
				var doc = engine.Document;
				lock (doc)
				{
					var elt = doc.GetElementById(id);
					if (elt == null)
						return null;
				}

				Thread.Sleep(timespan);
			}
			return engine.Document.GetElementById(id);
		}

		/// <summary>
		/// Wait while element with specified id appears in document.
		/// </summary>
		public static Element WaitId(this Document document, string id)
		{
			return WaitId(document, id, DefaultTimeout);
		}

		/// <summary>
		/// Wait while element with specified id appears in document.
		/// </summary>
		public static Element WaitId(this Document document, string id, int timeout)
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


		public static IElement First(this Engine engine, string query)
		{
			return engine.Document.QuerySelectorAll(query).First();
		}

		public static HtmlElement FirstElement(this Engine engine, string query)
		{
			return engine.Document.QuerySelectorAll(query).OfType<HtmlElement>().First();
		}

		public static IEnumerable<IElement> WaitSelector(this Engine engine, string query)
		{
			return WaitSelector(engine, query, DefaultTimeout);
		}

		public static IEnumerable<IElement> WaitSelector(this Engine engine, string query, int timeout)
		{
			engine.WaitDocumentLoad();
			var selector = new CssSelector(query);
            var timespan = 100;
			var doc = engine.Document;

			for (int i = 0; i < timeout / timespan; i++)
			{
				try
				{
					var elt = selector.Select(doc).ToListOrNull();
					if (elt != null)
						return elt;
				}
				catch
				{

				}

				Thread.Sleep(timespan);
			}
			return selector.Select(doc);
		}

		public static void DumpToFile(this Engine engine, string fileName)
		{
			var data = engine.Document.InnerHTML;
			using (var stream = File.CreateText(fileName))
			{
				stream.Write(data);
			}
		}
	}
}
