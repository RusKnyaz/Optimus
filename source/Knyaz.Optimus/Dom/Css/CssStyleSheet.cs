using System.Collections.Generic;

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
			//todo: parse rule
			//CssRules.Insert(idx, rule);
		}
	}
}
