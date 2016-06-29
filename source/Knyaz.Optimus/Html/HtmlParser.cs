using System;
using System.Collections.Generic;

namespace Knyaz.Optimus.Html
{
	class HtmlParser
	{
		internal static IEnumerable<IHtmlNode> Parse(System.IO.Stream stream)
		{
			using(var enumerator = HtmlReader.Read(stream).GetEnumerator())
			{
				var rootElem = new HtmlElement();

				var moreExist = true;
				do
				{
					moreExist = ParseElement(rootElem, enumerator);
				} while (moreExist);

				return rootElem.Children;
			}
		}

		private static bool ParseElement(HtmlElement elem, IEnumerator<HtmlChunk> enumerator)
		{
			string attributeName = null;

			while(enumerator.MoveNext())
			{
				var htmlChunk = enumerator.Current;
				switch(htmlChunk.Type)
				{
					case HtmlChunk.Types.AttributeName:
						if(attributeName!=null)
							elem.Attributes.Add(attributeName.ToLowerInvariant(), null);
						attributeName = htmlChunk.Value.ToLower();
						break;
					case HtmlChunk.Types.AttributeValue:
						if (string.IsNullOrEmpty(attributeName))
							throw new HtmlParseException("Unexpected attribute value.");
						if(!elem.Attributes.ContainsKey(attributeName.ToLowerInvariant()))//todo:
							elem.Attributes.Add(attributeName.ToLowerInvariant(), htmlChunk.Value);
						attributeName = null;
						break;
					case HtmlChunk.Types.TagStart:
						var childElem = new HtmlElement() {Name = htmlChunk.Value.ToLower()};
						ParseElement(childElem, enumerator);
						elem.Children.Add(childElem);
						break;
					case HtmlChunk.Types.TagEnd:
						if (attributeName != null && !elem.Attributes.ContainsKey(attributeName.ToLowerInvariant()))
							elem.Attributes.Add(attributeName.ToLowerInvariant(), string.Empty);
						return true;
					case HtmlChunk.Types.Text:
						elem.Children.Add(new HtmlText(){Value = htmlChunk.Value});
						break;
					case HtmlChunk.Types.Comment:
						elem.Children.Add(new HtmlComment(){ Text = htmlChunk.Value });
						break;
					case HtmlChunk.Types.DocType:
						elem.Children.Add(new HtmlDocType());
						break;
				}
			}
			return false;
		}
	}

	internal class HtmlDocType : IHtmlNode
	{
	}

	internal class HtmlComment : IHtmlNode
	{
		public string Text;
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
