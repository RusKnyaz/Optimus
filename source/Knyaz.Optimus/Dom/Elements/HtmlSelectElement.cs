using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents &lt;SELECT&gt; HTML element.
	/// </summary>
	public sealed class HtmlSelectElement : HtmlElement
	{
		internal HtmlSelectElement(Document ownerDocument) : base(ownerDocument, TagsNames.Select)
		{
			Options = new HtmlOptionsCollection(this);
			SelectedOptions = new List<HtmlOptionElement>();
		}

		/// <summary>
		/// Adds new 'OPTION' nodes to the dropdown. Other nodes types are ignored.
		/// </summary>
		public override Node AppendChild(Node node)
		{
			if (node is HtmlOptionElement option && node.ChildNodes.Count == 0 && !Multiple)
				SelectedOptions.Add(option);

			return base.AppendChild(node);
		}

		/// <summary>
		/// Adds an element to the collection of option elements for this select element.
		/// </summary>
		/// <param name="option">The element to be added.</param>
		/// <param name="before">An element of the collection, should be inserted before.</param>
		public void Add(HtmlOptionElement option, HtmlOptionElement before = null)
		{
			if (before == null)
				AppendChild(option);
			else
				InsertBefore(option, before);
		}

		/// <summary>
		/// Adds an element to the collection of option elements for this select element.
		/// </summary>
		/// <param name="option">The element to be added.</param>
		/// <param name="index">The index, representing the item should be inserted before.</param>
		public void Add(HtmlOptionElement option, long index) =>
			InsertBefore(option, Options[index]);
		
		/// <summary>
		/// Removes the option from specified position.
		/// </summary>
		/// <param name="index">Position of option to be removed.</param>
		public void Remove(int index)
		{
			var options = Options;
			if (index >= 0 && index < options.Length)
			{
				var optionToRemove = options[index];
				RemoveChild(optionToRemove);
				if (SelectedOptions.IndexOf(optionToRemove) != -1)
					SelectedOptions.Remove(optionToRemove);
			}
		}

		/// <summary>
		/// The number of &lt;option&gt; elements in this select element.
		/// </summary>
		public int Length => ChildNodes.OfType<HtmlOptionElement>().Count();

		/// <summary>
		/// Gets an options collection of this select element.
		/// </summary>
		public HtmlOptionsCollection Options { get; private set; }

		/// <summary>
		/// Reflects the multiple HTML attribute, indicating whether multiple items can be selected.
		/// </summary>
		public bool Multiple { get; set; }

		/// <summary>
		/// The form control's type. When multiple is true, it returns "select-multiple"; otherwise, it returns "select-one".
		/// </summary>
		public string Type => Multiple ? "select-multiple" : "select-one";

		/// <summary>
		/// Gets the first selected option or <c>null</c> if nothing have been selected.
		/// </summary>
		public string Value => SelectedOptions.Count > 0 ? SelectedOptions[0].Value : null;

		/// <summary>
		/// Gets a collection of selected options.
		/// </summary>
		public IList<HtmlOptionElement> SelectedOptions { get; private set; }

		/// <summary>
		/// Gets or sets the 'name' attribute, used by servers and DOM search functions.
		/// </summary>
		public string Name
		{
			get => GetAttribute("name", string.Empty);
			set => SetAttribute("name", value);
		}

		/// <summary>
		/// Gets the index of the first selected &lt;option&gt; element. The value -1 indicates no element is selected.
		/// </summary>
		public long SelectedIndex
		{
			get => SelectedOptions.Count == 0 ? -1 : ChildNodes.IndexOf(SelectedOptions[0]);
			set
			{
				SelectedOptions.Clear();
				SelectedOptions.Add(Options[value]);
			}
		}
		
		/// <summary>
		/// Is a <see cref="HtmlFormElement"/> reflecting the form that this button is associated with.
		/// </summary>
		public HtmlFormElement Form => this.FindOwnerForm();
	}

	/// <summary>
	/// Represents collection of 'option' elements inside 'select'.
	/// </summary>
	public class HtmlOptionsCollection : IEnumerable<HtmlOptionElement>
	{
		private readonly HtmlSelectElement _owner;

		internal HtmlOptionsCollection(HtmlSelectElement owner) => _owner = owner;

		private IEnumerable<HtmlOptionElement> Options =>
			_owner.ChildNodes.Flat(x => x.ChildNodes).OfType<HtmlOptionElement>();
		
		public int Length
		{
			get => _owner.Length;
			set
			{
				while (value > _owner.Length)
					_owner.ChildNodes.Add(_owner.OwnerDocument.CreateElement("option"));

				while (value < _owner.Length)
					_owner.Remove(_owner.Length - 1);
			}	
		}

		/// <summary>
		/// Gets the specific node at the given zero-based index (gives null if out of range).
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public HtmlOptionElement Item(int index) => Options.ElementAtOrDefault(index);

		/// <summary>
		/// Searches the specific node with the given name. Returns null if no such named node exists.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public HtmlOptionElement NamedItem(string name)
		{
			return Options.FirstOrDefault(x => x.Id == name)
			       ?? Options.FirstOrDefault(x => x.Name == name);
		}

		/// <summary>
		/// Gets a node by specified name or index.
		/// </summary>
		/// <param name="key"></param>
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

		public IEnumerator<HtmlOptionElement> GetEnumerator() => Options.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
