using System;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Css
{
	internal class ComputedCssStyleDeclaration : ICssStyleDeclaration
	{
		private readonly Func<int> _getVersion;
		private CachedEnumerable<ICssStyleDeclaration> _styles;

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

		private int _cachedVersion;

		public ComputedCssStyleDeclaration(IElement elt, Func<int> getVersion)
		{
			_getVersion = getVersion;
			_styles = new CachedEnumerable<ICssStyleDeclaration>(GetStylesFor(elt));
			_cachedVersion = getVersion();
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
			var curVer = _getVersion();
			if (curVer != _cachedVersion)
			{
				_cachedVersion = curVer;
				_styles.Reset();
			}

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