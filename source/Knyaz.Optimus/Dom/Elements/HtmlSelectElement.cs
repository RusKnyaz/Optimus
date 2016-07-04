using System.Linq;

namespace Knyaz.Optimus.Dom.Elements
{
	public class HtmlSelectElement : HtmlElement
	{
		public HtmlSelectElement(Document ownerDocument) : base(ownerDocument, TagsNames.Select)
		{
			Options = new HtmlOptionsCollection(this);
		}

		public override Node AppendChild(Node node)
		{
			if (!(node is HtmlOptionElement))
				return node;

			return base.AppendChild(node);
		}

		public void Remove(int index)
		{
			var options = ChildNodes.ToArray();
			if (index >= 0 && index < options.Length)
			{
				RemoveChild(options[index]);
			}
		}

		public int Length { get { return ChildNodes.Count; } }

		public HtmlOptionsCollection Options { get; private set; }
	}

	public class HtmlOptionsCollection
	{
		private readonly HtmlSelectElement _owner;

		public HtmlOptionsCollection(HtmlSelectElement owner)
		{
			_owner = owner;
		}

		public int Length
		{
			get
			{
				return _owner.Length;
			}
			set
			{
				while (value > _owner.Length)
					_owner.ChildNodes.Add(_owner.OwnerDocument.CreateElement("option"));

				while (value < _owner.Length)
					_owner.Remove(_owner.Length - 1);
			}	
		}

		public HtmlOptionElement Item(int index)
		{
			return (HtmlOptionElement)_owner.ChildNodes[index];
		}

		public HtmlOptionElement NamedItem(string name)
		{
			return _owner.ChildNodes.OfType<HtmlOptionElement>().FirstOrDefault(x => x.Id == name)
			       ?? _owner.ChildNodes.OfType<HtmlOptionElement>().FirstOrDefault(x => x.Name == name);
		}

		[System.Runtime.CompilerServices.IndexerName("_Item")]
		public HtmlOptionElement this[object key]
		{
			get
			{
				var strKey = key.ToString();
				int idx;
				return int.TryParse(strKey, out idx) ? Item(idx) : NamedItem(strKey);
			}
		}
	}
}
