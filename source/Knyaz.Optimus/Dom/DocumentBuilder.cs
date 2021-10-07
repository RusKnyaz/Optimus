using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Html;
using HtmlElement = Knyaz.Optimus.Html.HtmlElement;

namespace Knyaz.Optimus.Dom
{
	//Build document using own parser
	internal static class DocumentBuilder
	{
		public static void Build(Node parentNode, string htmlString, NodeSources source = NodeSources.DocumentBuilder)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlString)))
			{
				var html = HtmlParser.Parse(stream);
				BuildInternal(parentNode, html, source, true);
			}
		}

		private static IEnumerable<IHtmlNode> ExpandHtmlTag(IEnumerable<IHtmlNode> parse)
		{
			foreach (var htmlNode in parse)
			{
				if (htmlNode is HtmlElement tag && tag.Name.ToLowerInvariant() == "html")
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

		public static void Build(Document document, Stream stream, NodeSources source = NodeSources.DocumentBuilder)
		{
			var html = HtmlParser.Parse(stream);
			Build(document, html, source);
		}

		public static void Build(Document document, IEnumerable<IHtmlNode> htmlElements, NodeSources source = NodeSources.DocumentBuilder) =>
			BuildInternal(document.DocumentElement, ExpandHtmlTag(htmlElements), source);

		private static IEnumerable<IHtmlNode> BuildInternal(
			Node node, 
			IEnumerable<IHtmlNode> htmlNodes, 
			NodeSources source = NodeSources.DocumentBuilder, 
			bool root = false)
		{
			var extruded = new List<IHtmlNode>();

			HtmlTableSectionElement currentTBody = null;
			
			foreach (var htmlNode in htmlNodes)
			{
				var currentNode = node;
				
				if (htmlNode is HtmlDocType)
				{
					//todo: may be it's wrong to assume the doctype element placed before html in source document
					//todo: fill doctype attributes.
					currentNode.OwnerDocument.InsertBefore(new DocType(), currentNode.OwnerDocument.DocumentElement);
				}

				if (htmlNode is HtmlComment comment)
				{
					currentNode.AppendChild(currentNode.OwnerDocument.CreateComment(comment.Text));
					continue;
				}
				
				var htmlElement = htmlNode as HtmlElement;

				var htmlHtmlElt = currentNode as HtmlHtmlElement;
				if (htmlHtmlElt != null && (htmlElement == null || (
					                            !htmlElement.Name.Equals(TagsNames.Body,
						                            StringComparison.InvariantCultureIgnoreCase) &&
					                            !htmlElement.Name.Equals(TagsNames.Head,
						                            StringComparison.InvariantCultureIgnoreCase))))
				{
					currentNode = currentNode.OwnerDocument.Body;
				}
				
				if (htmlNode is HtmlText txt)
				{
					var c = currentNode.OwnerDocument.CreateTextNode(txt.Value);
					c.Source = source;
					currentNode.AppendChild(c);
					continue;
				}
				
				//skip child handling for nodes that not accepted children.
				if (currentNode is HtmlScriptElement)
					continue;

				if (currentNode is HtmlOptionElement option)
				{
					var innerText = htmlNode.ExtractText();
					option.Text += innerText;
					continue;
				}

				if (htmlElement == null)
					continue;


				if (htmlHtmlElt != null)
				{
					var invariantName = htmlElement.Name.ToUpperInvariant();
					if (invariantName == "HEAD" || invariantName == "BODY")
					{
						var headOrBody = htmlHtmlElt.GetElementsByTagName(invariantName).FirstOrDefault();
						headOrBody.Source = source;
						SetAttributes(htmlElement, headOrBody);
						BuildInternal(headOrBody, htmlElement.Children, source);
						continue;
					}
				}

				//if parent is table
				if (currentNode is HtmlTableElement table)
				{
					var elementInvariantName = htmlElement.Name.ToUpperInvariant();
					if (elementInvariantName == TagsNames.Tr)
					{
						if (currentTBody == null)
						{
							currentTBody = (HtmlTableSectionElement)currentNode.OwnerDocument.CreateElement(TagsNames.TBody);
							currentTBody.Source = source;
							table.AppendChild(currentTBody);
						}
						currentNode = currentTBody;
					}
					else if (elementInvariantName == TagsNames.Col)
					{
						var colgroup = currentNode.OwnerDocument.CreateElement(TagsNames.Colgroup);
						table.AppendChild(colgroup);
						currentNode = colgroup;
					}
					else if(elementInvariantName == TagsNames.TBody)
					{
						currentTBody = null;
					}
					else if (!root &&
					         elementInvariantName != TagsNames.TBody &&
					         elementInvariantName != TagsNames.Tr &&
					         elementInvariantName != TagsNames.Caption &&
					         elementInvariantName != TagsNames.THead &&
					         elementInvariantName != TagsNames.TFoot &&
					         elementInvariantName != TagsNames.Colgroup &&
					         elementInvariantName != TagsNames.Col)
					{
						extruded.Add(htmlElement);
						continue;
					}
				}

				var elem = currentNode.OwnerDocument.CreateElement(htmlElement.Name);
				elem.Source = source;
				SetAttributes(htmlElement, elem);

				var extrudedNodes = BuildInternal(elem, htmlElement.Children, source);
				BuildInternal(currentNode, extrudedNodes, source);

				currentNode.AppendChild(elem);
			}
			return extruded;
		}

		private static void SetAttributes(HtmlElement htmlElement, Element elem)
		{
			foreach (var attribute in htmlElement.Attributes)
			{
				elem.SetAttribute(attribute.Key, attribute.Value ?? string.Empty);

				if (attribute.Key == "selected" && elem is HtmlOptionElement option)
				{
					option.DefaultSelected = true;
				}
			}
		}
	}
	
	static class IHtmlNodeExtension
	{
		public static string ExtractText(this IHtmlNode node) =>
			node is HtmlText text ? text.Value
			: node is HtmlElement elt ? string.Join("", elt.Children.Select(ExtractText))
			: string.Empty;
	}
}
