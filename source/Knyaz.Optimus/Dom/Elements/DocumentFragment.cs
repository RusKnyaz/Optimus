namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents a minimal document object that has no parent.
	/// </summary>
	public sealed class DocumentFragment : Element
	{
		internal DocumentFragment(Document ownerDocument): base(ownerDocument)
			=>	NodeType = DOCUMENT_FRAGMENT_NODE;
		
		public override string NodeName => "#document-fragment";

		public override Node CloneNode(bool deep)
		{
			var node = OwnerDocument.CreateDocumentFragment();
			if (deep)
			{
				foreach (var childNode in ChildNodes)
				{
					node.AppendChild(childNode.CloneNode(true));
				}
			}
			return node;
		}
	}
}