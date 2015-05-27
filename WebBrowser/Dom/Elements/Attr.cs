using System;
using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom.Elements
{
	public class Attr : Node, IAttr
	{
		private string _name;
		private Element _ownerElement;

		public Attr(Element owner, string name, string value)
		{
			_name = name;
			_ownerElement = owner;
			Value = value;
		}

		public Attr(string name)
		{
			_name = name;
		}

		public override Node CloneNode(bool deep)
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

		public Element OwnerElement
		{
			get { return _ownerElement; }
		}

		internal void SetOwnerElement(Element element)
		{
			_ownerElement = element;
		}

		public string Value { get; set; }

		//todo: is it right?
		public bool IsId { get { return _name == "id"; } }
		//todo: is it right?
		public bool Specified { get { return true; } }
		//todo: TypeInfo SchemaTypeInfo {get{}}
	}

	[DomItem]
	public interface IAttr
	{
		string Value { get; set; }
		string Name { get; }
		bool IsId { get; }
		bool Specified { get; }
	}
}
