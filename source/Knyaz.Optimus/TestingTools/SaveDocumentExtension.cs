using System;
using System.IO;
using System.Linq;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.TestingTools
{
	/// <summary>
	/// Contains methods for saving of html document. 
	/// </summary>
	public static class SaveDocumentExtension
	{
		/// <summary>
		/// Writes html document using specified <see cref="TextWriter"/>
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="writer"></param>
		public static void Save(this Document doc, TextWriter writer)
		{
			if(doc.DocumentElement.TagName != "HTML")
				throw new ArgumentOutOfRangeException("doc", "HTML document supported only.");

			if (doc.DocType != null)
			{
				writer.Write("<!DOCTYPE ");
				writer.Write(doc.DocType.Name);
				writer.Write(">");
			}
			
			WriteOpenTag(writer, doc.DocumentElement.TagName);
			WriteHead(writer, doc.Head);
			WriteElement(writer, doc.Body);
			WriteCloseTag(writer, doc.DocumentElement.TagName);
		}

		private static void WriteHead(TextWriter writer, Head docHead)
		{
			WriteOpenTag(writer, docHead.TagName);
			WriteStyles(writer, docHead.OwnerDocument);
			//todo: implement (metadata, etc)
			WriteCloseTag(writer, docHead.TagName);
		}

		private static void WriteStyles(TextWriter writer, Document document)
		{
			if (document.StyleSheets.SelectMany(x => x.CssRules).Any())
			{
				WriteOpenTag(writer, TagsNames.Style);
				foreach (var styleRule in document.StyleSheets.Skip(1).SelectMany(x => x.CssRules)) //skip default stylesheet
				{
					writer.Write(styleRule.CssText);
					//todo: complete
				}
				WriteCloseTag(writer, TagsNames.Style);
			}
		}

		private static void WriteElement(TextWriter writer, Element elt)
		{
			var attributes =
				string.Join(" ",
				elt.Attributes
				.Where(x => x.Name != "onload" && x.Name != "onerror" && x.Name != "onclick")
				.Select(x =>
					x.Value != null ? (x.Name+"=\""+x.Value+"\"") : (x.Name)));
			
			
			var tag = attributes.Length > 0 ? elt.TagName + " " + attributes : elt.TagName; 
			
			WriteOpenTag(writer, tag);
			foreach (var child in elt.ChildNodes)
			{
				if (child is Element element)
				{
					if(element.TagName != "SCRIPT" && element.TagName != "LINK")
						WriteElement(writer, element);	
				}
				else if(child is Text txt)
				{
					writer.Write(txt.Data);
				}
			}
			WriteCloseTag(writer, elt.TagName);
		}

		private static void WriteOpenTag(TextWriter writer, string tagName)
		{
			writer.Write('<');
			writer.Write(tagName);
			writer.Write('>');
		}
		
		private static void WriteCloseTag(TextWriter writer, string tagName)
		{
			writer.Write("</");
			writer.Write(tagName);
			writer.Write('>');
		}

		public static string Save(this Document doc)
		{
			using (var writer = new StringWriter())
			{
				doc.Save(writer);
				return writer.ToString();
			}
		}
	}
}