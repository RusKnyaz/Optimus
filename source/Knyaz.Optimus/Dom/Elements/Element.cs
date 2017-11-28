using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Elements
{
	public class Element : Node, IElement, IElementSelector
	{
		private readonly IAttributesCollection _attributes;
		private TokenList _classList = null;

		public Element(Document ownerDocument) : base(ownerDocument)
		{
			NodeType = ELEMENT_NODE;
			_attributes = new AttributesCollection();
			_classList = new TokenList(() => ClassName);
			_classList.Changed += () => {
				ClassName = string.Join(" ", _classList);
			};
		}

		public IAttributesCollection Attributes
		{
			get { return _attributes; }
		}

		public Element(Document ownerDocument, string tagName) :this(ownerDocument)
		{
			TagName = tagName;
		}

		/// <summary>
		/// Get the tag name of an element.
		/// </summary>
		public string TagName { get; private set; }

		/// <summary>
		/// Sets or gets the value of the 'class' attribute.
		/// </summary>
		public string ClassName
		{
			get { return GetAttribute("class", ""); }
			set { SetAttribute("class", value); }
		}

		/// <summary>
		/// Returns a live DOMTokenList collection of the class attributes of the element.
		/// </summary>
		public ITokenList ClassList => _classList;

		/// <summary>
		/// Represents the element's identifier, reflecting the id global attribute.
		/// </summary>
		public string Id
		{
			get { return GetAttribute("id", string.Empty); }
			set { SetAttribute("id", value); }
		}

		/// <summary>
		/// Gets the serialized HTML fragment describing the element including its descendants. 
		/// It can be set to replace the element with nodes parsed from the given string.
		/// </summary>
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

		/// <summary>
		/// Sets or gets the serialized HTML describing the element's descendants.
		/// </summary>
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

		/// <summary>
		///  Represents the text content of a node and its descendants.
		/// </summary>
		public virtual string TextContent
		{
			get
			{
				return string.Join(" ", ChildNodes.Flat(x => x.ChildNodes).OfType<CharacterData>().Select(x => x.Data));
			}
			set { InnerHTML = value; }
		}

		/// <summary>
		/// Returns a collection containing all elements with the specified tag name.
		/// </summary>
		/// <param name="tagNameSelector"></param>
		/// <returns></returns>
		public Element[] GetElementsByTagName(string tagNameSelector)
		{
			var parts = tagNameSelector.Split('.');
			var tagName = parts[0].ToUpperInvariant();
			//todo: revise this strange code (i mean handling 'classes' selector)

			return ChildNodes.SelectMany(x => x.Flatten()).OfType<Element>().Where(x => x.TagName == tagName && parts.Skip(1).All(c => x.ClassList.Contains(c))).ToArray();
		}

		/// <summary>
		/// Returns a collection containing all descendant elements with the specified class name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Element[] GetElementsByClassName(string name)
		{
			return ChildNodes.SelectMany(x => x.Flatten()).OfType<Element>().Where(x => x.ClassList.Contains(name)).ToArray();
		}

		public string GetAttribute(string name)
		{
			var node = GetAttributeNode(name);
			return node == null ? null : node.Value;
		}

		/// <summary>
		/// Returns the top-level document object for this node.
		/// </summary>
		public override Document OwnerDocument
		{
			get
			{
				return base.OwnerDocument;
			}
			set	{}
		}

		internal override void SetOwner(Document doc)
		{
			base.SetOwner(doc);
			foreach (var attribute in Attributes)
			{
				attribute.SetOwner(doc);
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
				var attr = new Attr(this, invariantName, value);
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

		/// <summary>
		/// Checks whether a node is a descendant of a given Element or not.
		/// </summary>
		/// <param name="element">The node to search.</param>
		/// <returns><c>True</c> if node found, <c>False</c> otherwise.</returns>
		public bool Contains(INode element)
		{
			return ChildNodes.Flat(x => x.ChildNodes).Contains(element);
		}

		/// <summary>
		/// For an Element the NodeName is tag name.
		/// </summary>
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

		public virtual IElement QuerySelector(string query)
		{
			return ((CssSelector) query).Select(this).FirstOrDefault();
		}

		public virtual IReadOnlyList<IElement> QuerySelectorAll(string query)
		{
			return ((CssSelector)query).Select(this).ToList().AsReadOnly();
		}

		/// <summary>
		/// Returns nearest ancestor or itself which satisfies to specified selector.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public IElement Closest(string query)
		{
			var selector = new CssSelector(query);
			return ((IElement) this).GetRecursive(x => (IElement) x.ParentNode).FirstOrDefault(selector.IsMatches);
		}

		public DomRect GetBoundingClientRect()
		{
			//stub
			//todo: implement something
			return new DomRect();
		}
	}

	public class DomRect
	{
		/// <summary>
		/// Y-coordinate, relative to the viewport origin, of the bottom of the rectangle box.
		/// </summary>
		public float Bottom { get; private set; }

		/// <summary>
		/// Height of the rectangle box (This is identical to bottom minus top).
		/// </summary>
		public float Height { get; private set; }

		/// <summary>
		/// X-coordinate, relative to the viewport origin, of the left of the rectangle box.
		/// </summary>
		public float Left { get; private set; }

		/// <summary>
		/// X-coordinate, relative to the viewport origin, of the right of the rectangle box.
		/// </summary>
		public float Right { get; private set; }

		/// <summary>
		/// Y-coordinate, relative to the viewport origin, of the top of the rectangle box.
		/// </summary>
		public float Top { get; private set; }

		/// <summary>
		/// Width of the rectangle box (This is identical to right minus left).
		/// </summary>
		public float Width { get; private set; }

		/// <summary>
		/// X-coordinate, relative to the viewport origin, of the left of the rectangle box.
		/// </summary>
		public float X { get; private set; }

		/// <summary>
		/// Y-coordinate, relative to the viewport origin, of the top of the rectangle box.
		/// </summary>
		public float Y { get; private set; }
	}

	[DomItem]
	public interface IElement : INode
	{
		string TagName { get; }
		string Id { get; }
		string ClassName { get; set; }
		string InnerHTML { get; set; }
		string TextContent { get; set; }

		/// <summary>
		/// Returns a collection containing all elements with the specified tag name
		/// </summary>
		/// <param name="tagNameSelector"></param>
		/// <returns></returns>
		Element[] GetElementsByTagName(string tagNameSelector);

		/// <summary>
		/// Returns a collection containing all descendant elements with the specified class name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		Element[] GetElementsByClassName(string tagName);
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