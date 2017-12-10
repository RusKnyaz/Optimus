namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents a minimal document object that has no parent.
	/// </summary>
	public class DocumentFragment : Element
	{
		internal DocumentFragment(Document ownerDocument): base(ownerDocument)
			=>	NodeType = DOCUMENT_FRAGMENT_NODE;
		
		public override string NodeName => "#document-fragment";
	}
}