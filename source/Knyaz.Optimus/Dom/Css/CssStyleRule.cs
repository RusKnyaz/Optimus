using System;
using System.Diagnostics;
using System.Linq;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.Dom.Css
{
	[DebuggerDisplay("CssStyleRule, Selector: {SelectorText}")]
	public class CssStyleRule : CssRule
	{
		internal event Action SelectorChanged;

		private string _selectorText;

		public string SelectorText
		{
			get { return _selectorText; }
			set
			{
				if (_selectorText != null)
					_selectors = null;
				_selectorText = value;
				if (SelectorChanged != null)
					SelectorChanged();
			}
		}

		private CssSelector[] _selectors = null;

		public CssStyleDeclaration Style { get; private set; }

		public CssStyleRule(CssStyleSheet parentStyleSheet) : base(parentStyleSheet)
		{
			Style = new CssStyleDeclaration(this);
		}

		internal CssSelector[] Selectors 
		{ 
			get
			{
				return _selectors ?? (_selectors =
					SelectorText.Split(',')
					.Select(x => x.Trim())
					.Where(x => !string.IsNullOrEmpty(x))
					.Select(x => new CssSelector(x))
					.ToArray());
			}
		}

		/// <summary>
		/// Check if the specified elt matched by selector
		/// </summary>
		/// <param name="elt"></param>
		/// <returns></returns>
		internal bool IsMatchesSelector(IElement elt)
		{
			return Selectors.Any( x=> x.IsMatches(elt));
		}
	}
}