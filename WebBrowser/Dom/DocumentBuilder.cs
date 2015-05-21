using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebBrowser.Dom.Elements;
using WebBrowser.Html;

namespace WebBrowser.Dom
{
	internal class DocumentBuilder
	{
		public static IEnumerable<INode> Build(Document doc, string htmlString)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlString)))
			{
				return Build(doc, stream);
			}
		}

		public static IEnumerable<INode> Build(Document doc, Stream stream)
		{
			var html = HtmlParser.Parse(stream);
			return Build(doc, html).ToList();
		}

		public static IEnumerable<INode> Build(Document doc, IEnumerable<IHtmlNode> htmlElements)
		{
			return htmlElements.Select(x => BuildElem(doc, x));
		}

		private static INode BuildElem(Document doc, IHtmlNode htmlNode)
		{
			var comment = htmlNode as HtmlComment;
			if (comment != null)
				return doc.CreateComment(comment.Text);
			
			var txt = htmlNode as IHtmlText;
			if (txt != null)
				return doc.CreateTextNode(txt.Value);

			var htmlElement = htmlNode as IHtmlElement;
			if (htmlElement == null)
				return null;

			var elem = htmlElement.Name == "script" 
				? BuildScript(htmlElement) 
				: doc.CreateElement(htmlElement.Name);
			
			foreach (var attribute in htmlElement.Attributes)
			{
				elem.SetAttribute(attribute.Key, attribute.Value);
			}

			foreach(var child in htmlElement.Children)
			{
				var cn = BuildElem(doc, child);
				cn.Parent = elem;
				elem.AppendChild(cn);
			}

			return elem;
		}

		private static Element BuildScript(IHtmlElement htmlElement)
		{
			if (htmlElement.Attributes.Keys.Contains("src"))
			{
				var type = htmlElement.Attributes.ContainsKey("type") 
					?  htmlElement.Attributes["type"] 
					: "text/JavaScript"; //todo: get type from url


				return new LinkScript(type, htmlElement.Attributes["src"]);
			}
			else
			{
				var type = htmlElement.Attributes.ContainsKey("type") 
					?  htmlElement.Attributes["type"] 
					: "text/JavaScript";

				var htmlText = htmlElement.Children.FirstOrDefault() as IHtmlText;
				var text = htmlText != null ? htmlText.Value : string.Empty;
				return new EmbeddedScript(type, text);
			}
		}
	}
}
