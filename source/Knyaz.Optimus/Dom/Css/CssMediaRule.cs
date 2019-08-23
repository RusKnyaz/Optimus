using System;
using System.Collections.Generic;
using System.Linq;

namespace Knyaz.Optimus.Dom.Css
{
	/// <summary>
	/// Provides access to the media rules styles definitions.
	/// </summary>
	public class CssMediaRule : CssRule
	{
		internal CssMediaRule(string cssText, CssStyleSheet parentStyleSheet) : base(parentStyleSheet)
		{
			Media = new MediaList(cssText);
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

		public override string CssText
		{
			get => "@"+ Media.MediaText + " {" + string.Join(" ", CssRules.Select(x => x.CssText)) + "}";
			set{}
		}
	}
}
