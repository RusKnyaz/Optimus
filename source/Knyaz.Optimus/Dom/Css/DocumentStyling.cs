using System;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.Dom.Css
{
	/// <summary>
	/// Loads css, applies computed style for nodes
	/// </summary>
	internal class DocumentStyling : IDisposable
	{
		private readonly Document _document;

		public DocumentStyling(Document document)
		{
			_document = document;
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
			}

			var linkElt = obj as HtmlLinkElement;
			if (linkElt != null && linkElt.Rel == "stylesheet")
			{
				//todo: enqueue styles to load.
			}
		}

		public CssStyleDeclaration GetComputedStyle(Element elt)
		{
			//todo: waiting for loading all deferred styles and compute element style

			return new CssStyleDeclaration();
		}
	}
}