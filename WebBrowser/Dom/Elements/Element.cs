using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowser.Dom.Elements
{
	public class Element : Node
	{
		public Element()
		{
			NodeType = ELEMENT_NODE;
			Attributes = new Dictionary<string, Attr>();
		}
		
		public IDictionary<string, Attr> Attributes { get; private set; }

		public Element(string tagName) :this()
		{
			TagName = tagName;
		}

		public string TagName { get; private set; }

		public string InnerHtml
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
				var items = DocumentBuilder.Build(value);
				foreach (var it in items)
				{
					ChildNodes.Add(it);
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

		public void SetAttribute(string name, string value)
		{
			if (Attributes.ContainsKey(name))
				Attributes[name].Value = value;
			else
				Attributes.Add(name, new Attr(this, name, value));
		}

		public void RemoveAttribute(string name)
		{
			Attributes.Remove(name);
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
						sb.Append(attribute.Value.Value.Replace("\"","\\\""));
						sb.Append("\"");
					}
				}
			}
			sb.Append(">");
			sb.Append(InnerHtml);
			sb.Append("</");
			sb.Append(TagName);
			sb.Append(">");
			return sb.ToString();
		}

		public override INode CloneNode()
		{
			var node  = DocumentBuilder.Build(ToString()).Single();
			node.OwnerDocument = OwnerDocument;
			return node;
		}

		public Attr GetAttributeNode(string name)
		{
			return Attributes.ContainsKey(name) ? Attributes[name] : null;
		}
	}
}