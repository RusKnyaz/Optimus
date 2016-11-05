using System;

namespace Knyaz.Optimus.Dom.Css
{
	public class MediaQueryList
	{
		internal MediaQueryList(string media, Func<MediaSettings> getMedia)
		{
			Media = media;
		}

		public string Media { get; private set; }

		public bool Matches
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}

	internal class MediaSettings
	{
		public string Device;
		public int Width;
		public bool Landscape;
	}
}
