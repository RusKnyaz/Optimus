using System.Collections.Generic;
using System.IO;
using System.Linq;
using Knyaz.Optimus.Dom.Css.Expression;
using Knyaz.Optimus.Tools;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Dom.Css
{
	class CssSelector
	{
		/// <summary> Priority of the selector. </summary>
		public readonly int Specificity;

		private readonly CssExpression _expression;

		public CssSelector(string text)
		{
			_expression = CssExpressionParser.Parse(new StringReader(text));
			Specificity = _expression.GetSpecificity();
		}
		
		/// <summary> Check if the element matched by given selector. </summary>
		/// <param name="elt">The element to be checked.</param>
		/// <returns></returns>
		public bool IsMatches(IElement elt) => _expression.Match(elt);

		
		public IEnumerable<IElement> Select(IElement root) => 
			root.Flatten().OfType<IElement>().Where(IsMatches);

		public static implicit operator CssSelector(string query) => 
			new CssSelector(query);
	}
}
