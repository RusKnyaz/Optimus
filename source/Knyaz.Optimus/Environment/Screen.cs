using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Environment
{
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