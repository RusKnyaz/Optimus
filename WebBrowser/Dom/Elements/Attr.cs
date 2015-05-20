namespace WebBrowser.Dom.Elements
{
	public class Attr : Node
	{
		private string _name;

		public Attr(Element owner, string name, string value)
		{
			_name = name;
			OwnerElement = owner;
			Value = value;
		}

		public override INode CloneNode()
		{
			return new Attr(null, Name, Value);
		}

		public override string NodeName
		{
			get { return _name; }
		}

		public string Name
		{
			get { return _name; }
		}

		public Element OwnerElement { get; private set; }
		public string Value { get; set; }

		//todo: is it right?
		public bool IsId { get { return _name == "id"; } }
		//todo: is it right?
		public bool Specified { get { return true; } }
		//todo: TypeInfo SchemaTypeInfo {get{}}
	}
}
