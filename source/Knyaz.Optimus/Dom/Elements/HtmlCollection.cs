using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary> Represents readonly collection of html elements. </summary>
	[JsName("HTMLCollection")]
	public class HtmlCollection : IReadOnlyList<HtmlElement>
	{
		private readonly Func<IEnumerable<HtmlElement>> _items;

		internal HtmlCollection(Func<IEnumerable<HtmlElement>> items) {_items = items;}

		public IEnumerator<HtmlElement> GetEnumerator() { return _items().GetEnumerator();}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator();}

		[JsName("length")]
		public int Count => _items().Count();

		public HtmlElement this[int index] => _items().Skip(index).First();

		/// <summary> Gets the item with specified Id. </summary>
		public HtmlElement this[string id] => 
			_items().FirstOrDefault(x => x.Id == id) 
			?? (int.TryParse(id, out var idx) ? this[idx] : null);

		/// <summary> Gets the item with specified Id. </summary>
		public HtmlElement NamedItem(string id) => this[id];
	}
}
