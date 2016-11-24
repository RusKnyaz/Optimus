using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Tools;
using HtmlElement = Knyaz.Optimus.Dom.Elements.HtmlElement;

namespace Knyaz.Optimus.Dom.Css
{
	class CssSelector
	{
		private List<Node> _chains = new List<Node>();

		public CssSelector(string text)
		{
			var orParts = text.Split(',');
			foreach (var part in orParts)
			{
				Node _chain = null;
				foreach (var chunk in NormalizeSelector(part.Trim()).Split(' ').Where(x => !string.IsNullOrEmpty(x)))
				{
					switch (chunk[0])
					{
						case '#':
							_chain = new Node {Value = chunk.Substring(1), Next = _chain, Type = ChunkTypes.Id};
							break;
						case '.':
							_chain = new Node {Value = chunk.Substring(1), Next = _chain, Type = ChunkTypes.Class};
							break;
						case '*':
							_chain = new Node {Value = null, Next = _chain, Type = ChunkTypes.All};
							break;
						case '>':
							_chain = new Node {Value = null, Next = _chain, Type = ChunkTypes.Parent};
							break;
						default:
							_chain = new Node {Values = chunk.ToUpper().Split(','), Next = _chain, Type = ChunkTypes.Tags};
							break;
					}
				}
				if(_chain != null)
					_chains.Add(_chain);
			}
		}
		
		enum ChunkTypes
		{
			Raw, Id, Class, Tags, All, Parent
		}

		class Node
		{
			public ChunkTypes Type;
			public string Value;
			public string[] Values;
			public Node Next;
		}

		
		public bool IsMatches(IElement elt)
		{
			return _chains.Any(chain => IsMatches(elt, chain));
		}

		private bool IsMatches(IElement elt, Node chain)
		{
			var chunk = chain.Value;
			
			if (chain.Type == ChunkTypes.Id)
			{
				if (elt.Id != chunk)
					return false;
			}
			else if (chain.Type == ChunkTypes.Class)
			{
				var htmlElt = elt as HtmlElement;
				if (htmlElt == null)
					return false;

				if (chunk.Split('.').Any(x => !htmlElt.ClassName.Split(' ').Contains(x)))
					return false;
			}
			else if (chain.Type == ChunkTypes.Tags)
			{
				if (chain.Values.All(x => elt.TagName != x))
					return false;
			}

			if (chain.Next != null)
			{
				if (chain.Next.Type == ChunkTypes.Parent)
				{
					return IsMatches((IElement) elt.ParentNode, chain.Next.Next);
				}

				return 
						chain.Type == ChunkTypes.All
						? ((INode)elt).GetRecursive(x => x.ParentNode).OfType<IElement>().Any(x => IsMatches(x, chain.Next))
						: elt.ParentNode.GetRecursive(x => x.ParentNode).OfType<IElement>().Any(x => IsMatches(x, chain.Next));
			}

			return true;
		}

		private static readonly Regex _normalizeCommas = new Regex("\\s*\\,\\s*", RegexOptions.Compiled);

		private static string NormalizeSelector(string selector)
		{
			selector = selector.Replace('\n', ' ').Replace('\r', ' ');
			return _normalizeCommas.Replace(selector, ",");
		}
	}
}
