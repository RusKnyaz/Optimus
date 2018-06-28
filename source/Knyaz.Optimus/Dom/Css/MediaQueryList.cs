using System;
using System.Linq;

namespace Knyaz.Optimus.Dom.Css
{
	/// <summary>
	/// Stores information on a media query applied to a document, 
	/// </summary>
	public class MediaQueryList
	{
		private readonly Func<MediaSettings> _getMedia;

		internal MediaQueryList(string media, Func<MediaSettings> getMedia)
		{
			_getMedia = getMedia;
			Media = media;
		}

		/// <summary>
		/// Gets a serialized media query.
		/// </summary>
		public string Media { get; private set; }

		/// <summary>
		/// Returns <c>true</c> if the document currently matches the media query list, or <c>false</c> if not.
		/// </summary>
		public bool Matches => Media.Split(',').Select(x => x.Trim()).Any(MatchMedia);

		private bool MatchMedia(string media)
		{
			var curMedia = _getMedia();

			media = media.ToLowerInvariant();

			var conditions = media.Split('(').Select(x => x.Trim()).ToArray();

			if (conditions.Length == 0)
				return false;

			var deviceCondition = conditions[0].Split(' ')[0];

			if (deviceCondition != string.Empty && deviceCondition != "all" && curMedia.Device.ToLowerInvariant() != deviceCondition)
				return false;

			for (var i = 1; i < conditions.Length; i++)
			{
				var condition = conditions[i].Split(')')[0].Split(':');
				var property = condition[0].Trim();
				switch (property)
				{
					case "orientation":
						if((condition[1].Trim() == "landscape") != curMedia.Landscape)
							return false;
						break;
					case "min-width":
						if (int.Parse(new string(condition[1].Trim().TakeWhile(x => char.IsDigit(x)).ToArray())) > curMedia.Width)
							return false;
						break;
					case "max-width":
						if (int.Parse(new string(condition[1].Trim().TakeWhile(x => char.IsDigit(x)).ToArray())) < curMedia.Width)
							return false;
						break;
					case "transform-3d":
						if(!curMedia.Transform3D)
							return false;
						break;
					default: return false;
				}
			}

			return true;
		}
	}

	public class MediaSettings
	{
		public string Device = "Screen";
		public int Width;
		public bool Landscape;
		public bool Transform3D;
	}
}
