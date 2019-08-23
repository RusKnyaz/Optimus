using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents an HTML attribute of an element.
	/// </summary>
	[DomItem]
	public class Attr : Node
	{
		private string _name;
		private Element _ownerElement;

		internal Attr(Element owner, string name, string value)
		{
			_name = name;
			_ownerElement = owner;
			SetOwner(owner.OwnerDocument);
			Value = value;
		}

		internal Attr(string name, Document ownerDoc)
		{
			_name = name;
			SetOwner(ownerDoc);
		}

		/// <summary>
		/// Creates new copy of this <see cref="Attr"/> object.
		/// </summary>
		/// <param name="deep">Makes no sense.</param>
		/// <returns>Created <see cref="Attr"/>.</returns>
		public override Node CloneNode(bool deep) => new Attr(Name, OwnerDocument) { Value = Value };

		/// <summary>
		/// Equals this.Name.
		/// </summary>
		public override string NodeName => _name;

		/// <summary>
		/// The name of an attribute.
		/// </summary>
		public string Name => _name;

		/// <summary>
		/// The element useded to access the attribute.
		/// </summary>
		public Element OwnerElement => _ownerElement;

		internal void SetOwnerElement(Element element) => _ownerElement = element;

		/// <summary>
		/// Sets or gets the value of the attribute.
		/// </summary>
		public string Value { get; set; }

		//todo: is it right?
		/// <summary>
		/// <c>true</c> if the attribute is of type Id, otherwise it returns <c>false</c>.
		/// </summary>
		public bool IsId => _name == "id";

		//todo: is it right?
		/// <summary>
		/// <c>true</c> if the attribute has been specified, otherwise it returns <c>false</c>.
		/// </summary>
		public bool Specified => true;

		//todo: TypeInfo SchemaTypeInfo {get{}}
	}
}
