using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Knyaz.Optimus.Dom.Css;
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

		/// <summary>
		/// Emulate entering text by user into input textbox.
		/// </summary>
		public static void EnterText(this HtmlTextAreaElement input, string text)
		{
			input.Value = text;
			var evt = input.OwnerDocument.CreateEvent("Event");
			evt.InitEvent("change", false, false);
			input.DispatchEvent(evt);
		}

		static Regex _gtNormalize = new Regex("\\s*>\\s*", RegexOptions.Compiled);

		public static IEnumerable<IElement> Select(this IElement doc, string selector)
		{
			var selectors = _gtNormalize.Replace(selector, " >").Split(' ', '[').Where(x => !string.IsNullOrEmpty(x)).ToArray();
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
					var id = selector.Substring(1);
					return elt.ChildNodes.OfType<IElement>().Flat(x => x.ChildNodes.OfType<IElement>()).Where(x => x.Id == id);
				case '.':
					return elt.GetElementsByClassName(selector.Substring(1));
				case '>':
					var sel = new CssSelector(selector.Substring(1));
					return elt.ChildNodes.OfType<IElement>().Where(x => sel.IsMatches(x));
				default:
					if (selector[selector.Length - 1] == ']')
						return elt.GetElementsByAttributes(selector.Substring(0, selector.Length-1));

					return elt.GetElementsByTagName(selector);
			}
		}

		private static IEnumerable<IElement> GetElementsByAttributes(this IElement elt, string selector)
		{
			foreach (var child in elt.Flatten().OfType<IElement>())
			{
				var arr = selector.Split('=');
				if (arr[0].Last() == '^')
				{
					var attrVal = child.GetAttribute(arr[0].TrimEnd('^'));
					if (attrVal != null && attrVal.StartsWith(arr[1].Trim('\'')))
						yield return child;
				}
				else
				{
					if (arr.Length == 1)
					{
						if (child.GetAttributeNode(arr[0]) != null)
							yield return child;
					}
					else if (child.GetAttribute(arr[0]) == arr[1].Trim('\''))
						yield return child;	
				}
			}
		}

		public static ICssStyleDeclaration GetComputedStyle(this IElement elt)
		{
			return elt.OwnerDocument.DefaultView.GetComputedStyle(elt);
		}
	}
}
