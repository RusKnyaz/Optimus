using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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

		static bool IsChunkChar(char c)
		{
			return char.IsLetterOrDigit(c) || c == '-' || c == '_' || c=='\"' || c=='=' || c=='~' || c=='|';
		}

		IEnumerable<string> SplitSelector(string selector)
		{
			var buffer = new StringBuilder();

			var readText = IsChunkChar(selector[0]) ;

			foreach (var c in selector)
			{
				if(c == '\r' || c == '\n')
					continue;

				if (IsChunkChar(c) != readText)
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

		/// <summary>
		/// Priority of the selector.
		/// </summary>
		public readonly int Specifity;


		public CssSelector(string text)
		{
			var orParts = text.Split(',');
			foreach (var part in orParts)
			{
				Node chain = null;
				var normalized = NormalizeSelector(part.Trim());

				var currentChunkType = ChunkTypes.Tags;
				foreach (var chunk in SplitSelector(normalized).Where(x => !string.IsNullOrEmpty(x)))
				{
					switch (chunk[0])
					{
						case '#':
							currentChunkType = ChunkTypes.Id;
							Specifity += 65536;
							break;
						case '.':
							Specifity += 256;
							currentChunkType = ChunkTypes.Class;
							break;
						case ':':
							Specifity++;	
							currentChunkType = ChunkTypes.State;
							break;
						case '[':
							Specifity += 256;
							currentChunkType = ChunkTypes.Attribute;
							break;
						case ']':
							currentChunkType = ChunkTypes.Tags;
							break;
						case '*':
							chain = new Node {Value = null, Next = chain, Type = ChunkTypes.All};
							break;
						case '>':
							chain = new Node {Value = null, Next = chain, Type = ChunkTypes.Parent};
							break;
						case ' ':
							chain = new Node {Value = null, Next = chain, Type = ChunkTypes.Ancestor};
							break;
						default:
							if (currentChunkType == ChunkTypes.Attribute)
							{
								var parts = chunk.Split('=');
								if (parts.Length == 1)
								{
									chain = new Node {Value = chunk, Next = chain, Type = currentChunkType};
									break;
								}

								var attr = parts[0];

								parts[1] = parts[1].Trim('\"');

								if(attr.Last() == '~')
								{
									chain = new Node { Value = attr.Substring(0, attr.Length -1),
										AttrValue = parts[1], Next = chain, Type = ChunkTypes.AttributeContains};
								}
								else if (attr.Last() == '|')
								{
									chain = new Node
									{
										Value = attr.Substring(0, attr.Length - 1),
										AttrValue = parts[1],
										Next = chain,
										Type = ChunkTypes.AttributeStartWith
									};
								}
								else
								{
									chain = new Node
									{
										Value = attr,
										AttrValue = parts[1],
										Next = chain,
										Type = ChunkTypes.Attribute
									};
								}
								break;
							}

							chain = new Node {Value = currentChunkType == ChunkTypes.Tags ? chunk.ToUpperInvariant(): chunk, Next = chain, Type = currentChunkType};
							if (currentChunkType == ChunkTypes.Tags)
								Specifity++;
							currentChunkType = ChunkTypes.Tags;
							break;
					}
				}
				if(chain != null)
					_chains.Add(chain);
			}
		}
		
		enum ChunkTypes
		{
			Raw, Id, Class, Tags, All, Parent, Ancestor, State,
			Attribute, AttributeContains, AttributeStartWith
		}

		class Node
		{
			public ChunkTypes Type;
			public string Value;
			public string AttrValue;
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

				if (chunk.Split('.').Any(x => !htmlElt.ClassList.Contains(x)))
					return false;
			}
			else if (chain.Type == ChunkTypes.Tags)
			{
				var htmlElt = elt as HtmlElement;
				if (htmlElt == null)
					return false;

				if(elt.TagName != chain.Value)
					return false;
			} else if (chain.Type == ChunkTypes.Attribute)
			{
				var htmlElt = elt as HtmlElement;
				if (htmlElt == null)
					return false;

				if (chain.AttrValue == null)
				{
					if (htmlElt.GetAttributeNode(chain.Value) == null)
						return false;
				}
				else
				{
					if (htmlElt.GetAttribute(chain.Value) != chain.AttrValue)
						return false;
				}
			}else if (chain.Type == ChunkTypes.AttributeContains)
			{
				var htmlElt = elt as HtmlElement;
				if (htmlElt == null)
					return false;

				if (chain.AttrValue == string.Empty)
					return false;

				var attr = htmlElt.GetAttribute(chain.Value);
				if (attr == null)
					return false;

				if (!attr.Split(' ').Contains(chain.AttrValue))
					return false;
			}else if (chain.Type == ChunkTypes.AttributeStartWith)
			{
				var htmlElt = elt as HtmlElement;
				if (htmlElt == null)
					return false;
				var attr = htmlElt.GetAttribute(chain.Value);
				if (attr == null)
					return false;

				if (!attr.StartsWith(chain.AttrValue+"-") && attr != chain.AttrValue)
					return false;
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
				else return IsMatches(elt, chain.Next);
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
