using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebBrowser.Dom.Elements;
using WebBrowser.Html;

namespace WebBrowser.Dom
{
	//todo: chose one

	//Build document parsed by HtmlAgilityPack
	/*internal class DocumentBuilder
	{
		public static void Build(Node parentNode, string htmlString, NodeSources source = NodeSources.DocumentBuilder)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlString)))
			{
				var html = Parse(stream);
				Build(parentNode, html);
			}
		}

		private static IEnumerable<HtmlNode> Parse(Stream stream)
		{
			var doc = new HtmlDocument();
			doc.Load(stream);

			return doc.DocumentNode.ChildNodes;
			//return HtmlParser.Parse(stream);
		}

		public static void Build(Document parentNode, Stream stream)
		{
			var html = ExpandHtmlTag(Parse(stream));
			Build(parentNode.DocumentElement, html);
		}

		private static IEnumerable<HtmlNode> ExpandHtmlTag(IEnumerable<HtmlNode> parse)
		{
			foreach (var htmlNode in parse)
			{
				if (htmlNode != null && htmlNode.Name.ToLowerInvariant() == "html")
				{
					foreach (var child in htmlNode.ChildNodes)
					{
						yield return child;
					}
				}
				else
				{
					yield return htmlNode;
				}
			}
		}

		private static void Build(Node parentNode, IEnumerable<HtmlNode> htmlElements)
		{
			foreach (var htmlElement in htmlElements)
			{
				BuildElem(parentNode, htmlElement);
			}
		}

		private static void BuildElem(Node node, HtmlNode htmlNode)
		{
			var comment = htmlNode as HtmlCommentNode;
			if (comment != null)
			{
				node.AppendChild(node.OwnerDocument.CreateComment(comment.Comment));
				return;
			}

			var txt = htmlNode as HtmlTextNode;
			if (txt != null)
			{
				var c = node.OwnerDocument.CreateTextNode(txt.Text);
				c.Source = NodeSources.DocumentBuilder;
				node.AppendChild(c);
				return;
			}

			if (!string.IsNullOrEmpty(htmlNode.Name))
			{
				var elem = node.OwnerDocument.CreateElement(htmlNode.Name);
				elem.Source = NodeSources.DocumentBuilder;

				if (elem is Script)
				{
					var htmlText = htmlNode.ChildNodes.FirstOrDefault() as HtmlTextNode;
					elem.InnerHTML = htmlText != null ? htmlText.Text : string.Empty;
				}

				foreach (var attribute in htmlNode.Attributes)
				{
					elem.SetAttribute(attribute.Name, attribute.Value);
				}

				node.AppendChild(elem);

				Build(elem, htmlNode.ChildNodes);
			}
			else
			{
				Build(node, htmlNode.ChildNodes);
			}
		}
	}*/

	//Build document using own parser
	internal class DocumentBuilder
	{
		public static void Build(Node parentNode, string htmlString, NodeSources source = NodeSources.DocumentBuilder)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlString)))
			{
				var html = HtmlParser.Parse(stream);
				Build(parentNode, html, source);
			}
		}

		public static void Build(Document parentNode, Stream stream)
		{
			var html = ExpandHtmlTag(HtmlParser.Parse(stream));
			Build(parentNode.DocumentElement, html);
		}

		private static IEnumerable<IHtmlNode> ExpandHtmlTag(IEnumerable<IHtmlNode> parse)
		{
			foreach (var htmlNode in parse)
			{
				var tag = htmlNode as Html.HtmlElement;
				if (tag != null && tag.Name.ToLowerInvariant() == "html")
				{
					foreach (var child in tag.Children)
					{
						yield return child;
					}
				}
				else
				{
					yield return htmlNode;
				}
			}
		}

		private static void Build(Node parentNode, IEnumerable<IHtmlNode> htmlElements, NodeSources source = NodeSources.DocumentBuilder)
		{
			foreach (var htmlElement in htmlElements)
			{
				BuildElem(parentNode, htmlElement, source);
			}
		}

		private static void BuildElem(Node node, IHtmlNode htmlNode, NodeSources source)
		{
			var docType = htmlNode as HtmlDocType;
			if (docType != null)
			{
				//todo: may be it's wrong to assume the doctype element placed before html in source document
				node.OwnerDocument.InsertBefore(new DocType(), node.OwnerDocument.DocumentElement);
			}
			
			var comment = htmlNode as HtmlComment;
			if (comment != null)
			{
				node.AppendChild(node.OwnerDocument.CreateComment(comment.Text));
				return;
			}

			var txt = htmlNode as IHtmlText;
			if (txt != null)
			{
				var c = node.OwnerDocument.CreateTextNode(txt.Value);
				c.Source = source;
				node.AppendChild(c);
				return;
			}

			var htmlElement = htmlNode as Html.IHtmlElement;
			if (htmlElement == null)
				return;

			var elem = node.OwnerDocument.CreateElement(htmlElement.Name);
			elem.Source = source;
			
			if (elem is Script)
			{
				var htmlText = htmlElement.Children.FirstOrDefault() as IHtmlText;
				elem.InnerHTML = htmlText != null ? htmlText.Value : string.Empty;
			}
			
			foreach (var attribute in htmlElement.Attributes)
			{
				elem.SetAttribute(attribute.Key, attribute.Value);
			}

			node.AppendChild(elem);

			Build(elem, htmlElement.Children, source);
		}
	}
}
