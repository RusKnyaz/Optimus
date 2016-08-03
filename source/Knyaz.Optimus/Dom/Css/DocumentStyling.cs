using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.Dom.Css
{
	/// <summary>
	/// Loads css, applies computed style for nodes
	/// </summary>
	internal class DocumentStyling
	{
		public DocumentStyling(Document document)
		{
			document.NodeInserted += OnNodeInserted;
		}

		private void OnNodeInserted(Node obj)
		{
			var styleElt = obj as HtmlStyleElement;
			if (styleElt == null)
				return;
		}
	}
}