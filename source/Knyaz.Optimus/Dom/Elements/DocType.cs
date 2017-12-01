namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents the doctype element of the DOM.
	/// </summary>
	public sealed class DocType : Node
	{
		public DocType() => NodeType = DOCUMENT_TYPE_NODE;

		public override Node CloneNode(bool deep) => new DocType();

		public override string NodeName => "html";

		public override string ToString() => "<!DOCTYPE html>";
	}
}
