using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Knyaz.Optimus.Html;

namespace Knyaz.Optimus.Dom.Css
{
	class StyleSheetBuilder
	{
		public static CssStyleSheet CreateStyleSheet(TextReader reader)
		{
			var styleSheet = new CssStyleSheet();
			var enumerator = CssReader.Read(reader).GetEnumerator();
			if(!enumerator.MoveNext())
				throw new Exception("Unable to parse rule");
			CssStyleRule rule;
			while (CreateRule(styleSheet, enumerator, out rule))
			{
				styleSheet.CssRules.Add(rule);
			}

			return styleSheet;
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
						style.SetProperty(property, cssChunk.Data, "");
						property = null;
						break;
				}
			} while (enumerator.MoveNext());
			return false;
		}
	}
}