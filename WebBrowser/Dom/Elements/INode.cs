using System.Collections.Generic;
using System.Linq;

namespace WebBrowser.Dom.Elements
{
	public static class DocumentElementExtension
	{
		public static IEnumerable<Node> Flatten(this Node elem)
		{
			yield return elem;
			foreach (var documentElement in elem.ChildNodes.SelectMany(x => x.Flatten()))
			{
				yield return documentElement;
			}
		} 
	}
}
