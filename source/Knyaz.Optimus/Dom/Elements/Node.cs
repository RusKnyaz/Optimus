using System;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Dom.Elements
{
	internal enum NodeSources
	{
		Script,
		DocumentBuilder
	}

	/// <summary>
	/// http://www.w3.org/TR/DOM-Level-3-Core/core.html#ID-1950641247
	/// </summary>
	/// <inheritdoc cref="INode"/>
	public abstract class Node : INode, IEventTarget
	{
		protected EventTarget EventTarget;

		internal NodeSources Source;

		protected Node(Document ownerDocument)
		{
			_ownerDocument = ownerDocument;
			ChildNodes = new List<Node>();
			EventTarget = new EventTarget(this, () => ParentNode, () => OwnerDocument ?? this as Document);
		}
	
		private Document _ownerDocument;

		public virtual Document OwnerDocument
		{
			get { return _ownerDocument; }
			set { }
		}

		internal virtual void SetOwner(Document doc)
		{
			_ownerDocument = doc;
			foreach (var childNode in ChildNodes)
				childNode.SetOwner(doc);
		}

		public virtual Node AppendChild(Node node)
		{
			if(node == this)
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
				ChildNodes.Add(node);
				RegisterNode(node);
			}
			return node;
		}

		protected virtual void RegisterNode(Node node)
		{
			node.ParentNode = this;
			var owner = OwnerDocument ?? this as Document;
			if (owner != null)
			{
				node.SetOwner(owner);
				owner.HandleNodeAdded(node);
			}
		}

		private void UnattachFromParent(Node node)
		{
			if (node.ParentNode != null)
				node.ParentNode.ChildNodes.Remove(node);
		}

		protected Node()
		{
			ChildNodes = new List<Node>();
			NodeType = _NODE;
		}

		/// <summary>
		/// Gets a live collection of child nodes of the given element.
		/// </summary>
		public IList<Node> ChildNodes { get; protected set; }

		/// <summary>
		/// Removes a child node from the DOM.
		/// </summary>
		/// <param name="node">The node to remove.</param>
		/// <returns>The removed node.</returns>
		public Node RemoveChild(Node node)
		{
			ChildNodes.Remove(node);
			OwnerDocument?.HandleNodeRemoved(this, node);
			return node;
		}

		/// <summary>
		/// Inserts a node before the reference node as a child of a specified parent node. 
		/// If the given child is a reference to an existing node in the document, the method moves it from its current position to the new position.
		/// If the reference node is null, the specified node is added to the end of the list of children of the specified parent node.
		/// If the given child is a DocumentFragment, the entire contents of the DocumentFragment are moved into the child list of the specified parent node.
		/// </summary>
		/// <param name="newChild"></param>
		/// <param name="refNode"></param>
		/// <returns></returns>
		public Node InsertBefore(Node newChild, Node refNode)
		{
			UnattachFromParent(newChild);
			if (refNode == null)
				ChildNodes.Add(newChild);
			else
				ChildNodes.Insert(ChildNodes.IndexOf(refNode), newChild);
			RegisterNode(newChild);
			return newChild;
		}

		/// <summary>
		/// Indicating whether the current Node has child nodes or not.
		/// </summary>
		public bool HasChildNodes { get { return ChildNodes.Count > 0; } }

		/// <summary>
		/// Replaces one child node of the specified node with another.
		/// </summary>
		/// <param name="newChild">The node to be added.</param>
		/// <param name="oldChild">The node to be removed</param>
		/// <returns>The removed node.</returns>
		public Node ReplaceChild(Node newChild, Node oldChild)
		{
			InsertBefore(newChild, oldChild);
			RemoveChild(oldChild);
			return newChild;
		}

		/// <summary>
		/// Gets the node's first child in the tree, or null if the node has no children. If the node is a Document, it returns the first node in the list of its direct children.
		/// </summary>
		public Node FirstChild => ChildNodes.FirstOrDefault();

		/// <summary>
		/// Gets the last child of the node. If its parent is an element, then the child is generally an element node, a text node, or a comment node. It returns null if there are no child elements.
		/// </summary>
		public Node LastChild => ChildNodes.LastOrDefault();

		/// <summary>
		/// Gets the node immediately following the specified one in its parent's childNodes list, or null if the specified node is the last node in that list.
		/// </summary>
		public Node NextSibling
		{
			get
			{
				if (ParentNode == null)
					return null;

				var idx = ParentNode.ChildNodes.IndexOf(this);
				if (idx == ParentNode.ChildNodes.Count - 1)
					return null;
				return ParentNode.ChildNodes[idx + 1];
			}
		}

		/// <summary>
		/// Gets the node immediately preceding the specified one in its parent's childNodes list, or null if the specified node is the first in that list.
		/// </summary>
		public Node PreviousSibling
		{
			get
			{
				if (ParentNode == null)
					return null;

				var idx = ParentNode.ChildNodes.IndexOf(this);
				if (idx == 0)
					return null;
				return ParentNode.ChildNodes[idx- 1];
			}
		}

		/// <summary>
		/// Gets the parent of the specified node in the DOM tree.
		/// </summary>
		public Node ParentNode { get; private set; }

		/// <summary>
		/// Creates a duplicate of the node on which this method was called.
		/// </summary>
		/// <returns></returns>
		public Node CloneNode()
		{
			return CloneNode(false);
		}

		/// <summary>
		/// Creates a duplicate of the node on which this method was called.
		/// </summary>
		/// <param name="deep">If <c>true</c> the children of the node will also be cloned.</param>
		/// <returns>Newly created node.</returns>
		public abstract Node CloneNode(bool deep);

		/// <summary>
		/// Gets the type of the node.
		/// </summary>
		public int NodeType { get; protected set; }

		/// <summary>
		/// Gets name of the current node as a string.
		/// The returned values for different types of nodes are:
		/// Attribute - The value of Attr.name
		/// CDATASection - "#cdata-section"
		/// Comment - "#comment"
		/// Document - "#document"
		/// DocumentFragment - "#document-fragment"
		/// DocumentType - The value of DocumentType.name
		/// Element - The value of Element.tagName
		/// Entity - The entity name
		/// EntityReference - The name of entity reference
		/// Notation - The notation name
		/// ProcessingInstruction - The value of ProcessingInstruction.target
		/// Text - "#text"
		/// </summary>
		public abstract string NodeName { get; }

		public const ushort ELEMENT_NODE = 1;
		public const ushort _NODE = 2;
		public const ushort TEXT_NODE = 3;
		public const ushort CDATA_SECTION_NODE = 4;
		public const ushort ENTITY_REFERENCE_NODE = 5;
		public const ushort ENTITY_NODE = 6;
		public const ushort PROCESSING_INSTRUCTION_NODE = 7;
		public const ushort COMMENT_NODE = 8;
		public const ushort DOCUMENT_NODE = 9;
		public const ushort DOCUMENT_TYPE_NODE = 10;
		public const ushort DOCUMENT_FRAGMENT_NODE = 11;
		public const ushort NOTATION_NODE = 12;

		public void AddEventListener(string type, Action<Event> listener)
		{
			EventTarget.AddEventListener(type,  listener);
		}

		public void AddEventListener(string type, Action<Event> listener, bool useCapture)
		{
			EventTarget.AddEventListener(type, listener, useCapture);
		}

		public void RemoveEventListener(string type, Action<Event> listener)
		{
			EventTarget.RemoveEventListener(type, listener);
		}

		public void RemoveEventListener(string type, Action<Event> listener, bool useCapture)
		{
			EventTarget.RemoveEventListener(type, listener, useCapture);
		}

		/// <summary>
		/// This method allows the dispatch of events into the implementations event model. 
		/// Events dispatched in this manner will have the same capturing and bubbling behavior as events dispatched directly by the implementation. The target of the event is the EventTarget on which dispatchEvent is called.
		/// </summary>
		/// <returns> If preventDefault was called the value is false, else the value is true.</returns>
		public virtual bool DispatchEvent(Event evt) => EventTarget.DispatchEvent(evt);

		/// <summary>
		/// 1: No relationship, the two nodes do not belong to the same document.
		///2: The first node (p1) is positioned after the second node (p2).
		///4: The first node (p1) is positioned before the second node (p2).
		///8: The first node (p1) is positioned inside the second node (p2).
		///16: The second node (p2) is positioned inside the first node (p1).
		///32: No relationship, or the two nodes are two attributes on the same element.
		///Note: The return value could also be a combination of values. I.e. the returnvalue 20 means that p2 is inside p1 (16) AND p1 is positioned before p2 (4).
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public int CompareDocumentPosition(Node node)
		{
			//todo: rewrite the sketch, consider to move to Extension

			if (node.OwnerDocument != OwnerDocument 
				|| (ParentNode == null && node.ParentNode == null))
				return 1;

			if (this is Element && node is Attr)
			{
				var el = (Element) this;
				var attr = (Attr)node;
				return el.GetAttributeNode(attr.Name) == attr 
					? 20 
					: CompareDocumentPosition(attr.OwnerElement) & (255 - 8);
			}
			
			if(this is Attr && node is Element)
			{
				var attr = (Attr) this;
				var el = (Element) node;
				return el.GetAttributeNode(attr.Name) == attr 
					? 10 : 
					attr.OwnerElement.CompareDocumentPosition(el) & (255 - 8);
			}

			if (this is Attr && node is Attr)
			{
				var attr1 = (Attr) this;
				var attr2 = (Attr) node;
				if (attr1.OwnerElement == attr2.OwnerElement)
				{
					var attrList = attr1.OwnerElement.Attributes.ToList();
					return attrList.IndexOf(attr1) > attrList.IndexOf(attr2) ? 2 : 4;
				}
				return attr1.OwnerElement.CompareDocumentPosition(attr2.OwnerElement);
			}

			if (ParentNode == node.ParentNode)
			{
				return ParentNode.ChildNodes.IndexOf(this) > ParentNode.ChildNodes.IndexOf(node) ? 2 : 4;
			}

			//Search for shared ancestors
			var thisAncestors = new List<Node>();
			var otherAncestors = new List<Node>();

			for (var p = (Node)this; p != null; p = p.ParentNode )
				otherAncestors.Add(p);

			for (var p = (Node)node; p != null && !otherAncestors.Contains(p); p = p.ParentNode)
				thisAncestors.Add(p);

			//node placed inside
			if (thisAncestors.Count == 0)
				return 10;

			var sharedParent = thisAncestors.Last().ParentNode;

			if (sharedParent == this)
				return 20;

			var sharedParentIndex = otherAncestors.IndexOf(sharedParent);

			//this placed inside node
			if (sharedParentIndex == 0)
				return 2;

			var thisPreParent = thisAncestors[thisAncestors.Count - 1];
			var otherPreParent = otherAncestors[sharedParentIndex - 1];

			return thisPreParent.CompareDocumentPosition(otherPreParent);
		}
	}
}