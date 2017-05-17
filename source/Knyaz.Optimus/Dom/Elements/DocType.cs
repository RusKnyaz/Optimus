namespace Knyaz.Optimus.Dom.Elements
{
	public sealed class DocType : Node
	{
		public DocType()
		{
			NodeType = DOCUMENT_TYPE_NODE;
		}

		public override Node CloneNode(bool deep)
		{
			return new DocType();
		}

		public override string NodeName
		{
			get { return "html"; }
		}

		public override string ToString()
		{
			return "<!DOCTYPE html>";
		}
	}
}
