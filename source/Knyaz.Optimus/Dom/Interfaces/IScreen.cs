using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Interfaces
{
	[DomItem]
	public interface IScreen
	{
		int Width { get; set; }
		int Height { get; set; }
		int AvailWidth { get; set; }
		int AvailHeight { get; set; }
		int ColorDepth { get; set; }
		int PixelDepth { get; set; }
	}
}
