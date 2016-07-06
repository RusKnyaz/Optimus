using System;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.TestingTools
{
	public static class ElementExtension
	{
		/// <summary>
		/// Emulate entering text by user into input textbox.
		/// </summary>
		public static void EnterText(this HtmlInputElement input, string text)
		{
			input.Value = text;
			var evt = input.OwnerDocument.CreateEvent("Event");
			evt.InitEvent("change", false, false);
			input.DispatchEvent(evt);
		}

		public static IEnumerable<IElement> Select(this IElement doc, string selector)
		{
			var selectors = selector.Split(' ', '[').Where(x => !string.IsNullOrEmpty(x)).ToArray();
			if (selectors.Length == 0)
				return Enumerable.Empty<IElement>();

			return selectors.Aggregate((IEnumerable<IElement>)new[] { doc }, (current, s) => current.SelectMany(x => x.SelectByOneSelector(s)));
		}

		private static IEnumerable<IElement> SelectByOneSelector(this IElement elt, string selector)
		{
			var firstSymbol = selector[0];
			switch (firstSymbol)
			{
				case '#':
				{
					var doc = elt as IDocument;
					if (doc != null)
					{
						var res = doc.GetElementById(selector.Substring(1));
						if (res != null)
							return new[] {res};
						
						return new IElement[0];
					}
						
					throw new InvalidOperationException("Id search can be performed only on document");
				}
				case '.':
					return elt.GetElementsByClassName(selector.Substring(1));
				default:
					if (selector[selector.Length - 1] == ']')
						return elt.GetElementsByAttributes(selector.Substring(0, selector.Length-1));

					return elt.GetElementsByTagName(selector);
			}
		}

		private static IEnumerable<IElement> GetElementsByAttributes(this IElement elt, string selector)
		{
			var attrs = selector.Split(',');
			foreach (var child in elt.ChildNodes.Flat(x => x.ChildNodes).OfType<IElement>())
			{
				var notMatched = false;
				foreach (var attr in attrs)
				{
					var arr = attr.Split('=');
					notMatched = child.GetAttribute(arr[0]) != arr[1];
				}

				if (!notMatched)
					yield return child;
			}
		}
	}
}
