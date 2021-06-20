namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents a minimal document object that has no parent.
	/// </summary>
	public sealed class DocumentFragment : Element
	{
		internal DocumentFragment(HtmlDocument ownerDocument): base(ownerDocument)
			=>	NodeType = DOCUMENT_FRAGMENT_NODE;
		
		/// <summary>
		/// Always "#document-fragment".
		/// </summary>
		public override string NodeName => "#document-fragment";

		/// <summary>
		/// Creates a new copy of the DocumentFragment.
		/// </summary>
		/// <param name="deep">Specifies whether or not the child nodes should be cloned too.</param>
		/// <returns></returns>
		public override Node CloneNode(bool deep = false)
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