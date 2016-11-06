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

			foreach (var cssRule in elt.OwnerDocument.StyleSheets.SelectMany(x => x.CssRules).Reverse())
			{
				var mediaRule = cssRule as CssMediaRule;
				if (mediaRule != null)
				{
					var mediaQuery = mediaRule.Media.MediaText.Substring("media ".Length).Trim();
					if (elt.OwnerDocument.DefaultView.MatchMedia(mediaQuery).Matches)
					{
						foreach (var result in mediaRule.CssRules.OfType<CssStyleRule>().Reverse())
						{
							if (result.IsMatchesSelector(elt))
								yield return result.Style;
						}
					}
				}

				var styleRule = cssRule as CssStyleRule;

				if (styleRule != null && styleRule.IsMatchesSelector(elt))
					yield return styleRule.Style;
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

		string GetParentPropertyValue(string propertyName)
		{
			var doc = _elt.ParentNode as Document;
			if (doc != null)
			{
				if (propertyName == "font-size" || propertyName == "font-style" || propertyName == "font-family")
				{ 
					if (propertyName == "font-size")
						return "16px";
					else if (propertyName == "font-family")
						return "Times New Roman";
					else return "normal";
				}
				return null;
			}

			var parentElt = _elt.ParentNode as Element;
			if (parentElt != null)
				return parentElt.GetComputedStyle().GetPropertyValue(propertyName);
			return null;
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
				res = GetParentPropertyValue(propertyName);
			}

			if (propertyName == "font-size")
			{
				if (!string.IsNullOrEmpty(res))
				{
					var fontSize = FontSize(res);
					if (fontSize.Item2 == "em" || fontSize.Item2 == "%")
					{
						var parentFontSizeStr = GetParentPropertyValue("font-size");
						if (!string.IsNullOrEmpty(parentFontSizeStr))
						{
							var parentFontSize = FontSize(parentFontSizeStr);

							var ratio = fontSize.Item2.Length == 1 ? fontSize.Item1/100 : fontSize.Item1;

							return (parentFontSize.Item1*ratio).ToString(CultureInfo.InvariantCulture) + parentFontSize.Item2;
						}
					}
				}
			}

			if((propertyName == "font-size" || propertyName == "font-family" || propertyName == "font-style") && string.IsNullOrEmpty(res))
			{
				var parentElt = _elt.ParentNode as IElement;

				return parentElt != null ? parentElt.GetComputedStyle().GetPropertyValue(propertyName) : string.Empty;
			}

			return res;
		}

		Tuple<float, string> FontSize(string strValue)
		{
			var unitIdx = strValue.IndexOf(c => !char.IsDigit(c) && c != '.');
			if (unitIdx != 0)
			{
				var unit = string.Empty;
				if (unitIdx != -1)
				{
					unit = strValue.Substring(unitIdx);
					strValue = strValue.Substring(0, unitIdx);
					return new Tuple<float, string>(float.Parse(strValue, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture), unit.ToLowerInvariant());
				}
			}
			return new Tuple<float, string>(float.Parse(strValue), "");
		}

		public string GetPropertyPriority(string propertyName)
		{
			throw new NotImplementedException();
		}
	}
}