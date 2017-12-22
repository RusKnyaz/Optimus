namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents the doctype element of the DOM.
	/// </summary>
	public sealed class DocType : Node
	{
		internal DocType() => NodeType = DOCUMENT_TYPE_NODE;

		internal DocType(string name, string publicId, string systemId)
		{
			Name = name;
			PublicId = publicId;
			SystemId = systemId;
		}

		/// <summary>
		/// Creates a copy of this DocType object.
		/// </summary>
		public override Node CloneNode(bool deep) => new DocType();

		/// <summary>
		/// The same as <see cref="Name"/>.
		/// </summary>
		public override string NodeName => Name;

		/// <summary>
		/// Always [Object DocumentType] for the DocType element.
		/// </summary>
		public override string ToString() => "[Object DocumentType]";

		/// <summary>
		/// A DOMString, eg "html" for &lt;!DOCTYPE HTML&gt;.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// A string, eg "-//W3C//DTD HTML 4.01//EN", empty string for HTML5.
		/// </summary>
		public string PublicId { get; }

		/// <summary>
		/// A string, eg "http://www.w3.org/TR/html4/strict.dtd", empty string for HTML5.
		/// </summary>
		public string SystemId { get; }

		/// <summary>
		/// Removes this doctype from parent document.
		/// </summary>
		public void Remove() => ParentNode?.RemoveChild(this);
	}
}
