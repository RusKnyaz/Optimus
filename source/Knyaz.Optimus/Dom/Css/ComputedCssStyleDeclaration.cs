using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.Dom.Css
{
	internal class ComputedCssStyleDeclaration : ICssStyleDeclaration
	{
		private readonly IElement _elt;

		IEnumerable<ICssStyleDeclaration> _styles
		{
			get
			{
				return GetStylesFor(_elt);
			}
		}

		private IEnumerable<ICssStyleDeclaration> GetStylesFor(IElement elt)
		{
			var htmlElt = elt as HtmlElement;
			if (htmlElt != null)
				yield return htmlElt.Style;

			//todo: it would be better to have reversed list, or acces it by index;
			//todo(2): what about safe enumeration
			foreach (var result in elt.OwnerDocument.StyleSheets.SelectMany(x => x.CssRules).OfType<CssStyleRule>().Reverse())
			{
				if (result.IsMatchesSelector(elt))
					yield return result.Style;
			}
		}

		public ComputedCssStyleDeclaration(IElement elt)
		{
			_elt = elt;
		}

		public object this[string name]
		{
			get
			{
				int number;
				return int.TryParse(name, out number) ? this[number] : GetPropertyValue(name);
			}
		}

		public string this[int idx]
		{
			get { throw new NotImplementedException(); }
		}

		public string GetPropertyValue(string propertyName)
		{
			foreach (var style in _styles)
			{
				var val = style.GetPropertyValue(propertyName);
				if (val != null)
					return val;
			}
			return null;
		}
	}
}