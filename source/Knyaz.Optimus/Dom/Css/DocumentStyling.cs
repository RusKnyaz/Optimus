using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Properties;
using Knyaz.Optimus.ResourceProviders;
using HtmlElement = Knyaz.Optimus.Dom.Elements.HtmlElement;

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

		public ICssStyleDeclaration GetComputedStyle(Element elt)
		{
			var styles = GetStylesFor(elt);
			return new ComputedCssStyleDeclaration(styles);
		}

		private ICssStyleDeclaration[] GetStylesFor(Element elt)
		{
			var lst = new List<ICssStyleDeclaration>();
			
			foreach (var result in _document.StyleSheets.SelectMany(x => x.CssRules).OfType<CssStyleRule>())
			{
				if(result.IsMatchesSelector(elt))
					lst.Add(result.Style);
			}
			var htmlElt = elt as HtmlElement;
			if (htmlElt != null)
				lst.Add(htmlElt.Style);

			lst.Reverse();

			return lst.ToArray();
		}
	}
}