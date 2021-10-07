using System;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom
{
	[JsName("HTMLDocument")]
	public class HtmlDocument : Document
	{
		internal HtmlDocument(IWindow window = null): base(window){}
			
		internal HtmlDocument(string namespaceUri, string qualifiedNameStr, DocType docType, IWindow window) 
			: base(namespaceUri, qualifiedNameStr, docType, window)
		{
			
		}

		private protected override void Initialize()
		{
			DocumentElement.AppendChild(Head = (Head)CreateElement(TagsNames.Head));
			DocumentElement.AppendChild(Body = (HtmlBodyElement)CreateElement(TagsNames.Body));
		}
		
		
		/// <summary> Creates an Element node. </summary>
		/// <param name="tagName">The tag name of element to be created.</param>
		public override Element CreateElement(string tagName)
		{
			if(tagName == null)
				throw new ArgumentNullException(nameof(tagName));
			if(tagName == string.Empty)
				throw new ArgumentOutOfRangeException(nameof(tagName));

			var invariantTagName = tagName.ToUpperInvariant();
			switch (invariantTagName)
			{
				//todo: fill the list
				case TagsNames.A: return new HtmlAnchorElement(this);
				case TagsNames.Area: return new HtmlAreaElement(this);
				case TagsNames.Br: return new HtmlBrElement(this);
				case TagsNames.TFoot:
				case TagsNames.THead:
				case TagsNames.TBody:
					return new HtmlTableSectionElement(this, invariantTagName);
				case TagsNames.Td:
				case TagsNames.Th:
					return new HtmlTableCellElement(this, invariantTagName);
				case TagsNames.Caption: return new HtmlTableCaptionElement(this);
				case TagsNames.Table: return new HtmlTableElement(this);
				case TagsNames.Tr: return new HtmlTableRowElement(this);
				case TagsNames.Link: return new HtmlLinkElement(this);
				case TagsNames.Style: return new HtmlStyleElement(this);
				case TagsNames.Select: return new HtmlSelectElement(this);
				case TagsNames.Option: return new HtmlOptionElement(this);
				case TagsNames.Div: return new HtmlDivElement(this);
				case TagsNames.Span:
				case TagsNames.Nav:
				case TagsNames.Bold: return new HtmlElement(this, invariantTagName);
				case TagsNames.Button: return new HtmlButtonElement(this);
				case TagsNames.Input: return new HtmlInputElement(this);
				case TagsNames.Script: return new HtmlScriptElement(this);
				case TagsNames.Head:return new Head(this);
				case TagsNames.Body:return new HtmlBodyElement(this);
				case TagsNames.Textarea: return new HtmlTextAreaElement(this);
				case TagsNames.Title: return new HtmlTitleElement(this);
				case TagsNames.Form:return new HtmlFormElement(this);
				case TagsNames.IFrame:return new HtmlIFrameElement(this);
				case TagsNames.Html:return new HtmlHtmlElement(this);
				case TagsNames.Col: return new HtmlTableColElement(this);
				case TagsNames.Label: return new HtmlLabelElement(this);
				case TagsNames.OptGroup: return new HtmlOptGroupElement(this);
				case TagsNames.Img: return new HtmlImageElement(this, GetImage);
				case TagsNames.Embed: return new HtmlEmbedElement(this);
			}

			return new HtmlUnknownElement(this, invariantTagName);
		}
	}
}