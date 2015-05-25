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
		public static IEnumerable<Node> Build(Document doc, string htmlString)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlString)))
			{
				return Build(doc, stream);
			}
		}

		public static IEnumerable<Node> Build(Document doc, Stream stream)
		{
			var html = HtmlParser.Parse(stream);
			return Build(doc, html).ToList();
		}

		public static IEnumerable<Node> Build(Document doc, IEnumerable<IHtmlNode> htmlElements)
		{
			return htmlElements.Select(x => BuildElem(doc, x));
		}

		private static Node BuildElem(Document doc, IHtmlNode htmlNode)
		{
			var comment = htmlNode as HtmlComment;
			if (comment != null)
				return doc.CreateComment(comment.Text);
			
			var txt = htmlNode as IHtmlText;
			if (txt != null)
				return doc.CreateTextNode(txt.Value);

			var htmlElement = htmlNode as Html.IHtmlElement;
			if (htmlElement == null)
				return null;

			var elem = doc.CreateElement(htmlElement.Name);

			if (elem is Script)
			{
				var htmlText = htmlElement.Children.FirstOrDefault() as IHtmlText;
				elem.InnerHtml = htmlText != null ? htmlText.Value : string.Empty;
			}
			
			foreach (var attribute in htmlElement.Attributes)
			{
				elem.SetAttribute(attribute.Key, attribute.Value);
			}

			foreach(var child in htmlElement.Children)
			{
				var cn = BuildElem(doc, child);
				cn.ParentNode = elem;
				elem.AppendChild(cn);
			}

			return elem;
		}

		
	}
}
