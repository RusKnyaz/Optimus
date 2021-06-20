using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Css
{
	/// <summary>
	/// Loads css, applies computed style for nodes
	/// </summary>
	internal class DocumentStyling : IDisposable
	{
		private readonly HtmlDocument _document;
		private readonly Func<string, Task<IResource>> _getResourceAsyncFn;
		public int Version = 0;

		public DocumentStyling(
			HtmlDocument document,
			CssStyleSheet defaultStyleSheet,
			Func<string, Task<IResource>> getResourceAsyncFn)
		{
			_document = document;
			_userAgentStyleSheet = defaultStyleSheet;
			_getResourceAsyncFn = getResourceAsyncFn;
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
			if (node is Text txt && node.ParentNode is HtmlStyleElement styleElt && !string.IsNullOrWhiteSpace(txt.Data))
			{
				//todo: check type
				var type = !string.IsNullOrEmpty(styleElt.Type) ? styleElt.Type : "text/css";
				var media = !string.IsNullOrEmpty(styleElt.Media) ? styleElt.Type : "all";
				AddStyleToDocument(new StringReader(txt.Data));
			}

			if (node is HtmlLinkElement linkElt && linkElt.Rel == "stylesheet" && _getResourceAsyncFn != null)
			{
				//todo: check type
				var task = _getResourceAsyncFn(linkElt.Href);
				task.Wait();
				using (var reader = new StreamReader(task.Result.Stream))
					AddStyleToDocument(reader);
			}
		}

		//Get imported css
		private TextReader GetImport(string url)
		{
			var task = _getResourceAsyncFn(url);
			task.Wait();
			var stream = task.Result.Stream;
			return new StreamReader(stream);
		}

		private void AddStyleToDocument(TextReader content)
		{
			var styleSheet = StyleSheetBuilder.CreateStyleSheet(content, GetImport);
			_document.StyleSheets.Add(styleSheet);
		}

		private Dictionary<IElement, ICssStyleDeclaration> _cache = new Dictionary<IElement, ICssStyleDeclaration>();
		/// <summary>
		/// Default styles.
		/// </summary>
		private CssStyleSheet _userAgentStyleSheet;

		/// <summary>
		/// Gives the values of all the CSS properties of an element after applying the active stylesheets and resolving any basic computation those values may contain.
		/// </summary>
		/// <param name="elt">The <see cref="Element"/> for which to get the computed style.</param>
		/// <returns>The <see cref="ICssStyleDeclaration"/> containing all element's computed properties.</returns>
		public ICssStyleDeclaration GetComputedStyle(IElement elt)
		{
			if (_cache.ContainsKey(elt))
				return _cache[elt];

			return _cache[elt] = new ComputedCssStyleDeclaration(_userAgentStyleSheet, elt, () => Version);
		}
	}
}