using System;
using System.Collections.Generic;

namespace WebBrowser.Html
{
	class HtmlParser
	{
		internal static IEnumerable<IHtmlNode> Parse(System.IO.Stream stream)
		{
			using(var enumerator = HtmlReader.Read(stream).GetEnumerator())
			{
				while(enumerator.MoveNext())
				{
					var htmlChunk = enumerator.Current;
					if (htmlChunk.Type == HtmlChunkTypes.TagStart)
						yield return BuildElement(htmlChunk, enumerator);

					if(htmlChunk.Type == HtmlChunkTypes.Text)
						yield return new HtmlText(){Value = htmlChunk.Value};
				}
			}
		}

		private static IHtmlElement BuildElement(HtmlChunk tagStart, IEnumerator<HtmlChunk> enumerator)
		{
			var elem = new HtmlElement() { Name = tagStart.Value.ToLower() };

			string attributeName = null;

			while(enumerator.MoveNext())
			{
				var htmlChunk = enumerator.Current;
				switch(htmlChunk.Type)
				{
					case HtmlChunkTypes.AttributeName:
						if(attributeName!=null)
							elem.Attributes.Add(attributeName.ToLowerInvariant(), null);
						attributeName = htmlChunk.Value.ToLower();		
						break;
					case HtmlChunkTypes.AttributeValue:
						if (string.IsNullOrEmpty(attributeName))
							throw new HtmlParseException("Unexpected attribute value.");
						elem.Attributes.Add(attributeName.ToLowerInvariant(), htmlChunk.Value);
						attributeName = null;
						break;
					case HtmlChunkTypes.TagStart:
						elem.Children.Add(BuildElement(htmlChunk, enumerator));
						break;
					case HtmlChunkTypes.TagEnd:
						return elem;
					case HtmlChunkTypes.Text:
						elem.Children.Add(new HtmlText(){Value = htmlChunk.Value});
						break;
				}
			}

			return elem;
		}
	}

	internal class HtmlElement : IHtmlElement
	{
		public HtmlElement()
		{
			Attributes = new Dictionary<string, string>();
			Children = new List<IHtmlNode>();
			InnerText = string.Empty;
		}

		public string Name { get; set; }

		public IList<IHtmlNode> Children
		{
			get;
			private set;
		}

		public IDictionary<string, string> Attributes { get; private set; }


		IEnumerable<IHtmlNode> IHtmlElement.Children
		{
			get { return this.Children; }
		}

		public string InnerText { get; set; }
	}

	interface IHtmlNode
	{
		 
	}

	interface IHtmlElement : IHtmlNode
	{
		IEnumerable<IHtmlNode> Children { get; }
		string Name { get; }
		IDictionary<string, string> Attributes { get; }
	}

	interface IHtmlText : IHtmlNode
	{
		string Value { get; }
	}

	class HtmlText : IHtmlText
	{
		public string Value { get; set; }
	}

	class HtmlParseException : Exception
	{
		public HtmlParseException(string message) : base(message) { }
	}
}
