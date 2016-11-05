using System;
using System.Collections.Generic;

namespace Knyaz.Optimus.Dom.Css
{
	public class CssMediaRule : CssRule
	{
		public CssMediaRule(string query, CssStyleSheet parentStyleSheet) : base(parentStyleSheet)
		{
			Media = new MediaList(query);
			CssRules = new List<CssRule>();
		}

		/// <summary>
		/// Delete a rule from the media block.
		/// </summary>
		/// <param name="index"></param>
		public void DeleteRule(int index)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// A list of all CSS rules contained within the media block.
		/// </summary>
		public IList<CssRule> CssRules { get; private set; }

		/// <summary>
		/// A list of media types for this rule.
		/// </summary>
		public MediaList Media { get; private set; }

		/// <summary>
		/// Used to insert a new rule into the media block.
		/// </summary>
		/// <param name="rule"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public int InsertRule(string rule, int index)
		{
			throw new NotImplementedException();
		}
	}
}
