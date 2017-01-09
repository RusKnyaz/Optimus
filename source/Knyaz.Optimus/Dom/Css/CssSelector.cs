using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Tools;
using HtmlElement = Knyaz.Optimus.Dom.Elements.HtmlElement;

namespace Knyaz.Optimus.Dom.Css
{
	class CssSelector
	{
		private List<Node> _chains = new List<Node>();

		IEnumerable<string> SplitSelector(string selector)
		{
			var buffer = new StringBuilder();

			var readText = char.IsLetterOrDigit(selector[0]);

			foreach (var c in selector)
			{
				if(c == '\r' || c == '\n')
					continue;

				if (char.IsLetterOrDigit(c) != readText)
				{
					if (!readText)
					{
						foreach (var d in buffer.ToString())
						{
							yield return d.ToString();
						}
					}
					else
						yield return buffer.ToString();
					buffer.Clear();
					readText = !readText;
				}

				buffer.Append(c);
			}
			if (buffer.Length > 0)
			{
				if (!readText)
				{
					foreach (var d in buffer.ToString())
					{
						yield return d.ToString();
					}
				}
				else
					yield return buffer.ToString();
			}
		}


		public CssSelector(string text)
		{
			var orParts = text.Split(',');
			foreach (var part in orParts)
			{
				Node _chain = null;
				var normalized = NormalizeSelector(part.Trim());

				ChunkTypes currentChunkType = ChunkTypes.Tags;
				foreach (var chunk in SplitSelector(normalized).Where(x => !string.IsNullOrEmpty(x)))
				{
					switch (chunk[0])
					{
						case '#':
							currentChunkType = ChunkTypes.Id;
							break;
						case '.':
							currentChunkType = ChunkTypes.Class;
							break;
						case '*':
							_chain = new Node {Value = null, Next = _chain, Type = ChunkTypes.All};
							break;
						case '>':
							_chain = new Node {Value = null, Next = _chain, Type = ChunkTypes.Parent};
							break;
						case ' ':
							_chain = new Node {Value = null, Next = _chain, Type = ChunkTypes.Ancestor};
							break;
						default:
							_chain = new Node {Value = chunk, Next = _chain, Type = currentChunkType};
							currentChunkType = ChunkTypes.Tags;
							break;
					}
				}
				if(_chain != null)
					_chains.Add(_chain);
			}
		}
		
		enum ChunkTypes
		{
			Raw, Id, Class, Tags, All, Parent, Ancestor
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
				var htmlElt = elt as HtmlElement;
				if (htmlElt == null)
					return false;

				return elt.TagName == chain.Value.ToUpperInvariant();
			}

			if (chain.Next != null)
			{
				if (chain.Next.Type == ChunkTypes.Parent)
				{
					return IsMatches((IElement) elt.ParentNode, chain.Next.Next);
				}
				else if (chain.Next.Type == ChunkTypes.Ancestor)
				{
					return elt.ParentNode.GetRecursive(x => x.ParentNode).OfType<IElement>().Any(x => IsMatches(x, chain.Next.Next));
				}
				else if (chain.Type == ChunkTypes.All)
				{
					return ((INode) elt).GetRecursive(x => x.ParentNode).OfType<IElement>().Any(x => IsMatches(x, chain.Next));
				}
			}

			return true;
		}

		private static readonly Regex _normalizeCommas = new Regex("\\s*\\,\\s*", RegexOptions.Compiled);
		private static readonly Regex _normalizeGt = new Regex("\\s*\\>\\s*", RegexOptions.Compiled);
		private static readonly Regex _normalizeSpaces = new Regex("\\s+", RegexOptions.Compiled);

		private static string NormalizeSelector(string selector)
		{
			selector = selector.Replace('\n', ' ').Replace('\r', ' ');
			selector = _normalizeCommas.Replace(selector, ",");
			selector = _normalizeGt.Replace(selector, ">");
			selector = _normalizeSpaces.Replace(selector, " ");
			return selector;
		}
	}
}
