using Knyaz.Optimus.Dom.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.Tools
{
	/// <summary>
	/// Helper class for work with Element
	/// </summary>
	static class NodeExtension
	{
		/// <summary>
		/// Gets all descendants of a given node with the node itself.
		/// </summary>
		/// <returns>The original node and all of its descendants.</returns>
		public static IEnumerable<INode> Flatten(this INode elem)
		{
			yield return elem;
			foreach (var documentElement in elem.ChildNodes.SelectMany(x => x.Flatten()))
			{
				yield return documentElement;
			}
		}
		
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
		public static bool IsInDocument(this INode node)
		{
			var praParent = node;
			while (praParent.ParentNode != null)
				praParent = praParent.ParentNode;

			return praParent == node.OwnerDocument;
		}

		/// <summary>
		/// Retrieves all node ancestors.
		/// </summary>
		public static IEnumerable<INode> Ancestors(this INode node)
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
