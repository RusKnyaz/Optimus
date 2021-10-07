using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom
{
	[JsName("XMLDocument")]
	public class XmlDocument : Document
	{
		internal XmlDocument(string namespaceUri, string qualifiedNameStr, DocType documentType, IWindow window) 
			: base(namespaceUri, qualifiedNameStr, documentType, window)
		{
		}
	}
}