using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom.Elements
{
	public class AttributesCollection : IEnumerable<Attr>
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

	public class Element : Node, IElement
	{
		public Element(Document ownerDocument) : base(ownerDocument)
		{
			NodeType = ELEMENT_NODE;
			Attributes = new AttributesCollection();
		}
		
		public AttributesCollection Attributes { get; private set; }

		public Element(Document ownerDocument, string tagName) :this(ownerDocument)
		{
			TagName = tagName;
		}

		public string TagName { get; private set; }

		public virtual string InnerHTML
		{
			get
			{
				var sb = new StringBuilder();
				foreach (var child in ChildNodes)
				{
					var text = child as Text;
					if (text != null)
						sb.Append(text.Data);

					var elem = child as Element;
					if (elem != null)
						sb.Append(elem);
				}

				return sb.ToString();
			}
			set
			{
				ChildNodes.Clear();
				DocumentBuilder.Build(this, value);
			} 
		}

		public Element[] GetElementsByTagName(string tagName)
		{
			return ChildNodes.SelectMany(x => x.Flatten()).OfType<Element>().Where(x => x.TagName == tagName).ToArray();
		}

		public string GetAttribute(string name)
		{
			if(Attributes.ContainsKey(name))
				return Attributes[name].Value;
			return null;
		}

		public override Document OwnerDocument
		{
			get
			{
				return base.OwnerDocument;
			}
			set
			{
				base.OwnerDocument = value;
				foreach (var attribute in Attributes)
				{
					attribute.OwnerDocument = value;
				}
			}
		}

		public void RemoveAttributeNode(Attr attr)
		{
			if (attr.OwnerElement != this)
				return;

			Attributes.Remove(attr.Name);
			attr.SetOwnerElement(null);
		}

		public void SetAttribute(string name, string value)
		{
			var invariantName = name.ToLowerInvariant();
			if (Attributes.ContainsKey(invariantName))
				Attributes[invariantName].Value = value;
			else
				Attributes.Add(invariantName, new Attr(this, name, value) { OwnerDocument = OwnerDocument });

			UpdatePropertyFromAttribute(value, invariantName);
		}

		protected virtual void UpdatePropertyFromAttribute(string value, string invariantName)
		{
			if (invariantName == "id")
			{
				Id = value;
			}
		}

		public Attr SetAttributeNode(Attr attr)
		{
			attr.SetOwnerElement(this);

			var invariantName = attr.Name.ToLowerInvariant();

			if (Attributes.ContainsKey(invariantName))
				Attributes[invariantName] = attr;
			else
				Attributes.Add(invariantName, attr);

			UpdatePropertyFromAttribute(attr.Value, invariantName);

			return attr;
		}

		public void RemoveAttribute(string name)
		{
			var attr = GetAttributeNode(name);
			if(attr!=null)
				RemoveAttributeNode(attr);
		}

		public bool HasAttribute(string name)
		{
			return Attributes.ContainsKey(name);
		}

		public bool HasAttributes()
		{
			return Attributes.Count > 0;
		}

		public override string NodeName { get { return TagName;} }

		public override string ToString()
		{
			return ToString(true);
		}

		private string ToString(bool deep)
		{
			var sb = new StringBuilder();
			sb.Append("<");
			sb.Append(TagName ?? NodeName);
			if (HasAttributes())
			{
				foreach (var attribute in Attributes)
				{
					sb.Append(" ");
					sb.Append(attribute.Name);//todo: use invariant name
					if (attribute.Value != null)
					{
						sb.Append("=\"");
						sb.Append(attribute.Value.Replace("\"", "\\\""));
						sb.Append("\"");
					}
				}
			}
			sb.Append(">");
			if(deep)
				sb.Append(InnerHTML);
			sb.Append("</");
			sb.Append(TagName);
			sb.Append(">");
			return sb.ToString();
		}

		public override Node CloneNode(bool deep)
		{
			var node = OwnerDocument.CreateElement(TagName);
			foreach (var attribute in Attributes)
			{
				node.SetAttributeNode((Attr)attribute.CloneNode());
			}
			if (deep)
			{
				foreach (var childNode in ChildNodes)
				{
					node.AppendChild(childNode.CloneNode());
				}
			}
			return node;
		}

		public Attr GetAttributeNode(string name)
		{
			return Attributes.ContainsKey(name) ? Attributes[name] : null;
		}
	}

	[DomItem]
	public interface IElement
	{
		string TagName { get; }
		string InnerHTML { get; set; }
		Element[] GetElementsByTagName(string tagName);
		Attr GetAttributeNode(string name);
		string GetAttribute(string name);
		void RemoveAttribute(string name);
		void SetAttribute(string name, string value);
		Attr SetAttributeNode(Attr attr);
		void RemoveAttributeNode(Attr attr);
		bool HasAttribute(string name);
		bool HasAttributes();
	}
}