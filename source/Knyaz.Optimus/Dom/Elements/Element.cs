using System;
using System.Linq;
using System.Text;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Elements
{
	public class Element : Node, IElement
	{
		private readonly IAttributesCollection _attributes;

		public Element(Document ownerDocument) : base(ownerDocument)
		{
			NodeType = ELEMENT_NODE;
			_attributes = new AttributesCollection();
		}

		public IAttributesCollection Attributes
		{
			get { return _attributes; }
		}

		public Element(Document ownerDocument, string tagName) :this(ownerDocument)
		{
			TagName = tagName;
		}

		public string TagName { get; private set; }

		public string Id
		{
			get { return GetAttribute("id", string.Empty); }
			set { SetAttribute("id", value); }
		}

		public string OuterHTML
		{
			get { return ToString();}
			set
			{
				if (ParentNode == null)
					throw new DOMException(DOMException.Codes.NoModificationAllowedError);

				var tempNode = OwnerDocument.CreateElement("div");
				DocumentBuilder.Build(tempNode, value, NodeSources.Script);
				foreach (var childNode in tempNode.ChildNodes.ToArray())
				{
					ParentNode.InsertBefore(childNode, this);
				}
				ParentNode.RemoveChild(this);
			}
		}

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
				DocumentBuilder.Build(this, value, NodeSources.Script);
			} 
		}

		public string TextContent
		{
			get
			{
				return string.Join(" ", ChildNodes.Flat(x => x.ChildNodes).OfType<CharacterData>().Select(x => x.Data));
			}
			set { InnerHTML = value; }
		}

		public Element[] GetElementsByTagName(string tagName)
		{
			return ChildNodes.SelectMany(x => x.Flatten()).OfType<Element>().Where(x => x.TagName == tagName.ToUpperInvariant()).ToArray();
		}

		public HtmlElement[] GetElementsByClassName(string name)
		{
			return ChildNodes.SelectMany(x => x.Flatten()).OfType<HtmlElement>().Where(x => x.ClassName.Split(' ').Contains(name)).ToArray();
		}

		public string GetAttribute(string name)
		{
			var node = GetAttributeNode(name);
			return node == null ? null : node.Value;
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
			{
				//todo: we should notify someone about value changed. For example to handle event subscriptions.
				Attributes[invariantName].Value = value;
			}
			else
			{
				var attr = new Attr(this, invariantName, value) {OwnerDocument = OwnerDocument};
				Attributes.Add(invariantName, attr);
				OwnerDocument.HandleNodeAdded(attr);
			}

			UpdatePropertyFromAttribute(value, invariantName);
		}

		protected virtual string PreGetAttribute(string invariantName, string value)
		{
			return value;
		}

		protected virtual void UpdatePropertyFromAttribute(string value, string invariantName)
		{
			//todo: remove the stuff
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

		public bool Contains(INode element)
		{
			return ChildNodes.Flat(x => x.ChildNodes).Contains(element);
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
					node.AppendChild(childNode.CloneNode(true));
				}
			}
			return node;
		}

		public Attr GetAttributeNode(string name)
		{
			return Attributes.ContainsKey(name) ? Attributes[name] : null;
		}

		protected string GetAttribute(string name, string[] availableValues, string def)
		{
			var val = GetAttribute(name, def);
			return availableValues.Contains(val.ToLowerInvariant()) ? val : def;
		}

		protected T GetAttribute<T>(string name, T def)
		{
			var val = GetAttribute(name);
			if (val == null)
				return def;

			try
			{
				return (T)Convert.ChangeType(val, typeof(T));
			}
			catch
			{
				return def;
			}
		}

		/// <summary>
		/// Parses the given string text as HTML or XML and inserts the resulting nodes into the tree in the position given by the position argument, as follows:
		///"beforebegin" - Before the element itself.
		///"afterbegin" - Just inside the element, before its first child.
		///"beforeend" - Just inside the element, after its last child.
		///"afterend" - After the element itself.
		/// </summary>
		public void InsertAdjacentHTML(string position, string html)
		{
			var tempNode = OwnerDocument.CreateElement("div");
			DocumentBuilder.Build(tempNode, html);
			switch (position)
			{
				case "beforebegin":
					if(ParentNode == null)
						throw new DOMException(DOMException.Codes.NoModificationAllowedError);
					foreach (var child in tempNode.ChildNodes.ToArray())
					{
						ParentNode.InsertBefore(child, this);
					}
					break;
				case "afterbegin":
					foreach (var child in tempNode.ChildNodes.Reverse().ToArray())
					{
						if (FirstChild == null)
							AppendChild(child);
						else
							InsertBefore(child, FirstChild);
					}
					break;
				case "beforeend":
					foreach (var child in tempNode.ChildNodes.ToArray())
					{
						AppendChild(child);
					}
					break;
				case "afterend":
					if (NextSibling == null)
					{
						foreach (var child in tempNode.ChildNodes.ToArray())
						{
							ParentNode.AppendChild(child);
						}
					}
					else
					{
						var sibl = NextSibling;
						foreach (var child in tempNode.ChildNodes.ToArray())
						{
							ParentNode.InsertBefore(child, sibl);
						}
					}
					break;
			}
		}
	}

	[DomItem]
	public interface IElement : INode
	{
		string TagName { get; }
		string Id { get; }
		string InnerHTML { get; set; }
		string TextContent { get; set; }
		Element[] GetElementsByTagName(string tagName);
		HtmlElement[] GetElementsByClassName(string tagName);
		Attr GetAttributeNode(string name);
		string GetAttribute(string name);
		void RemoveAttribute(string name);
		void SetAttribute(string name, string value);
		Attr SetAttributeNode(Attr attr);
		void RemoveAttributeNode(Attr attr);
		bool HasAttribute(string name);
		bool HasAttributes();
		bool Contains(INode element);
	}
}