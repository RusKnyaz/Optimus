using System.Linq;
using System.Text.RegularExpressions;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Tools;
using HtmlElement = Knyaz.Optimus.Dom.Elements.HtmlElement;

namespace Knyaz.Optimus.Dom.Css
{
	class CssSelector
	{
		class Node
		{
			public string Value;
			public Node Next;
		}

		private readonly string _text;

		private Node _chain;

		public CssSelector(string text)
		{
			_text = text;
			foreach (var chunk in NormalizeSelector(_text).Split(' ').Where(x => !string.IsNullOrEmpty(x)))
			{
				_chain = new Node {Value = chunk, Next=_chain};
			}
		}

		public bool IsMatches(IElement elt)
		{
			return IsMatches(elt, _chain);
		}

		private bool IsMatches(IElement elt, Node chain)
		{
			var chunk = chain.Value;

			if (chunk == "*")
				return true;

			if (chunk[0] == '#')
			{
				if (elt.Id != chunk.Substring(1))
					return false;
			}
			else if (chunk[0] == '.')
			{
				var htmlElt = elt as HtmlElement;
				if (htmlElt == null || !htmlElt.ClassName.Split(' ').Contains(chunk.Substring(1)))
					return false;
			}
			else
			{
				if (chunk.ToUpper().Split(',').All(x => elt.TagName != x))
					return false;
			}

			if (chain.Next != null)
			{
				if (chain.Next.Value == ">")
				{
					return IsMatches((IElement) elt.ParentNode, chain.Next.Next);
				}
				else
				{
					return elt.ParentNode.GetRecursive(x => x.ParentNode).OfType<IElement>().Any(x => IsMatches(x, chain.Next));
				}
			}

			return true;
		}

		private static Regex _normalizeCommas = new Regex("\\s*\\,\\s*", RegexOptions.Compiled);

		private string NormalizeSelector(string selector)
		{
			selector = selector.Replace('\n', ' ').Replace('\r', ' ');
			return _normalizeCommas.Replace(selector, ",");
		}
	}
}
