using System;
using System.Collections.Generic;
using System.Linq;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;

namespace WebBrowser.Tools
{
	public static class EngineQueryExtension
	{
		public static IEnumerable<Node> Query(this Engine engine, string query)
		{
			return engine.Document.Query(query);
		}

		public static IEnumerable<Node> Query(this Document document, string query)
		{
			var terms = query.Split(' ', '>');
			if (terms.Length == 1)
			{
				if (terms[0].StartsWith("#"))
					return new[]{document.GetElementById(terms[0].Remove(0, 1))};
			}

			throw new NotImplementedException();
		}

		public static Node First(this Engine engine, string query)
		{
			return engine.Query(query).First();
		}

		public static HtmlElement FirstElement(this Engine engine, string query)
		{
			return engine.Query(query).OfType<HtmlElement>().First();
		}
	}
}
