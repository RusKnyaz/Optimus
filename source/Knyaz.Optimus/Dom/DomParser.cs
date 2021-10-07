using System;
using System.IO;
using System.Linq;
using System.Text;
using Knyaz.Optimus.Html;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom
{
	/// <summary> Implements parsing of html or xml document. </summary>
	[JsName("DOMParser")]
	public class DomParser
	{
		public Document ParseFromString(string content, string mimeType)
		{
			if (string.IsNullOrEmpty(mimeType))
				throw new ArgumentOutOfRangeException(nameof(mimeType));
			
			if (mimeType == "text/html")
			{
				var document = new HtmlDocument();
				using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
				{
					var html = HtmlParser.Parse(stream).ToList();
					DocumentBuilder.Build(document, html);
					document.Complete();
				}

				return document;
			}

			throw new NotImplementedException("Unsupported mime type: " + mimeType);
		}
	}
}