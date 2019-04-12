using System;
using System.Collections.Generic;

namespace Knyaz.Optimus.Html
{
	class HtmlParser
	{
		internal static IEnumerable<IHtmlNode> Parse(System.IO.Stream stream)
		{
			using(var enumerator = FixHtml(HtmlReader.Read(stream)).GetEnumerator())
			{
				//todo: try to get rid of fake element
				var rootElem = new HtmlElement("root");

				while (ParseElement(rootElem, enumerator) == ParseResult.Ok){}

				return rootElem.Children;
			}
		}

		private static IEnumerable<HtmlChunk> FixHtml(IEnumerable<HtmlChunk> html)
		{
			var tags = new List<string>();
			foreach (var chunk in html)
			{
				if(chunk.Type == HtmlChunk.Types.TagStart)
					tags.Add(chunk.Value);
				else if (chunk.Type == HtmlChunk.Types.TagEnd)
				{
					int idx;
					for(idx = tags.Count -1; idx>=0;idx--)
						if (StringComparer.InvariantCultureIgnoreCase.Equals(tags[idx], chunk.Value))
							break;

					if (idx >= 0)
						tags.RemoveRange(idx, tags.Count - idx);
					else
						continue;
				}

				yield return chunk;
			}
		}

		enum ParseResult : byte
		{
			End,
			BackToParent,
			Ok
		}

		private static ParseResult ParseElement(HtmlElement elem, IEnumerator<HtmlChunk> enumerator)
		{
			string attributeName = null;
			var reread = false;
			while (reread || enumerator.MoveNext())
			{
				reread = false;
				var htmlChunk = enumerator.Current;
				switch (htmlChunk.Type)
				{
					case HtmlChunk.Types.AttributeName:
						//attribute without value followed by attribute
						if (attributeName != null && !elem.Attributes.ContainsKey(attributeName))
							elem.Attributes.Add(attributeName, null);

						attributeName = htmlChunk.Value.ToLowerInvariant();
						break;
					case HtmlChunk.Types.AttributeValue:
						if (string.IsNullOrEmpty(attributeName))
							throw new HtmlParseException("Unexpected attribute value.");
						if (!elem.Attributes.ContainsKey(attributeName)) //todo:
							elem.Attributes.Add(attributeName, htmlChunk.Value);
						attributeName = null;
						break;
					case HtmlChunk.Types.TagStart:
						var tagName = htmlChunk.Value.ToLower();
						if (ClosePrevTag(elem.Name, tagName))
							return ParseResult.BackToParent;

						var childElem = new HtmlElement(htmlChunk.Value);
						if (ParseElement(childElem, enumerator) == ParseResult.BackToParent)
							reread = true;
						elem.Children.Add(childElem);
						break;
					case HtmlChunk.Types.TagEnd:
						if (attributeName != null && !elem.Attributes.ContainsKey(attributeName))
							elem.Attributes.Add(attributeName, string.Empty);

						return
							string.Equals(elem.Name, htmlChunk.Value, StringComparison.InvariantCultureIgnoreCase) 
								? ParseResult.Ok : ParseResult.BackToParent;
					case HtmlChunk.Types.Text:
						elem.Children.Add(new HtmlText(htmlChunk.Value));
						break;
					case HtmlChunk.Types.Comment:
						elem.Children.Add(new HtmlComment {Text = htmlChunk.Value});
						break;
					case HtmlChunk.Types.DocType:
						elem.Children.Add(new HtmlDocType());
						break;
				}
			} 
			return ParseResult.End;
		}

		private static bool ClosePrevTag(string prevName, string name)
		{
			return (prevName == "li" && name == "li") 
				|| (prevName == "dl" && (name == "dt" || name == "dl"))
				|| (prevName == "dt" && (name == "dt" || name == "dl"))

				|| (prevName == "th" && (name == "td" || name == "th" || name == "tr" || name == "tbody" || name=="tfoot"))
				|| (prevName == "td" && (name == "td" || name == "th" || name == "tr" || name == "tbody" || name == "tfoot"))
				|| (prevName == "tr" && (name == "tr" || name=="tbody" || name=="tfoot"))
				
				|| (prevName == "option" && (name == "option" || name == "optgroup"))
			    || (prevName == "optgroup" && name == "optgroup")
				
				|| (prevName == "thead" && (name == "tbody" || name == "tfoot"));
		}
	}

	internal class HtmlDocType : IHtmlNode
	{
	}

	internal class HtmlComment : IHtmlNode
	{
		public string Text;
	}

	internal sealed class HtmlElement : IHtmlNode
	{
		public HtmlElement(string name)
		{
			Name = name;
			Attributes = new Dictionary<string, string>();
			Children = new List<IHtmlNode>();
		}

		public readonly string Name;
		public readonly IList<IHtmlNode> Children;
		public readonly IDictionary<string, string> Attributes;
	}

	interface IHtmlNode{}

	sealed class HtmlText : IHtmlNode
	{
		public HtmlText(string value) => Value = value;
		public readonly string Value;
	}

	class HtmlParseException : Exception
	{
		public HtmlParseException(string message) : base(message) { }
	}
}
