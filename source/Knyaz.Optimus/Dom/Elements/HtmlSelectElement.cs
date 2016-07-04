using System.Linq;

namespace Knyaz.Optimus.Dom.Elements
{
	public class HtmlSelectElement : HtmlElement
	{
		public HtmlSelectElement(Document ownerDocument) : base(ownerDocument, TagsNames.Select)
		{
		}

		public override Node AppendChild(Node node)
		{
			if (!(node is HtmlOptionElement))
				return node;

			return base.AppendChild(node);
		}

		public void Remove(int index)
		{
			var options = ChildNodes.ToArray();
			if (index >= 0 && index < options.Length)
			{
				RemoveChild(options[index]);
			}
		}

		public int Length { get { return ChildNodes.Count; } }
	}
}
