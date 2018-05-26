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
using Knyaz.Optimus.Environment;

namespace Knyaz.Optimus.TestingTools
{
	/// <summary>
	/// Contains helper methods for testing with optimus Engine.
	/// </summary>
	public static class EngineExtension
	{
		/// <summary>
		/// Default timeout for 'wait*' methods in milliseconds.
		/// </summary>
		public static int DefaultTimeout = 20000;

		/// <summary>
		/// Wait for the loading of document.
		/// </summary>
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
		/// Blocks the execution of the current thread until an item with the specified ID appears in the document.
		/// </summary>
		/// <param name="engine">The engine with the document to wait in.</param>
		/// <param name="id">The identifier to be awaited.</param>
		/// <returns>Element with specified Id, <c>null</c> if the element with the specified identifier has not appeared in the document for the default timeout.</returns>
		public static Element WaitId(this Engine engine , string id)
		{
			return WaitId(engine, id, DefaultTimeout);
		}

		/// <summary>
		/// Blocks the execution of the current thread until an item with the specified ID appears in the document.
		/// </summary>
		/// <param name="engine">Document onwer.</param>
		/// <param name="id">Id of element waiting for.</param>
		/// <param name="timeout">The time to wait in milliseconds</param>
		/// <returns>Element with specified Id, <c>null</c> if the element with the specified identifier has not appeared in the document for a given time.</returns>
		public static Element WaitId(this Engine engine, string id, int timeout)
		{
			engine.WaitDocumentLoad();
			var timespan = 100;
			for (int i = 0; i < timeout / timespan; i++)
			{
				var doc = engine.Document;
				lock (doc)
				{
					try
					{
						var elt = doc.GetElementById(id);
						if (elt != null)
							return elt;
					}
					catch 
					{
						//catch 'collection was changed...'
					}
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

		/// <summary>
		/// Search the first html element in document which satisfies specified selector.
		/// </summary>
		/// <param name="engine">Document owner.</param>
		/// <param name="query">Css selector.</param>
		/// <returns>Found <see cref="HtmlElement"/> or <c>null</c>.</returns>
		public static HtmlElement FirstElement(this Engine engine, string query)
		{
			return engine.Document.QuerySelectorAll(query).OfType<HtmlElement>().First();
		}

		/// <summary>
		/// Waits at least one element which satisfies to a given query selector.
		/// </summary>
		/// <param name="engine"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IEnumerable<IElement> WaitSelector(this Engine engine, string query)
		{
			return WaitSelector(engine, query, DefaultTimeout);
		}

		/// <summary>
		/// Blocks the execution of the current thread until an at least one item which satisfies specified selector appears in the document.
		/// </summary>
		/// <param name="engine">Document owner.</param>
		/// <param name="query">Css selector.</param>
		/// <param name="timeout">Time to wait in milliseconds.</param>
		/// <returns>Collection of found elements.</returns>
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

		/// <summary>
		/// Dumps the entire documents html to the file.
		/// </summary>
		/// <param name="engine">Document owner.</param>
		/// <param name="fileName">Target file name.</param>
		public static void DumpToFile(this Engine engine, string fileName)
		{
			var data = engine.Document.InnerHTML;
			using (var stream = File.CreateText(fileName))
			{
				stream.Write(data);
			}
		}

		/// <summary>
		/// Alias for QuerySelectorAll method with element types filtration.
		/// </summary>
		public static IEnumerable<T> Get<T>(this Element elt, string query)
		{
			return elt.QuerySelectorAll(query).OfType<T>();
		}

		/// <summary>
		/// Setup user agent name to be one from the know list.
		/// </summary>
		public static Engine UseKnownUserAgent(this Engine engine, KnownUserAgents agent)
		{
			var ua = agent == KnownUserAgents.Linux_Chromium ? "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Ubuntu Chromium/66.0.3359.139 Chrome/66.0.3359.139 Safari/537.36"
				: agent == KnownUserAgents.Windows_Firefox ? "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0"
				: agent == KnownUserAgents.IOs_Firefox ? "Mozilla/5.0 (Macintosh; Intel Mac OS X x.y; rv:42.0) Gecko/20100101 Firefox/42.0"
				: agent == KnownUserAgents.Andrpoid_Chrome ? "Mozilla/5.0 (Linux; Android 7.0; SM-G930V Build/NRD90M) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.125 Mobile Safari/537.36"
				: throw new ArgumentException("agent");

			return engine.UseCustomUserAgent(ua);
		}

		/// <summary>
		/// Setup user agent name value that avaliable in js (navigator.userAgent) and sent within http request headers.
		/// </summary>
		public static Engine UseCustomUserAgent(this Engine engine, string userAgent)
		{
			((Navigator) engine.Window.Navigator).UserAgent = userAgent;
			return engine;
		}
	}

	public enum KnownUserAgents
	{
		Linux_Chromium,
		Windows_Firefox,
		IOs_Firefox,
		Andrpoid_Chrome
	}
}
