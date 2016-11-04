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

		internal event Action Changed;

		//todo: readonly list
		public IList<CssRule> CssRules { get; private set; }

		public void DeleteRule(int idx)
		{
			var r = CssRules[idx] as CssStyleRule;
			CssRules.RemoveAt(idx);

			if (r != null)
			{
				r.SelectorChanged -= OnChanged;
			}
			
			OnChanged();
		}

		private void OnChanged()
		{
			if (Changed != null)
				Changed();
		}

		public void InsertRule(string rule, int idx)
		{
			using (var enumerator = CssReader.Read(new StringReader(rule)).GetEnumerator())
			{
				enumerator.MoveNext();
				CssRule r;
				StyleSheetBuilder.CreateRule(this, enumerator, out r);
				CssRules.Add(r);
				var styleRule = r as CssStyleRule;
				if(styleRule != null)
					styleRule.SelectorChanged += OnChanged;
			}
			OnChanged();
		}
	}
}
