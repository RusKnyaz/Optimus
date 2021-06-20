using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.Tools;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Dom.Css
{
	internal class ComputedCssStyleDeclaration : ICssStyleDeclaration
	{
		private readonly CssStyleSheet _defaultStyleSheet;
		private readonly IElement _elt;
		private readonly Func<int> _getVersion;
		private CachedEnumerable<ICssStyleDeclaration> _styles;

		private IEnumerable<ICssStyleDeclaration> GetStylesFor(IElement elt)
		{
			if (elt is HtmlElement htmlElt)
				yield return htmlElt.Style;

			foreach (var rule in GetStyleRulesFor(elt, elt.OwnerDocument.StyleSheets)
			         .SelectMany(x => x.Selectors.Select(sel => Tuple.Create(sel, x)))
					 .OrderByDescending(tuple => tuple.Item1.Specificity)
					 .Select(tuple => tuple.Item2))
				yield return rule.Style;
			
			//default styles is last
			if (elt is HtmlElement && _defaultStyleSheet != null)
				foreach (var rule in GetStyleRulesFor(elt, Enumerable.Repeat(_defaultStyleSheet,1))
					.SelectMany(x => x.Selectors.Select(sel => Tuple.Create(sel, x)))
					.OrderByDescending(tuple => tuple.Item1.Specificity)
					.Select(tuple => tuple.Item2))
					yield return rule.Style;
		}

		private static IEnumerable<CssStyleRule> GetStyleRulesFor(IElement elt, IEnumerable<CssStyleSheet> styleSheets)
		{
			//todo: it would be better to have reversed list, or access it by index;
			//todo(2): what about safe enumeration

			foreach (var cssRule in styleSheets.SelectMany(x => x.CssRules).Reverse())
			{
				if (cssRule is CssMediaRule mediaRule)
				{
					var mediaQuery = mediaRule.Media.MediaText.Substring("media ".Length).Trim();
					if (elt.OwnerDocument.DefaultView.MatchMedia(mediaQuery).Matches)
					{
						foreach (var result in mediaRule.CssRules.OfType<CssStyleRule>().Reverse())
						{
							if (result.IsMatchesSelector(elt))
								yield return result;
						}
					}
				}

				if (cssRule is CssStyleRule styleRule && styleRule.IsMatchesSelector(elt))
					yield return styleRule;
			}
		}

		private int _cachedVersion;

		internal ComputedCssStyleDeclaration(CssStyleSheet defaultStyleSheet, IElement elt, Func<int> getVersion)
		{
			_defaultStyleSheet = defaultStyleSheet;
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
			var doc = _elt.ParentNode as HtmlDocument;
			if (doc != null)
			{
				if (propertyName == Css.FontSize|| propertyName == Css.FontStyle || propertyName == Css.FontFamily)
				{ 
					if (propertyName == Css.FontSize)
						return "16px";
					else if (propertyName == Css.FontFamily)
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
			if(res == "inherit" ||
				((propertyName == Css.FontSize || propertyName == Css.FontFamily || propertyName == Css.FontStyle
				|| propertyName == "color") && string.IsNullOrEmpty(res)))
			{
				res = GetParentPropertyValue(propertyName);
			}

			// 'em' size
			if (propertyName == Css.FontSize)
			{
				if (!string.IsNullOrEmpty(res))
				{
					try
					{
						var fontSize = GetValueUnitPair(res);
						if (fontSize.Item2 == "em" || fontSize.Item2 == "%")
						{
							var parentFontSizeStr = propertyName == Css.FontSize
								? GetParentPropertyValue(Css.FontSize)
								: GetPropertyValue(Css.FontSize);
							if (!string.IsNullOrEmpty(parentFontSizeStr))
							{
								var parentFontSize = GetValueUnitPair(parentFontSizeStr);

								var ratio = fontSize.Item2.Length == 1 ? fontSize.Item1/100 : fontSize.Item1;

								return (parentFontSize.Item1*ratio).ToString(CultureInfo.InvariantCulture) + parentFontSize.Item2;
							}
						}
					}
					catch (FormatException)
					{
						return res;
					}
				}
			}
			else if (propertyName == "height" || propertyName == "width"
			         || propertyName == "min-height" || propertyName == "min-width"
			         || propertyName == "max-height" || propertyName == "max-width"
			         || propertyName == "padding-top"
			         || propertyName == "padding-right"
			         || propertyName == "padding-bottom"
			         || propertyName == "padding-left"
			         || propertyName == "margin-top"
			         || propertyName == "margin-right"
			         || propertyName == "margin-bottom"
			         || propertyName == "margin-left"
			         || propertyName == "border-top-width"
			         || propertyName == "border-right-width"
			         || propertyName == "border-bottom-width"
			         || propertyName == "border-left-width")
			{
				if (!string.IsNullOrEmpty(res) && res.EndsWith("em"))
				{
					try
					{
						var size = GetValueUnitPair(res);
						if (size.Item2 == "em")
						{
							var fontSize = GetPropertyValue(Css.FontSize);
							if (!string.IsNullOrEmpty(fontSize))
							{
								var parentFontSize = GetValueUnitPair(fontSize);

								var ratio = size.Item2.Length == 1 ? size.Item1 / 100 : size.Item1;

								return (parentFontSize.Item1 * ratio).ToString(CultureInfo.InvariantCulture) + parentFontSize.Item2;
							}
						}
					}
					catch (FormatException)
					{
						return res;
					}
				}
			}

			return res;
		}

		Tuple<float, string> GetValueUnitPair(string strValue)
		{
			var unitIdx = strValue.IndexOf(c => !char.IsDigit(c) && c != '.');
			if (unitIdx != 0)
			{
				if (unitIdx != -1)
				{
					var unit = strValue.Substring(unitIdx);
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