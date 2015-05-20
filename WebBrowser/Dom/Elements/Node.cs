using System;
using System.Collections.Generic;
using System.Linq;

namespace WebBrowser.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/DOM-Level-3-Core/core.html#ID-1950641247
	/// </summary>
	public abstract class Node : INode
	{
		private Document _ownerDocument;

		public Document OwnerDocument
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

		public INode AppendChild(INode node)
		{
			if (node is DocumentFragment)
			{
				foreach (var child in node.ChildNodes)
				{
					AppendChild(child);
				}
			}
			else
			{
				UnattachFromParent(node);
				ChildNodes.Add(node);
				node.Parent = this;
				node.OwnerDocument = OwnerDocument;
			}
			return node;
		}

		private void UnattachFromParent(INode node)
		{
			if (node.Parent != null)
				node.Parent.ChildNodes.Remove(node);
		}

		protected Node()
		{
			InternalId = Guid.NewGuid().ToString();
			ChildNodes = new List<INode>();
		}

		public IList<INode> ChildNodes { get; protected set; }
		public string InternalId { get; private set; }
		public string Id { get; set; }

		public Node RemoveChild(Node node)
		{
			ChildNodes.Remove(node);
			return node;
		}

		public Node InsertBefore(Node newChild, Node refNode)
		{
			UnattachFromParent(newChild);
			ChildNodes.Insert(ChildNodes.IndexOf(refNode), newChild);
			newChild.Parent = this;
			newChild.OwnerDocument = OwnerDocument;
			return newChild;
		}

		public bool HasChildNodes { get { return ChildNodes.Count > 0; } }

		public Node ReplaceChild(Node newChild, Node oldChild)
		{
			InsertBefore(newChild, oldChild);
			RemoveChild(oldChild);
			newChild.Parent = this;
			newChild.OwnerDocument = OwnerDocument;
			return newChild;
		}

		public INode FirstChild { get { return ChildNodes.FirstOrDefault(); } }
		public INode LastChild { get { return ChildNodes.LastOrDefault(); } }
		public INode NextSibling { get
		{
			if (Parent == null)
				return null;
			
			var idx = Parent.ChildNodes.IndexOf(this);
			if (idx == Parent.ChildNodes.Count - 1)
				return null;
			return Parent.ChildNodes[idx + 1];} }

		public INode PreviousSibling
		{
			get
			{
				var idx = Parent.ChildNodes.IndexOf(this);
				if (idx == 0)
					return null;
				return Parent.ChildNodes[idx- 1];
			}
		}

		public INode Parent { get; set; }
		public abstract INode CloneNode();

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
			throw new NotImplementedException();
		}
		public void RemoveEventListener(string type, Action<Event> listener, bool useCapture)
		{
			throw new NotImplementedException();
		}

		public bool DispatchEvent(Event evt)
		{
			if (OnEvent != null)
				OnEvent(evt);
			return true;//todo: what we should return?
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
			if (node.OwnerDocument != OwnerDocument)
				return 1;

			throw new NotImplementedException();
		}

		public event Action<Event> OnEvent;
	}

	public class Event
	{
		public string Type;
		public Node Target;
		public ushort EventPhase;
		public bool Bubbles;
		public bool Cancellable;
		public void StopPropagation()
		{
			throw new NotImplementedException();
		}

		public void PreventDefault()
		{
			throw new NotImplementedException();
		}

		public void InitEvent(string type, bool canBubble, bool canCancel)
		{
			Type = type;
			Cancellable = canCancel;
			Bubbles = canBubble;
		}

		//todo: implement remains properties
	}

	
}