using System;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Tools;

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
	public abstract class Node : IEventTarget
	{
		private readonly EventTarget  EventTarget;
		internal NodeSources Source;

		protected Node(Document ownerDocument = null)
		{
			_ownerDocument = ownerDocument;
			NodeType = _NODE;
			EventTarget = new EventTarget(this, () => 
					this is Document doc ? (IEventTarget)doc.DefaultView : ParentNode?.EventTarget, 
				() => OwnerDocument ?? this as Document);
			
			EventTarget.BeforeEventDispatch += x => BeforeEventDispatch(x);
			EventTarget.CallDirectEventSubscribers += x => CallDirectEventSubscribers(x);
			EventTarget.AfterEventDispatch += x => AfterEventDispatch(x);
			ChildNodes = new NodeList(() => Children);
		}
		
		protected virtual void BeforeEventDispatch(Event evt) {}
		protected virtual void CallDirectEventSubscribers(Event evt) {}
		protected virtual void AfterEventDispatch(Event evt) {}

		private Document _ownerDocument;

		public virtual Document OwnerDocument
		{
			get { return _ownerDocument; }
			set { }
		}
		
		[JsName("namespaceURI")]
		public string NamespaceUri { get; internal set; }

		internal virtual void SetOwner(Document doc) => _ownerDocument = doc;

		public virtual Node AppendChild(Node node) => throw new NotSupportedException();

		protected void RegisterNode(Node node)
		{
			node.ParentNode = this;
			var owner = OwnerDocument ?? this as Document;
			if (owner == null) return;
			node.SetOwner(owner);
			owner.HandleNodeAdded(node);
		
			if (node.Source != NodeSources.DocumentBuilder)
				RaiseDomNodeInserted(node);
		}
		
		private void RaiseDomNodeInserted(Node newChild)
		{
			var owner = OwnerDocument ?? this as Document;
			var evt = (MutationEvent)owner.CreateEvent("MutationEvent");
			evt.InitMutationEvent("DOMNodeInserted", true, false, newChild.ParentNode, null, null, null, 0);
			newChild.DispatchEvent(evt);
		}

		protected void UnattachFromParent(Node node) => node.ParentNode?.RemoveChild(node);

		private static readonly List<Node> emptyList = new List<Node>(0);

		internal protected virtual IEnumerable<Node> Children => emptyList;
			

		/// <summary>
		/// Gets a live collection of child nodes of the given element.
		/// </summary>
		public virtual NodeList ChildNodes { get; }

		/// <summary>
		/// Removes a child node from the DOM.
		/// </summary>
		/// <param name="node">The node to remove.</param>
		/// <returns>The removed node.</returns>
		public virtual Node RemoveChild(Node node) => throw new NotSupportedException();

		/// <summary>
		/// Inserts a node before the reference node as a child of a specified parent node. 
		/// If the given child is a reference to an existing node in the document, the method moves it from its current position to the new position.
		/// If the reference node is null, the specified node is added to the end of the list of children of the specified parent node.
		/// If the given child is a DocumentFragment, the entire contents of the DocumentFragment are moved into the child list of the specified parent node.
		/// </summary>
		/// <param name="newChild"></param>
		/// <param name="refNode"></param>
		/// <returns></returns>
		public virtual Node InsertBefore(Node newChild, Node refNode) =>
			throw new NotSupportedException();

		/// <summary>
		/// Indicating whether the current Node has child nodes or not.
		/// </summary>
		public virtual bool HasChildNodes => false;

		/// <summary>
		/// Replaces one child node of the specified node with another.
		/// </summary>
		/// <param name="newChild">The node to be added.</param>
		/// <param name="oldChild">The node to be removed</param>
		/// <returns>The removed node.</returns>
		public virtual Node ReplaceChild(Node newChild, Node oldChild) =>
			throw new NotSupportedException();

		/// <summary>
		/// Gets the node's first child in the tree, or null if the node has no children. If the node is a Document, it returns the first node in the list of its direct children.
		/// </summary>
		public virtual Node FirstChild => null;

		/// <summary>
		/// Gets the last child of the node. If its parent is an element, then the child is generally an element node, a text node, or a comment node. It returns null if there are no child elements.
		/// </summary>
		public virtual Node LastChild => null;

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
				return idx == ParentNode.ChildNodes.Count - 1 ? null 
					: ParentNode.ChildNodes[idx + 1];
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

		#region .    constants    .
		/// <summary>
		/// 1
		/// </summary>
		public const ushort ELEMENT_NODE = 1;
		/// <summary>
		/// 2
		/// </summary>
		public const ushort _NODE = 2;
		/// <summary>
		/// 3
		/// </summary>
		public const ushort TEXT_NODE = 3;
		/// <summary>
		/// 4
		/// </summary>
		public const ushort CDATA_SECTION_NODE = 4;
		/// <summary>
		/// 5
		/// </summary>
		public const ushort ENTITY_REFERENCE_NODE = 5;
		/// <summary>
		/// 6
		/// </summary>
		public const ushort ENTITY_NODE = 6;
		/// <summary>
		/// 7
		/// </summary>
		public const ushort PROCESSING_INSTRUCTION_NODE = 7;
		/// <summary>
		/// 8
		/// </summary>
		public const ushort COMMENT_NODE = 8;
		/// <summary>
		/// 9
		/// </summary>
		public const ushort DOCUMENT_NODE = 9;
		/// <summary>
		/// 10
		/// </summary>
		public const ushort DOCUMENT_TYPE_NODE = 10;
		/// <summary>
		/// 11
		/// </summary>
		public const ushort DOCUMENT_FRAGMENT_NODE = 11;
		/// <summary>
		/// 12
		/// </summary>
		public const ushort NOTATION_NODE = 12;
		#endregion

		/// <summary>
		/// Registers new event handler.
		/// </summary>
		/// <param name="type">The type name of the event.</param>
		/// <param name="listener">The event handler.</param>
		/// <param name="options">An options object that specifies characteristics about the event listener. </param>
		public void AddEventListener(string type, Action<Event> listener, EventListenerOptions options) =>
			EventTarget.AddEventListener(type, listener, options);

		/// <summary>
		/// Registers new event handler in 'bubbling' order.
		/// </summary>
		/// <param name="type">The type name of the event.</param>
		/// <param name="listener">The event handler.</param>
		public void AddEventListener(string type, Action<Event> listener) =>
			EventTarget.AddEventListener(type,  listener);

		/// <summary>
		/// Registers new event handler.
		/// </summary>
		/// <param name="type">The type name of the event.</param>
		/// <param name="listener">The event handler.</param>
		/// <param name="useCapture">If <c>true</c> the handler invoked in 'capturing' order, 
		/// otherwise in the handler invoked in 'bubbling' order.</param>
		public void AddEventListener(string type, Action<Event> listener, bool useCapture) =>
			EventTarget.AddEventListener(type, listener, useCapture);

		/// <summary>
		/// Removes previously registered event handler from 'bubbling' order handlers list.
		/// </summary>
		/// <param name="type">The type name of event.</param>
		/// <param name="listener">The handler to be removed.</param>
		public void RemoveEventListener(string type, Action<Event> listener) =>
			EventTarget.RemoveEventListener(type, listener);
		
		/// <summary>
		/// Removes previously registered event handler.
		/// </summary>
		/// <param name="type">The type name of event.</param>
		/// <param name="listener">The handler to be removed.</param>
		/// <param name="options">The options with which the listener was added.</param>
		public void RemoveEventListener(string type, Action<Event> listener, EventListenerOptions options) =>
			EventTarget.RemoveEventListener(type, listener, options);

		/// <summary>
		/// Removes previously registered event handler.
		/// </summary>
		/// <param name="type">The type name of event.</param>
		/// <param name="listener">The handler to be removed.</param>
		/// <param name="useCapture">The invocation order to be handler removed from.</param>
		public void RemoveEventListener(string type, Action<Event> listener, bool useCapture) =>
			EventTarget.RemoveEventListener(type, listener, useCapture);

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
		///Note: The return value could also be a combination of values. I.e. the return value 20 means that p2 is inside p1 (16) AND p1 is positioned before p2 (4).
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

			for (var p = this; p != null; p = p.ParentNode )
				otherAncestors.Add(p);

			for (var p = node; p != null && !otherAncestors.Contains(p); p = p.ParentNode)
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