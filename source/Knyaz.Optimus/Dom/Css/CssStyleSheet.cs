using System;
using System.Collections.Generic;
using System.IO;
using Knyaz.Optimus.Html;

namespace Knyaz.Optimus.Dom.Css
{
	/// <summary>
	/// Represents a single CSS style sheet.
	/// </summary>
	public class CssStyleSheet
	{
		internal CssStyleSheet() => CssRules = new List<CssRule>();
		internal event Action Changed;

		//todo: readonly list
		/// <summary>
		/// Gets a live CSSRuleList, listing the CssRule objects in the style sheet.
		/// </summary>
		public IList<CssRule> CssRules { get; private set; }

		/// <summary>
		/// Deletes a rule at the specified position from the style sheet.
		/// </summary>
		/// <param name="idx">The  position of the role to be removed.</param>
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

		/// <summary>
		/// Inserts a new rule at the specified position in the style sheet, given the textual representation of the rule.
		/// </summary>
		/// <param name="rule">The textual representation of the rule to be inserted.</param>
		/// <param name="idx">The desired insertion position.</param>
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
