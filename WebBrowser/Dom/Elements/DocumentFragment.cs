namespace WebBrowser.Dom
{
	public class DocumentFragment : Element
	{
		public DocumentFragment()
		{
			NodeType = DOCUMENT_FRAGMENT_NODE;
		}

		public override string NodeName
		{
			get { return "#document-fragment";}
		}
	}
}