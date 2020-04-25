using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using System;

namespace Knyaz.Optimus.Dom.Interfaces
{
	public interface IDocument : INode
	{
		Element CreateElement(string tagName);
		Element DocumentElement { get; }
		void Write(string text);
		Event CreateEvent(string type);
		Head Head { get; }
		HtmlBodyElement Body { get; set; }
		Comment CreateComment(string data);
		Text CreateTextNode(string data);
		DocumentFragment CreateDocumentFragment();
		Element GetElementById(string id);
		Attr CreateAttribute(string name);
		//not a part of public API
		event Action<Node> NodeInserted;
		event Action<IDocument> DomContentLoaded;
		string Title { get; set; }
		object ActiveElement { get; set; }
	}
}
