using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary> Represents dynamic or static nodes collection. </summary>
	public class NodeList : IReadOnlyList<Node>
	{
		private readonly Func<IEnumerable<Node>> _items;

		internal NodeList(Func<IEnumerable<Node>> items) {_items = items;}

		public IEnumerator<Node> GetEnumerator() { return _items().GetEnumerator();}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator();}

		[JsName("length")]
		public int Count => _items().Count();

		public Node this[int index] => _items().Skip(index).First();
	}
}