using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	[DomItem]
	public class AttributesCollection : IEnumerable<Attr>
	{
		internal AttributesCollection() {}

		private readonly OrderedDictionary _properties 
			= new OrderedDictionary(StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		/// Gets or sets <see cref="Attr"/> object with the specified name.
		/// </summary>
		/// <param name="name"></param>
		public Attr this[string name]
		{
			get => int.TryParse(name, out var number) ? this[number] : (Attr) _properties[name];
			set
			{
				if (int.TryParse(name, out _))
					return;
			
				_properties[name] = value;
			}
		}

		/// <summary>
		/// Gets <see cref="Attr"/> node at specified position.
		/// </summary>
		public Attr this[int idx] => idx < 0 || idx >= _properties.Count ? null : (Attr)_properties[idx];

		internal bool ContainsKey(string name) =>
			int.TryParse(name, out var number) 
				? number >= 0 && number < Length 
				: _properties.Contains(name);

		public IEnumerator<Attr> GetEnumerator() => _properties.Values.Cast<Attr>().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _properties.GetEnumerator();

		/// <summary>
		/// Removes the attribute with specifeid name.
		/// </summary>
		internal void Remove(string name) => _properties.Remove(name);

		internal void Add(string invariantName, Attr attr) => _properties.Add(invariantName, attr);

		public int Length => _properties.Count;
	}
}