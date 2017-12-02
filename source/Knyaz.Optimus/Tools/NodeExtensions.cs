using Knyaz.Optimus.Dom.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Knyaz.Optimus.Tools
{
	/// <summary>
	/// Helper class for work with Element
	/// </summary>
	static class DocumentElementExtension
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
	}
}
