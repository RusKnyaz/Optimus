using System.Drawing;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus
{
	/// <summary> Represents external layout service which used int getBoundingClientRect method. </summary>
	public interface ILayoutService
	{
		RectangleF[] GetElementBounds(Element element);
	}
}