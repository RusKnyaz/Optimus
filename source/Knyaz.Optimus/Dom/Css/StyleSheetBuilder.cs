using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Knyaz.Optimus.Html;

namespace Knyaz.Optimus.Dom.Css
{
	class StyleSheetBuilder
	{
		public static CssStyleSheet CreateStyleSheet(TextReader reader, Func<string, TextReader> getImport)
		{
			var styleSheet = new CssStyleSheet();
			FillStyleSheet(reader, styleSheet, getImport);
			return styleSheet;
		}

		private static void FillStyleSheet(TextReader reader, CssStyleSheet styleSheet, Func<string, TextReader> getImport)
		{
			using (var enumerator = CssReader.Read(reader).GetEnumerator())
			{
				if (!enumerator.MoveNext())
					return;
				CssRule rule;
				while (enumerator.Current.Type == CssChunkTypes.Directive 
					&& enumerator.Current.Data != null && !enumerator.Current.Data.StartsWith("media "))
				{
					if (enumerator.Current.Data.StartsWith("import "))
					{
						var dirrective = enumerator.Current.Data;
						var import = dirrective.Substring(7);
						Import(styleSheet, import, getImport);
					}
					enumerator.MoveNext();
				}

				while (CreateRule(styleSheet, enumerator, out rule))
				{
					if (rule != null)
					{
						styleSheet.CssRules.Add(rule);
					}
				}
				styleSheet.CssRules.Add(rule);
			}
		}

		private static void Import(CssStyleSheet styleSheet, string import, Func<string, TextReader> getImport)
		{
			var url = import.Trim();
			url = url.Substring(url.IndexOf('"')+1);
			url = url.Substring(0, url.IndexOf('"'));
			var reader = getImport(url);
			FillStyleSheet(reader, styleSheet, getImport);
		}

		public static bool CreateRule(CssStyleSheet styleSheet, IEnumerator<CssChunk> enumerator, out CssRule rule)
		{
			if (enumerator.Current.Type == CssChunkTypes.Directive)
			{
				if (enumerator.Current.Data.StartsWith("media "))
				{
					var mediaRule = new CssMediaRule(enumerator.Current.Data, styleSheet);
					rule = mediaRule;

					CssRule childRule;
					bool cont = true;
					enumerator.MoveNext();
					while (enumerator.Current.Type != CssChunkTypes.End &&
					       (cont = CreateRule(styleSheet, enumerator, out childRule)))
					{
						mediaRule.CssRules.Add(childRule);
					}

					return enumerator.Current.Type == CssChunkTypes.End ? enumerator.MoveNext() : cont;
				}
				else //skip other directives
				{
					rule = null;
					while (enumerator.Current.Type != CssChunkTypes.End)
					{
						if (!enumerator.MoveNext())
							return false;
					}
					return enumerator.MoveNext();
				}
			}

			if (enumerator.Current.Type != CssChunkTypes.Selector)
				throw new Exception("Unable to parse rule");

			var styleRule = new CssStyleRule(styleSheet) { SelectorText = enumerator.Current.Data };
			rule = styleRule;
			enumerator.MoveNext();
			return FillStyle(styleRule.Style, enumerator);
		}

		public static void FillStyle(CssStyleDeclaration style, string str)
		{
			if (str[0] != '{')
				str = '{' + str;
			str = "toskip" + str;

			if (str.Last() != '}')
				str += '}';

			using (var enumerator = CssReader.Read(new StringReader(str)).GetEnumerator())
			{
				//skip selector
				while (enumerator.MoveNext() && enumerator.Current.Type == CssChunkTypes.Selector) ;

				FillStyle(style, enumerator);
			}
		}

		private static bool FillStyle(CssStyleDeclaration style, IEnumerator<CssChunk> enumerator)
		{
			string property = null;
			do
			{
				var cssChunk = enumerator.Current;
				switch (cssChunk.Type)
				{
					case CssChunkTypes.Selector:
						return true;
					case CssChunkTypes.Property:
						property = cssChunk.Data;
						break;
					case CssChunkTypes.Value:
						var data = cssChunk.Data;
						var important = "";
						if (data.EndsWith("!important"))
						{
							data = data.Substring(0, data.Length - 10).TrimEnd();
							important = "important";
						}
						style.SetProperty(property, data, important);
						property = null;
						break;
					case CssChunkTypes.End:
					case CssChunkTypes.Directive:
						return true;
				}
			} while (enumerator.MoveNext());
			return false;
		}
	}
}