using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Css
{
	internal class ComputedCssStyleDeclaration : ICssStyleDeclaration
	{
		private readonly IElement _elt;
		private readonly Func<int> _getVersion;
		private CachedEnumerable<ICssStyleDeclaration> _styles;

		private static IEnumerable<ICssStyleDeclaration> GetStylesFor(IElement elt)
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
			_elt = elt;
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

			var values = 
				_styles.Where(x => x.GetPropertyPriority(propertyName) == "important")
				.Concat(
				_styles.Where(x => x.GetPropertyPriority(propertyName) == string.Empty));

			var res = values.Select(x => x.GetPropertyValue(propertyName)).FirstOrDefault(x => x != null);
			if(res == "inherit")
			{
				var parentElt = _elt.ParentNode as IElement;
				if (parentElt != null)
					res = parentElt.GetComputedStyle().GetPropertyValue(propertyName);
			}

			if (propertyName == "font-size" && res != null && res.Length > 2 && res.Substring(res.Length - 2).ToLowerInvariant() == "em")
			{
				var parentElt = _elt.ParentNode as IElement;
				if (parentElt != null)
				{
					var valStr = res.Substring(0, res.Length - 2).Trim();
					float val;
					if (float.TryParse(valStr, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out val))
					{
						var parentFontSize = parentElt.GetComputedStyle().GetPropertyValue("font-size");

						var unitIdx = parentFontSize.IndexOf(c => !char.IsDigit(c) && c != '.');
						if (unitIdx != 0)
						{
							var unit = string.Empty;
							var parentValStr = parentFontSize;
							if (unitIdx != -1)
							{
								unit = parentFontSize.Substring(unitIdx);
								parentValStr = parentFontSize.Substring(0, unitIdx);
								float parentVal;
								if (float.TryParse(parentValStr, out parentVal))
								{
									return (parentVal*val).ToString(CultureInfo.InvariantCulture) + unit;
								}
							}
						}
					}
				}
			}

			return res;
		}

		public string GetPropertyPriority(string propertyName)
		{
			throw new NotImplementedException();
		}
	}
}