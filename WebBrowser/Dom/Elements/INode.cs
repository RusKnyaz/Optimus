using System.Collections.Generic;
using System.Linq;

namespace WebBrowser.Dom.Elements
{
	public interface INode
	{
		IList<INode> ChildNodes { get; }
		string InternalId { get; }
		string Id { get; set; }
		INode Parent { get; set; }
		Document OwnerDocument { get; set; }
		INode CloneNode();
	}

	
	public static class DocumentElementExtension
	{
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
