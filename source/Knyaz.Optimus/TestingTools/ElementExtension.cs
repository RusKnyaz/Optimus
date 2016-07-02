using System;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;

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
			var selectors = selector.Split(' ');
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
					return elt.GetElementsByTagName(selector);
			}
		}
	}
}
