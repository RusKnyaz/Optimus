using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Html;
using Knyaz.Optimus.Properties;
using Knyaz.Optimus.ResourceProviders;

namespace Knyaz.Optimus.Dom.Css
{
	/// <summary>
	/// Loads css, applies computed style for nodes
	/// </summary>
	internal class DocumentStyling : IDisposable
	{
		private readonly Document _document;
		private readonly IResourceProvider _resourceProvider;

		public DocumentStyling(Document document, IResourceProvider resourceProvider)
		{
			_document = document;
			_resourceProvider = resourceProvider;
			document.NodeInserted += OnNodeInserted;
		}

		public void Dispose()
		{
			_document.NodeInserted -= OnNodeInserted;
		}

		private void OnNodeInserted(Node obj)
		{
			var styleElt = obj as HtmlStyleElement;
			if (styleElt != null)
			{
				var content = styleElt.InnerHTML;
				var type = !string.IsNullOrEmpty(styleElt.Type) ? styleElt.Type : "text/css";
				var media = !string.IsNullOrEmpty(styleElt.Media) ? styleElt.Type : "all";
				AddStyleToDocument(new StringReader(content));
			}

			var linkElt = obj as HtmlLinkElement;
			if (linkElt != null && linkElt.Rel == "stylesheet" && _resourceProvider!=null)
			{
				var request = _resourceProvider.CreateRequest(linkElt.Href);
				var task = _resourceProvider.GetResourceAsync(request);
				task.Wait();
				using(var reader = new StreamReader(task.Result.Stream))
					AddStyleToDocument(reader);
			}
		}

		public void LoadDefaultStyles()
		{
			using (var reader = new StringReader(Resources.moz_default))
				AddStyleToDocument(reader);
		}

		private void AddStyleToDocument(TextReader content)
		{
			var styleSheet = StyleSheetBuilder.CreateStyleSheet(content);
			_document.StyleSheets.Add(styleSheet);
		}

		public CssStyleDeclaration GetComputedStyle(Element elt)
		{
			//todo: waiting for loading all deferred styles and compute element style

			return new CssStyleDeclaration();
		}
	}

	class StyleSheetBuilder
	{
		public static CssStyleSheet CreateStyleSheet(TextReader reader)
		{
			var styleSheet = new CssStyleSheet();
			var enumerator = CssReader.Read(reader).GetEnumerator();
			if(!enumerator.MoveNext())
				throw new Exception("Unable to parse rule");
			CssStyleRule rule;
			while (CreateRule(styleSheet, enumerator, out rule))
			{
				styleSheet.CssRules.Add(rule);
			}

			return styleSheet;
		}

		public static bool CreateRule(CssStyleSheet styleSheet, IEnumerator<CssChunk> enumerator, out CssStyleRule rule)
		{
			if (enumerator.Current.Type != CssChunkTypes.Selector)
				throw new Exception("Unable to parse rule");

			rule = new CssStyleRule(styleSheet) { SelectorText = enumerator.Current.Data };
			enumerator.MoveNext();
			return FillStyle(rule.Style, enumerator);
		}

		public static void FillStyle(CssStyleDeclaration style, string str)
		{
			if (str[0] != '{')
				str = '{' + str;
			str = "toskip" + str;

			if (str.Last() != '}')
				str += '}';

			using (var enumerator = CssReader.Read(new StringReader(str)).GetEnumerator())
			{
				//skip selector
				while (enumerator.MoveNext() && enumerator.Current.Type == CssChunkTypes.Selector) ;

				FillStyle(style, enumerator);
			}
		}

		private static bool FillStyle(CssStyleDeclaration style, IEnumerator<CssChunk> enumerator)
		{
			string property = null;
			do
			{
				var cssChunk = enumerator.Current;
				switch (cssChunk.Type)
				{
					case CssChunkTypes.Selector:
						return true;
					case CssChunkTypes.Property:
						property = cssChunk.Data;
						break;
					case CssChunkTypes.Value:
						style.SetProperty(property, cssChunk.Data, "");
						property = null;
						break;
				}
			} while (enumerator.MoveNext());
			return false;
		}
	}
}