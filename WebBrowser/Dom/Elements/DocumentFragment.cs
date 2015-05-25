using WebBrowser.Dom.Elements;

namespace WebBrowser.Dom
{
	public class DocumentFragment : Element
	{
		public DocumentFragment(Document ownerDocument): base(ownerDocument)
		{
			NodeType = DOCUMENT_FRAGMENT_NODE;
		}

		public override string NodeName
		{
			get { return "#document-fragment";}
		}
	}
}