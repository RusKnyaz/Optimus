using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	[DomItem]
	public interface IAttributesCollection : IEnumerable<Attr>
	{
		Attr this[string name] { get; set; }
		Attr this[int idx] { get; }
		bool ContainsKey(string name);
		void Remove(string name);
		void Add(string invariantName, Attr attr);
		int Count { get; }
	}

	public class AttributesCollection : IAttributesCollection
	{
		public AttributesCollection()
		{
			Properties = new OrderedDictionary();
		}

		public OrderedDictionary Properties { get; private set; }

		public Attr this[string name]
		{
			get
			{
				int number;
				if (int.TryParse(name, out number))
					return this[number];
				
				return (Attr)Properties[name]; //return value
			}
			set
			{
				int number;
				if (int.TryParse(name, out number))
					return;
			
				Properties[name] = value;
			}
		}

		public Attr this[int idx]
		{
			get
			{
				return idx < 0 || idx >= Properties.Count ? null : (Attr)Properties[idx];
			}
		}

		public bool ContainsKey(string name)
		{
			int number;
			if (int.TryParse(name, out number))
				return number >= 0  && number < Count;

			return Properties.Contains(name);
		}

		public IEnumerator<Attr> GetEnumerator()
		{
			return Properties.Values.Cast<Attr>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Properties.GetEnumerator();
		}

		public void Remove(string name)
		{
			Properties.Remove(name);
		}

		public void Add(string invariantName, Attr attr)
		{
			Properties.Add(invariantName, attr);
		}

		public int Count { get { return Properties.Count; } }
	}
}