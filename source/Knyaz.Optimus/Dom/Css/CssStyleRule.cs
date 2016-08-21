using System.Diagnostics;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.Dom.Css
{
	[DebuggerDisplay("CssStyleRule, Selector: {SelectorText}")]
	public class CssStyleRule : CssRule
	{
		private string _selectorText;

		public string SelectorText
		{
			get { return _selectorText; }
			set
			{
				if (_selectorText != null)
					_selector = null;
				_selectorText = value;
			}
		}

		private CssSelector _selector = null;

		public CssStyleDeclaration Style { get; private set; }

		public CssStyleRule(CssStyleSheet parentStyleSheet) : base(parentStyleSheet)
		{
			Style = new CssStyleDeclaration(this);
		}

		/// <summary>
		/// Check if the specified elt matched by selector
		/// </summary>
		/// <param name="elt"></param>
		/// <returns></returns>
		internal bool IsMatchesSelector(IElement elt)
		{
			var selector = _selector ?? (_selector = new CssSelector(SelectorText));
			return selector.IsMatches(elt);
		}
	}
}