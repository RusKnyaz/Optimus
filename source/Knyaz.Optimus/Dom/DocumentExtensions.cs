using System;
using System.Drawing;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.Dom
{
	public static class DocumentExtensions
	{
		/// <summary> Adds layout service to enable GetClientRects function. </summary>
		/// <param name="document"></param>
		/// <param name="layoutService"></param>
		public static void AttachLayoutService(this HtmlDocument document, ILayoutService layoutService)
		{
			document.GetElementBounds = 
				layoutService != null ? layoutService.GetElementBounds 
					: (Func<Element, RectangleF[]>)null;
		}
	}
}