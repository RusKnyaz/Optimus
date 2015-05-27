using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom.Elements
{
	public class Element : Node, IElement
	{
		public Element(Document ownerDocument) : base(ownerDocument)
		{
			NodeType = ELEMENT_NODE;
			Attributes = new Dictionary<string, Attr>();
		}
		
		public IDictionary<string, Attr> Attributes { get; private set; }

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
				var items = DocumentBuilder.Build(OwnerDocument, value);
				foreach (var it in items)
				{
					AppendChild(it);
				}
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
					attribute.Value.OwnerDocument = value;
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
			sb.Append(TagName);
			if (HasAttributes())
			{
				foreach (var attribute in Attributes)
				{
					sb.Append(" ");
					sb.Append(attribute.Key);
					if (attribute.Value != null)
					{
						sb.Append("=\"");
						sb.Append(attribute.Value.Value.Replace("\"", "\\\""));
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
			var node  = DocumentBuilder.Build(OwnerDocument, ToString(deep)).Single();
			node.OwnerDocument = OwnerDocument;
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