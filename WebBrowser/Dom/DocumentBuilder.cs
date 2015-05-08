using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebBrowser.Dom.Elements;
using WebBrowser.Html;
using HtmlElement = WebBrowser.Dom.Elements.HtmlElement;

namespace WebBrowser.Dom
{
	internal class DocumentBuilder
	{
		public static IEnumerable<INode> Build(string htmlString)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlString)))
			{
				return Build(stream).ToList();
			}
		}

		public static IEnumerable<INode> Build(Stream htmlStream)
		{
			var html = HtmlParser.Parse(htmlStream);
			return Build(html);
		}

		public static IEnumerable<INode> Build(IEnumerable<IHtmlNode> htmlElements)
		{
			return htmlElements.Select(BuildElem);
		}

		private static INode BuildElem(IHtmlNode htmlNode)
		{
			var txt = htmlNode as IHtmlText;
			if (txt != null)
			{
				return new Text(){Data = txt.Value};
			}

			var htmlElement = htmlNode as IHtmlElement;
			if (htmlElement == null)
				return null;

			var id = htmlElement.Attributes.ContainsKey("id") ? htmlElement.Attributes["id"] : string.Empty;

			if (htmlElement.Name == "script")
			{
				var x = BuildScript(htmlElement);
				x.Id = id;
				return x;
			}

			Element elem = null;

			if (htmlElement.Name == "input")
			{
				elem = new HtmlInputElement
				{
					Type = htmlElement.Attributes.ContainsKey("type") ? htmlElement.Attributes["type"] : "text",
					Value = htmlElement.Attributes.ContainsKey("value") ? htmlElement.Attributes["value"] : null,
					Checked = htmlElement.Attributes.ContainsKey("checked") && htmlElement.Attributes["checked"] == "true"
				};
			}
			else if (htmlElement.Name == "span" || htmlElement.Name == "div")
			{
				elem = new HtmlElement(htmlElement.Name);
			}
			else
			{
				elem = new Element(htmlElement.Name) { Id = id };	
			}

			if (elem is HtmlElement)
			{
				var hElem = elem as HtmlElement;
				hElem.Hidden = htmlElement.Attributes.ContainsKey("hidden") && htmlElement.Attributes["hidden"] == "true";
			}

			elem.Id = id;

			foreach (var attribute in htmlElement.Attributes)
			{
				elem.Attributes.Add(attribute.Key, attribute.Value);
			}

			foreach(var child in htmlElement.Children)
			{
				var cn = BuildElem(child);
				cn.Parent = elem;
				elem.ChildNodes.Add(cn);
			}

			return elem;
		}

		private static IScript BuildScript(IHtmlElement htmlElement)
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

				return new EmbeddedScript(type, ((IHtmlText)htmlElement.Children.Single()).Value);
			}
		}
	}

}
