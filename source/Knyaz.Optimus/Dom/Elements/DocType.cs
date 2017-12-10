using System;

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

		public override Node CloneNode(bool deep) => new DocType();

		public override string NodeName => "html";

		public override string ToString() => "<!DOCTYPE html>";

		/// <summary>
		/// A NamedNodeMap of entities declared in the DTD.Every node in this map implements the Entity interface.
		/// </summary>
		public object Entities => throw new NotImplementedException();

		/// <summary>
		/// A string of the internal subset, or null if there is none.Eg "<!ELEMENT foo (bar)>".
		/// </summary>
		public string InternalSubset => throw new NotImplementedException();

		/// <summary>
		/// A DOMString, eg "html" for <!DOCTYPE HTML>.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// A NamedNodeMap with notations declared in the DTD.Every node in this map implements the Notation interface.
		/// </summary>
		public object Notations => throw new NotImplementedException();

		/// <summary>
		/// A string, eg "-//W3C//DTD HTML 4.01//EN", empty string for HTML5.
		/// </summary>
		public string PublicId { get; }

		/// <summary>
		/// A string, eg "http://www.w3.org/TR/html4/strict.dtd", empty string for HTML5.
		/// </summary>
		public string SystemId { get; }
	}
}
