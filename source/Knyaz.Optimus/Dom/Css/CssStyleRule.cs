using System.Linq;
using Knyaz.Optimus.Dom.Elements;

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

		/// <summary>
		/// Check if the specified elt matched by selector
		/// </summary>
		/// <param name="elt"></param>
		/// <returns></returns>
		internal bool IsMatchesSelector(Element elt)
		{
			var chunks = SelectorText.Split(' ');



			var htmlElt = elt as HtmlElement;

			foreach (var chunk in chunks)
			{
				if (chunk[0] == '#')
				{
					if(elt.Id != chunk.Substring(1))
						return false;
					continue;
				}
				if (chunk[0] == '.')
				{
					if(htmlElt == null || !htmlElt.ClassName.Split(' ').Contains(chunk.Substring(1)))
						return false;
					continue;
				}
				//todo: attributes
				if(elt.TagName != chunk.ToUpper())
					return false;
			}

			return true;
		}
	}
}