using System;
using System.Collections.Generic;
using System.Linq;
using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom.Elements
{
	public enum NodeSources
	{
		Script,
		DocumentBuilder
	}

	/// <summary>
	/// http://www.w3.org/TR/DOM-Level-3-Core/core.html#ID-1950641247
	/// </summary>
	public abstract class Node : INode, IEventTarget
	{
		protected EventTarget EventTarget;

		public NodeSources Source;

		protected Node(Document ownerDocument)
		{
			_ownerDocument = ownerDocument;
			ChildNodes = new List<Node>();
			EventTarget = new EventTarget(this, () => ParentNode, () => OwnerDocument);
		}
	
		private Document _ownerDocument;

		public virtual Document OwnerDocument
		{
			get { return _ownerDocument; }
			set
			{
				_ownerDocument = value;
				foreach (var childNode in ChildNodes)
				{
					childNode.OwnerDocument = value;
				}
			}
		}

		public Node AppendChild(Node node)
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

		private void RegisterNode(Node node)
		{
			node.ParentNode = this;
			node.OwnerDocument = OwnerDocument;
			OwnerDocument.HandleNodeAdded(node);
		}

		private void UnattachFromParent(Node node)
		{
			if (node.ParentNode != null)
				node.ParentNode.ChildNodes.Remove(node);
		}

		protected Node()
		{
			InternalId = Guid.NewGuid().ToString();
			ChildNodes = new List<Node>();
			NodeType = _NODE;
		}

		public IList<Node> ChildNodes { get; protected set; }
		public string InternalId { get; private set; }
		
		public Node RemoveChild(Node node)
		{
			ChildNodes.Remove(node);
			return node;
		}

		public Node InsertBefore(Node newChild, Node refNode)
		{
			UnattachFromParent(newChild);
			ChildNodes.Insert(ChildNodes.IndexOf(refNode), newChild);
			RegisterNode(newChild);
			return newChild;
		}

		public bool HasChildNodes { get { return ChildNodes.Count > 0; } }

		public Node ReplaceChild(Node newChild, Node oldChild)
		{
			InsertBefore(newChild, oldChild);
			RemoveChild(oldChild);
			return newChild;
		}

		public Node FirstChild { get { return ChildNodes.FirstOrDefault(); } }
		public Node LastChild { get { return ChildNodes.LastOrDefault(); } }
		public Node NextSibling { get
		{
			if (ParentNode == null)
				return null;
			
			var idx = ParentNode.ChildNodes.IndexOf(this);
			if (idx == ParentNode.ChildNodes.Count - 1)
				return null;
			return ParentNode.ChildNodes[idx + 1];} }

		public Node PreviousSibling
		{
			get
			{
				var idx = ParentNode.ChildNodes.IndexOf(this);
				if (idx == 0)
					return null;
				return ParentNode.ChildNodes[idx- 1];
			}
		}

		public Node ParentNode { get; set; }
		public Node CloneNode()
		{
			return CloneNode(false);
		}
		public abstract Node CloneNode(bool deep);

		public int NodeType { get; protected set; }
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

		public void AddEventListener(string type, Action<Event> listener, bool useCapture)
		{
			EventTarget.AddEventListener(type, listener, useCapture);
		}

		public void RemoveEventListener(string type, Action<Event> listener, bool useCapture)
		{
			EventTarget.RemoveEventListener(type, listener, useCapture);
		}

		public virtual bool DispatchEvent(Event evt)
		{
			return EventTarget.DispatchEvent(evt);
		}

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

	[DomItem]
	public interface INode
	{
		Document OwnerDocument { get; }
		Node AppendChild(Node node);
		Node RemoveChild(Node node);
		Node InsertBefore(Node newChild, Node refNode);
		bool HasChildNodes { get; }
		Node ReplaceChild(Node newChild, Node oldChild);
		Node FirstChild { get; }
		Node LastChild { get; }
		Node NextSibling { get; }
		Node PreviousSibling { get; }
		Node ParentNode { get; }
		Node CloneNode();
		int NodeType { get; }
		string NodeName { get; }
		int CompareDocumentPosition(Node node);
		IList<Node> ChildNodes { get;}
	}

	public static class NodeExtension
	{
		public static bool RaiseEvent(this Node node, string eventType, bool bubblable, bool cancellable)
		{
			var e = node.OwnerDocument.CreateEvent("Event");
			e.InitEvent(eventType, bubblable, cancellable);
			return node.DispatchEvent(e);
		}

		/// <summary>
		/// Determines if the node lies to document hierarhcy.
		/// </summary>
		/// <returns><c>true</c> if is in document the specified node; otherwise, <c>false</c>.</returns>
		/// <param name="node">Node.</param>
		public static bool IsInDocument(this Node node)
		{
			var praParent = node;
			while (praParent.ParentNode != null)
				praParent = praParent.ParentNode;

			return praParent == node.OwnerDocument;
		}

		public static IEnumerable<Node> Ancestors(this Node node)
		{
			var parent = node.ParentNode;
			while (parent != null)
			{
				yield return parent;
				parent = parent.ParentNode;
			}
		}
	}
}