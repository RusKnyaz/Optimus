using System;
using System.Diagnostics;
using System.Linq;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Dom.Css
{
	/// <summary>
	/// Represents a single CSS style rule.
	/// </summary>
	[DebuggerDisplay("CssStyleRule, Selector: {SelectorText}")]
	public class CssStyleRule : CssRule
	{
		internal event Action SelectorChanged;
		private string _selectorText;
		private CssSelector[] _selectors = null;
		
		internal CssStyleRule(CssStyleSheet parentStyleSheet) : base(parentStyleSheet) =>
			Style = new CssStyleDeclaration(this);

		/// <summary>
		/// Gets the textual representation of the selector for this rule.
		/// </summary>
		public string SelectorText
		{
			get => _selectorText;
			set
			{
				if (_selectorText != null)
					_selectors = null;
				_selectorText = value;
				if (SelectorChanged != null)
					SelectorChanged();
			}
		}

		
		/// <summary>
		/// Gets the parent CSSStyleDeclaration object for the rule.
		/// </summary>
		public CssStyleDeclaration Style { get; private set; }
		
		public override string CssText 
		{
			get { return SelectorText + Style; }
			set { }
		}

		internal CssSelector[] Selectors => _selectors ?? (_selectors =
			                                    SelectorText.Split(',')
				                                    .Select(x => x.Trim())
				                                    .Where(x => !string.IsNullOrEmpty(x))
				                                    .Select(x => new CssSelector(x))
				                                    .ToArray());

		/// <summary>
		/// Check if the specified elt matched by selector
		/// </summary>
		/// <param name="elt"></param>
		/// <returns></returns>
		internal bool IsMatchesSelector(IElement elt) => Selectors.Any( x=> x.IsMatches(elt));
	}
}