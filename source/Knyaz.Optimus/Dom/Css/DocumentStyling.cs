using System;
using System.Collections.Generic;
using System.IO;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.Resources;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Css
{
	/// <summary>
	/// Loads css, applies computed style for nodes
	/// </summary>
	internal class DocumentStyling : IDisposable
	{
		private readonly Document _document;
		private readonly IResourceProvider _resourceProvider;

		public int Version = 0;

		public DocumentStyling(Document document, IResourceProvider resourceProvider)
		{
			_document = document;
			_resourceProvider = resourceProvider;
			document.NodeInserted += OnNodeInserted;
			_document.StyleSheets.Changed += OnStyleChanged;
		}

		private void OnStyleChanged()
		{
			Version++;
		}

		public void Dispose()
		{
			_document.NodeInserted -= OnNodeInserted;
			_document.StyleSheets.Changed -= OnStyleChanged;
		}

		private void OnNodeInserted(Node obj)
		{
		    foreach (var node in obj.Flatten())
		    {
		        HandleNode(node);
		    }
		}

	    private void HandleNode(INode node)
	    {
            var txt = node as Text;
            if (txt != null)
            {
                var styleElt = node.ParentNode as HtmlStyleElement;
                if (styleElt != null && !string.IsNullOrWhiteSpace(txt.Data))
                {
                    //todo: chech type
                    var type = !string.IsNullOrEmpty(styleElt.Type) ? styleElt.Type : "text/css";
                    var media = !string.IsNullOrEmpty(styleElt.Media) ? styleElt.Type : "all";
                    AddStyleToDocument(new StringReader(txt.Data));
                }
            }

            var linkElt = node as HtmlLinkElement;
            if (linkElt != null && linkElt.Rel == "stylesheet" && _resourceProvider != null)
            {
                //todo: check type
                var request = _resourceProvider.CreateRequest(linkElt.Href);
                var task = _resourceProvider.GetResourceAsync(request);
                task.Wait();
                using (var reader = new StreamReader(task.Result.Stream))
                    AddStyleToDocument(reader);
            }
        }

		//Get imported css
		private TextReader GetImport(string url)
		{
			var req = _resourceProvider.CreateRequest(url);
			var task = _resourceProvider.GetResourceAsync(req);
			task.Wait();
			var stream = task.Result.Stream;
			return new StreamReader(stream);
		}

		public void LoadDefaultStyles()
		{
			using (var stream = GetType().Assembly.GetManifestResourceStream("Knyaz.Optimus.Resources.moz_default.css"))
			using (var reader = new StreamReader(stream))
				AddStyleToDocument(reader);
		}

		private void AddStyleToDocument(TextReader content)
		{
			var styleSheet = StyleSheetBuilder.CreateStyleSheet(content, GetImport);
			_document.StyleSheets.Add(styleSheet);
		}

		private Dictionary<IElement, ICssStyleDeclaration> _cache = new Dictionary<IElement, ICssStyleDeclaration>();

		public ICssStyleDeclaration GetComputedStyle(IElement elt)
		{
			if (_cache.ContainsKey(elt))
				return _cache[elt];

			return _cache[elt] = new ComputedCssStyleDeclaration(elt, () => Version);
		}
	}
}