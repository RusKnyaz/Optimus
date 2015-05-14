namespace WebBrowser.Environment
{
	public class Window
	{
		public Window()
		{
			Screen = new Screen
				{
					Width = 1024,
					Height = 768,
					AvailWidth = 1024,
					AvailHeight = 768,
					ColorDepth = 24,
					PixelDepth = 24
				};

			InnerWidth = 1024;
			InnerHeight = 768;
			Location = new Location{Href = "http://localhost"};//todo: remove the stub href value
			Navigator = new Navigator();
		}

		public int InnerWidth { get; set; }
		public int InnerHeight { get; set; }

		public Screen Screen { get; private set; }
		public Location Location { get; private set; }
		public Navigator Navigator { get; private set; }
	}
}
