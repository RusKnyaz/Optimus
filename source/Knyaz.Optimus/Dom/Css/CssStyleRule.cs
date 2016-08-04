namespace Knyaz.Optimus.Dom.Css
{
	public class CssStyleRule : CssRule
	{
		public string SelectorText { get; set; }
		public CssStyleDeclaration Style { get; private set; }

		public CssStyleRule(CssStyleSheet parentStyleSheet) : base(parentStyleSheet)
		{
			Style = new CssStyleDeclaration(this);
		}
	}
}