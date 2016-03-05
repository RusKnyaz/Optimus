using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Environment
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

	/// <summary>
	/// http://www.w3schools.com/js/js_window_screen.asp
	/// </summary>
	public class Screen : IScreen
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public int AvailWidth { get; set; }
		public int AvailHeight { get; set; }
		public int ColorDepth { get; set; }
		public int PixelDepth { get; set; }
	}
}