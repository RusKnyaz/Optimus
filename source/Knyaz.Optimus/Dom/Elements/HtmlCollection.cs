using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents readonly colletion of html elements.
	/// </summary>
	public class HtmlCollection : IReadOnlyList<HtmlElement>
	{
		private readonly Func<IEnumerable<HtmlElement>> _items;

		internal HtmlCollection(Func<IEnumerable<HtmlElement>> items) {_items = items;}

		public IEnumerator<HtmlElement> GetEnumerator() { return _items().GetEnumerator();}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator();}

		public int Count => _items().Count();

		public HtmlElement this[int index] => _items().Skip(index).First();
	}
}
