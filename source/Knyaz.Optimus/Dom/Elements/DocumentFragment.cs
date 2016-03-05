namespace Knyaz.Optimus.Dom.Elements
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