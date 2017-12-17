using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Tools;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents the element of the DOM.
	/// https://www.w3.org/TR/2004/REC-DOM-Level-3-Core-2004040
	/// </summary>
	public abstract class Element : Node, IElement, IElementSelector
	{
		private readonly TokenList _classList = null;

		internal Element(Document ownerDocument) : base(ownerDocument)
		{
			NodeType = ELEMENT_NODE;
			Attributes = new AttributesCollection();
			_classList = new TokenList(() => ClassName);
			_classList.Changed += () => {
				ClassName = string.Join(" ", _classList);
			};
		}

		/// <summary>
		/// Returns a collection of the specified node's attributes.
		/// </summary>
		public AttributesCollection Attributes { get; }

		internal Element(Document ownerDocument, string tagName) : this(ownerDocument) => TagName = tagName;

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
			get => GetAttribute("id", string.Empty);
			set => SetAttribute("id", value);
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
					switch (child)
					{
						case Text text:sb.Append(text.Data);break;
						case Element elem:sb.Append(elem);break;
					}
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
			get => string.Join(" ", ChildNodes.Flat(x => x.ChildNodes).OfType<CharacterData>().Select(x => x.Data));
			set => InnerHTML = value;
		}

		/// <summary>
		/// Returns a collection containing all descendant elements with the specified tag name.
		/// </summary>
		/// <param name="tagName">A string that specifies the tagname to search for. The value "*" matches all tags</param>
		public Element[] GetElementsByTagName(string tagName)
		{
			var invariantName = tagName.ToUpperInvariant();
			
			return tagName == "*" 
				? Descendants.ToArray() 
				: Descendants.Where(x => x.TagName == invariantName).ToArray();
		}

		private IEnumerable<Element> Descendants => ChildNodes.SelectMany(x => x.Flatten()).OfType<Element>();


		private static char[] _spaceSplitter = new[] {' '};
		/// <summary>
		/// Returns a collection containing all descendant elements with the specified class name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Element[] GetElementsByClassName(string name)
		{
			var classes = name.Split(_spaceSplitter, StringSplitOptions.RemoveEmptyEntries);
			
			switch (classes.Length)
			{
				case 0:
					return new Element[0];
				case 1:
					return Descendants.Where(x => x.ClassList.Contains(name)).ToArray();
				default:
					return Descendants.Where(x => classes.All(c => x.ClassList.Contains(c))).ToArray();
			}
		}

		/// <summary>
		/// Retrieves an attribute value by name.
		/// </summary>
		/// <param name="name">The name of the attribute to retrieve.</param>
		/// <returns>The <see cref="Attr"/> value as a string, or the string.Empty if that attribute does not have a specified or default value.</returns>
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
			get => base.OwnerDocument;
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

		/// <summary>
		/// Removes the specified attribute node.
		/// </summary>
		/// <param name="attr">The <see cref="Attr"/> node to remove from the attribute list.</param>
		public void RemoveAttributeNode(Attr attr)
		{
			if (attr.OwnerElement != this)
				return;

			Attributes.Remove(attr.Name);
			attr.SetOwnerElement(null);
		}

		/// <summary>
		/// Adds a new attribute. If an attribute with that name is already present in the element, its value is changed to be that of the value parameter.
		/// </summary>
		/// <param name="name">The name of the attribute to create or alter.</param>
		/// <param name="value">Value to set in string form.</param>
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

		protected virtual void UpdatePropertyFromAttribute(string value, string invariantName)
		{
			//todo: remove the stuff
		}

		/// <summary>
		/// Adds a new attribute node. If an attribute with that name (nodeName) is already present in the element, it is replaced by the new one.
		/// </summary>
		/// <param name="attr">The <see cref="Attr"/> node to add to the attribute list.</param>
		/// <returns>If the newAttr attribute replaces an existing attribute, the replaced Attr node is returned, otherwise null is returned.</returns>
		public Attr SetAttributeNode(Attr attr)
		{
			Attr result = null;
			
			attr.SetOwnerElement(this);

			var invariantName = attr.Name.ToLowerInvariant();

			if (Attributes.ContainsKey(invariantName))
			{
				result = Attributes[invariantName];

				if (!ReferenceEquals(result, attr))
				{
					result.SetOwnerElement(null);
					Attributes[invariantName] = attr;	
				}
			}
			else
			{
				Attributes.Add(invariantName, attr);
			}

			UpdatePropertyFromAttribute(attr.Value, invariantName);

			return result;
		}

		/// <summary>
		/// Removes an attribute by name.
		/// </summary>
		/// <param name="name">The name of the attribute to remove.</param>
		public void RemoveAttribute(string name)
		{
			var attr = GetAttributeNode(name);
			if(attr!=null)
				RemoveAttributeNode(attr);
		}

		/// <summary>
		/// Returns <c>true</c> if the specified attribute exists, otherwise it returns <c>false</c>.
		/// </summary>
		/// <param name="name">The name of the attribute to check.</param>
		public bool HasAttribute(string name) => Attributes.ContainsKey(name);

		/// <summary>
		/// Indicates whether this node (if it is an element) has any attributes.
		/// </summary>
		/// <returns><c>true</c> if this node has any attributes, <c>false</c> otherwise.</returns>
		public bool HasAttributes() => Attributes.Length > 0;

		/// <summary>
		/// Checks whether a node is a descendant of a given Element or not.
		/// </summary>
		/// <param name="element">The node to search.</param>
		/// <returns><c>True</c> if node found, <c>False</c> otherwise.</returns>
		public bool Contains(INode element) => ChildNodes.Flat(x => x.ChildNodes).Contains(element);

		/// <summary>
		/// For an Element the NodeName is tag name.
		/// </summary>
		public override string NodeName => TagName;

		public override string ToString() => ToString(true);

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

		/// <summary>
		/// Retrieves an attribute node by name.
		/// </summary>
		/// <param name="name">The name (nodeName) of the attribute to retrieve.</param>
		/// <returns>The <see cref="Attr"/> node with the specified name (nodeName) or <c>null</c> if there is no such attribute.</returns>
		public Attr GetAttributeNode(string name) => Attributes.ContainsKey(name) ? Attributes[name] : null;

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

		/// <summary>
		/// Returns the first descendant element that matches a specified CSS selector(s).
		/// </summary>
		public virtual IElement QuerySelector(string query) =>
			((CssSelector) query).Select(this).FirstOrDefault();

		/// <summary>
		/// Returns all elements in the document that matches a specified CSS selector(s).
		/// </summary>
		public virtual IReadOnlyList<IElement> QuerySelectorAll(string query) =>
			((CssSelector)query).Select(this).ToList().AsReadOnly();

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

		/// <summary>
		/// Returns the size of an element and its position relative to the viewport.
		/// </summary>
		/// <returns></returns>
		public DomRect GetBoundingClientRect()
		{
			//stub
			//todo: implement something
			return new DomRect();
		}

		/// <summary>
		/// Removes this node from its parent.
		/// </summary>
		public void Remove() => ParentNode?.RemoveChild(this);
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
}