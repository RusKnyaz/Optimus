using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace Knyaz.Optimus.Dom.Elements
{
	public class HtmlSelectElement : HtmlElement
	{
		public HtmlSelectElement(Document ownerDocument) : base(ownerDocument, TagsNames.Select)
		{
			Options = new HtmlOptionsCollection(this);
			SelectedOptions = new List<HtmlOptionElement>();
		}

		//todo: revise insertChild
		public override Node AppendChild(Node node)
		{
			var option = node as HtmlOptionElement;
			if (option == null)
				return node;

			if (node.ChildNodes.Count == 0 && !Multiple)
				SelectedOptions.Add(option);

			return base.AppendChild(node);
		}

		public void Add(HtmlOptionElement option, HtmlOptionElement before = null)
		{
			if (before == null)
				AppendChild(option);
			else
				InsertBefore(option, before);
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

		public bool Multiple { get; set; }

		public string Type { get { return Multiple ? "select-multiple" : "select-one"; } }

		public string Value { get { return SelectedOptions.Count > 0 ? SelectedOptions[0].Value : null; } }

		public IList<HtmlOptionElement> SelectedOptions { get; private set; }

		public string Name
		{
			get { return GetAttribute("name", string.Empty); }
			set { SetAttribute("name", value); }
		}

		public long SelectedIndex
		{
			get
			{
				return SelectedOptions.Count == 0 ? -1 : ChildNodes.IndexOf(SelectedOptions[0]);
			}
			set
			{
				SelectedOptions.Clear();
				SelectedOptions.Add(Options[value]);
			}
		}
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
