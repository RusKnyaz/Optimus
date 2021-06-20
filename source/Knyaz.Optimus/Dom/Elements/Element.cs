using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Tools;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents the element of the DOM.
	/// https://www.w3.org/TR/2004/REC-DOM-Level-3-Core-2004040
	/// </summary>
	public abstract class Element : Node
	{
		private readonly TokenList _classList = null;
		private readonly List<Node> _childNodes = new List<Node>();

		internal Element(HtmlDocument ownerDocument) : base(ownerDocument)
		{
			NodeType = ELEMENT_NODE;
			Attributes = new AttributesCollection();
			_classList = new TokenList(() => ClassName);
			_classList.Changed += () => {
				ClassName = string.Join(" ", _classList);
			};
		}

		protected internal override IEnumerable<Node> Children => _childNodes;

		public override Node RemoveChild(Node node)
		{
			_childNodes.Remove(node);
			OwnerDocument?.HandleNodeRemoved(this, node);
			return node;
		}

		public override Node AppendChild(Node node)
		{
			if(node == this)
				throw new InvalidOperationException();
			
			if(node is Attr)
				throw new InvalidOperationException();

			if (node is DocumentFragment)
			{
				foreach (var child in node.ChildNodes.ToList())
				{
					AppendChild(child);
				}
			}
			else
			{
				UnattachFromParent(node);
				_childNodes.Add(node);
				RegisterNode(node);
			}
			return node;
		}
		
		public override Node InsertBefore(Node newChild, Node refNode)
		{
			UnattachFromParent(newChild);
			if (refNode == null)
				_childNodes.Add(newChild);
			else
				_childNodes.Insert(ChildNodes.IndexOf(refNode), newChild);
			RegisterNode(newChild);
			return newChild;
		}
		
		public override Node ReplaceChild(Node newChild, Node oldChild)
		{
			InsertBefore(newChild, oldChild);
			RemoveChild(oldChild);
			return newChild;
		}
		
		public override bool HasChildNodes => ChildNodes.Count > 0;
		
		/// <summary>
		/// Gets the node's first child in the tree, or null if the node has no children. If the node is a Document, it returns the first node in the list of its direct children.
		/// </summary>
		public override Node FirstChild => ChildNodes.FirstOrDefault();

		/// <summary>
		/// Gets the last child of the node. If its parent is an element, then the child is generally an element node, a text node, or a comment node. It returns null if there are no child elements.
		/// </summary>
		public override Node LastChild => ChildNodes.LastOrDefault();
		
		
		protected void Handle(string attrName, Action<Event> actionHandler, Event evt)
		{
			if (actionHandler != null)
				actionHandler(evt);
			else if (GetAttribute(attrName) is string handler)
				OwnerDocument.HandleNodeScript(evt, handler);
		}
		
		protected void Handle(string attrName, Func<Event, bool?> actionHandler, Event evt)
		{
			if (actionHandler != null)
			{
				if(actionHandler(evt) == false)
					evt.PreventDefault();
			}
			else if (GetAttribute(attrName) is string handler)
				OwnerDocument.HandleNodeScript(evt, handler);
			//todo: default can be prevented from 'attribute' function.
		}

		/// <summary>
		/// Returns a collection of the specified node's attributes.
		/// </summary>
		public AttributesCollection Attributes { get; }

		internal Element(HtmlDocument ownerDocument, string tagName) : this(ownerDocument) => TagName = tagName;

		/// <summary> Get the tag name of an element. </summary>
		public string TagName { get; private set; }

		/// <summary> Sets or gets the value of the 'class' attribute. </summary>
		public string ClassName
		{
			get { return GetAttribute("class", ""); }
			set { SetAttribute("class", value); }
		}

		/// <summary> Returns a live DOMTokenList collection of the class attributes of the element. </summary>
		public TokenList ClassList => _classList;

		/// <summary> Represents the element's identifier, reflecting the id global attribute. </summary>
		public string Id
		{
			get => GetAttribute(Attrs.Id, string.Empty);
			set => SetAttribute(Attrs.Id, value);
		}

		/// <summary>
		/// Gets the serialized HTML fragment describing the element including its descendants. 
		/// It can be set to replace the element with nodes parsed from the given string.
		/// </summary>
		public string OuterHTML
		{
			get 
			{
				var sb = new StringBuilder();
				sb.Append("<");
				sb.Append(TagName ?? NodeName);
				lock ((object)OwnerDocument ?? new object())
				{
					if (HasAttributes())
					{
						foreach (var attribute in Attributes)
						{
							sb.Append(" ");
							sb.Append(attribute.Name); //todo: use invariant name
							if (attribute.Value != null)
							{
								sb.Append("=\"");
								sb.Append(attribute.Value.Replace("\"", "\\\""));
								sb.Append("\"");
							}
						}
					}

					sb.Append(">");
					sb.Append(InnerHTML);
				}

				sb.Append("</");
				sb.Append(TagName);
				sb.Append(">");
				return sb.ToString();
			}
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
				lock ((object)OwnerDocument ?? new object())
				{
					foreach (var child in ChildNodes)
					{
						switch (child)
						{
							case Text text:
								sb.Append(text.Data);
								break;
							case Element elem:
								sb.Append(elem.OuterHTML);
								break;
						}
					}
				}

				return sb.ToString();
			}
			set
			{
				_childNodes.Clear();
				DocumentBuilder.Build(this, value, NodeSources.Script);
			} 
		}

		/// <summary>
		///  Represents the text content of a node and its descendants.
		/// </summary>
		public virtual string TextContent
		{
			get => string.Join("", ChildNodes.Flat(x => x.ChildNodes).OfType<Text>().Select(x => x.Data));
			set
			{
				while(ChildNodes.Count > 0)
					RemoveChild(LastChild);
				if(!string.IsNullOrEmpty(value))
					AppendChild(OwnerDocument.CreateTextNode(value));
			}
		}

		/// <summary> Returns a collection containing all descendant elements with the specified tag name. </summary>
		/// <param name="tagName">A string that specifies the tagname to search for. The value "*" matches all tags</param>
		public HtmlCollection GetElementsByTagName(string tagName)
		{
			var invariantName = tagName.ToUpperInvariant();
			
			return tagName == "*" 
				? new HtmlCollection(() => Descendants.OfType<HtmlElement>()) 
				: new HtmlCollection(() => Descendants.OfType<HtmlElement>().Where(x => x.TagName == invariantName));
		}

		private IEnumerable<Element> Descendants => ChildNodes.SelectMany(x => x.Flatten()).OfType<Element>();


		private static char[] _spaceSplitter = new[] {' '};
		/// <summary> Returns a collection containing all descendant elements with the specified class name. </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public HtmlCollection GetElementsByClassName(string name)
		{
			return new HtmlCollection(() =>
			{
				var classes = name.Split(_spaceSplitter, StringSplitOptions.RemoveEmptyEntries);
			
				switch (classes.Length)
				{
					case 0:
						return Enumerable.Empty<HtmlElement>();
					case 1:
						return Descendants.OfType<HtmlElement>().Where(x => x.ClassList.Contains(name));
					default:
						return Descendants.OfType<HtmlElement>().Where(x => classes.All(c => x.ClassList.Contains(c)));
				}	
			});
		}

		/// <summary> Retrieves an attribute value by name. </summary>
		/// <param name="name">The name of the attribute to retrieve.</param>
		/// <returns>The <see cref="Attr"/> value as a string, or the string.Empty if that attribute does not have a specified or default value.</returns>
		public string GetAttribute(string name)
		{
			var node = GetAttributeNode(name);
			return node == null ? null : node.Value;
		}

		/// <summary> Returns the top-level document object for this node. </summary>
		public override HtmlDocument OwnerDocument
		{
			get => base.OwnerDocument;
			set	{}
		}

		internal override void SetOwner(HtmlDocument doc)
		{
			base.SetOwner(doc);
			foreach (var attribute in Attributes)
			{
				attribute.SetOwner(doc);
			}
		}

		/// <summary> Removes the specified attribute node. </summary>
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

		/// <summary> Indicates whether this node (if it is an element) has any attributes. </summary>
		/// <returns><c>true</c> if this node has any attributes, <c>false</c> otherwise.</returns>
		public bool HasAttributes() => Attributes.Length > 0;

		/// <summary> Checks whether a node is a descendant of a given Element or not. </summary>
		/// <param name="element">The node to search.</param>
		/// <returns><c>True</c> if node found, <c>False</c> otherwise.</returns>
		public bool Contains(Node element) => ChildNodes.Flat(x => x.ChildNodes).Contains(element);

		/// <summary> For an Element the NodeName is tag name. </summary>
		public override string NodeName => TagName;

		public override string ToString() => $"[object {GetType().GetCustomAttribute<JsNameAttribute>()?.Name ?? GetType().Name}]";

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

		/// <summary> Retrieves an attribute node by name. </summary>
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

		protected void SetFlagAttribute(string name, bool value)
		{
			if(value)
				SetAttribute(name,name);
			else
				RemoveAttribute(name);
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
		/// Returns first descendant element that matches a specified CSS selector(s).
		/// </summary>
		public virtual Element QuerySelector(string query) =>
			((CssSelector) query).Select(this).FirstOrDefault();

		/// <summary>
		/// Returns all descendant elements that matches a specified CSS selector(s).
		/// </summary>
		public virtual NodeList QuerySelectorAll(string query)
		{
			var result =((CssSelector)query).Select(this).OfType<HtmlElement>().ToList();
			return new NodeList(() => result);
		}
			

		/// <summary> Returns nearest ancestor or itself which satisfies to specified selector. </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public Element Closest(string query)
		{
			var selector = new CssSelector(query);
			return this.GetRecursive(x => (Element) x.ParentNode).FirstOrDefault(selector.IsMatches);
		}

		/// <summary> Returns the size of an element and its position relative to the viewport. </summary>
		/// <returns><see cref="DomRect"/> object.</returns>
		public DomRect GetBoundingClientRect() =>
			OwnerDocument.GetElementBounds is Func<Element, RectangleF[]> getElementBounds 
				? DomRect.FromRectangleF(Surround(getElementBounds(this))) 
				: DomRect.Empty;

		private RectangleF Surround(RectangleF[] rects)
		{
			if(rects.Length == 0)
				return RectangleF.Empty;
			
			if (rects.Length == 1)
				return rects[0];

			return rects.Aggregate((r1, r2) =>
			{
				var left = Math.Min(
					Math.Min(r1.Left, r1.Left + r1.Width),
					Math.Min(r2.Left, r2.Left + r2.Width));
				var top = Math.Min(
					Math.Min(r1.Top, r1.Top+r1.Height),
					Math.Min(r2.Top, r2.Top+r2.Height));
				
				var right = 
					Math.Max(Math.Max(r1.Left, r1.Left+r1.Width),
						Math.Max(r2.Left, r2.Left+r2.Width));
				var bottom = Math.Max(Math.Max(r1.Top, r1.Top+r1.Height),
					Math.Max(r2.Top, r2.Top+r2.Height));
				
				return new RectangleF(
					left,top,
					right-left,
					bottom-top);
			});
		}


		/// <summary>
		/// Returns a collection of DOMRect objects that indicate the bounding rectangles for each CSS border box in a client.
		/// Most elements only have one border box each, but a multiline inline element
		/// (such as a multiline 'span' element, by default) has a border box around each line.
		/// </summary>
		/// <returns></returns>
		public DomRect[] GetClientRects() =>
			OwnerDocument.GetElementBounds is Func<Element, RectangleF[]> getElementBounds 
				? getElementBounds(this).Select(DomRect.FromRectangleF).ToArray() 
				: new[]{DomRect.Empty};


		/// <summary> Removes this node from its parent. </summary>
		public void Remove() => ParentNode?.RemoveChild(this);
	}

	[JsName("DOMRect")]
	public class DomRect
	{
		internal static DomRect Empty = new DomRect(0, 0, 0, 0);

		internal static DomRect FromRectangleF(RectangleF rect) => new DomRect(rect.X, rect.Y, rect.Width, rect.Height);
		
		public DomRect(float x, float y, float width, float height)
		{
			Width = width;
			Height = height;
			X = x;
			Y = y;
			Top = height >= 0 ? y : y + height;
			Bottom = height > 0 ? y + height : y;
			Left = width >= 0 ? x : x + width;
			Right = width > 0 ? x + width : x;
		}
		
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