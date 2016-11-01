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
				CssStyleRule rule;
				while (enumerator.Current.Type == CssChunkTypes.Directive)
				{
					var dirrective = enumerator.Current.Data;
					if (dirrective.StartsWith("import "))
					{
						var import = dirrective.Substring(7);
						Import(styleSheet, import, getImport);
					}
					enumerator.MoveNext();
				}

				while (CreateRule(styleSheet, enumerator, out rule))
				{
					styleSheet.CssRules.Add(rule);
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

		public static bool CreateRule(CssStyleSheet styleSheet, IEnumerator<CssChunk> enumerator, out CssStyleRule rule)
		{
			if (enumerator.Current.Type != CssChunkTypes.Selector)
				throw new Exception("Unable to parse rule");

			rule = new CssStyleRule(styleSheet) { SelectorText = enumerator.Current.Data };
			enumerator.MoveNext();
			return FillStyle(rule.Style, enumerator);
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
				}
			} while (enumerator.MoveNext());
			return false;
		}
	}
}