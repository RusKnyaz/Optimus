using System.IO;

namespace Knyaz.Optimus.ResourceProviders
{
	public interface IResource
	{
		string Type { get; }
		Stream Stream { get; }
	}

	public static class ResourceTypes
	{
		public const string Html = "text/html";
		public const string Text = "text/plain";
		public const string Css = "text/css";
		public const string JavaScript = "text/javascript";
		public const string Bmp = "image/bmp";
		public const string Png = "image/png";
		public const string Jpg = "image/jpg";
		
		
	}
}
