using System;
using System.Collections.Generic;
using System.IO;
using Knyaz.Optimus.Html;

namespace Knyaz.Optimus.Dom.Css
{
	public class CssStyleSheet
	{
		public CssStyleSheet()
		{
			CssRules = new List<CssRule>();
		}

		public IList<CssRule> CssRules { get; private set; }

		public void DeleterRule(int idx)
		{
			CssRules.RemoveAt(idx);
		}

		public void InsertRule(string rule, int idx)
		{
			using (var enumerator = CssReader.Read(new StringReader(rule)).GetEnumerator())
			{
				enumerator.MoveNext();
				CssStyleRule r;
				StyleSheetBuilder.CreateRule(this, enumerator, out r);
				CssRules.Add(r);
			}
		}
	}
}
