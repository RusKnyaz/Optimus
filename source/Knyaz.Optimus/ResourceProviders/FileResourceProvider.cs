using System.IO;
using System.Text;
using System.Threading.Tasks;
using Knyaz.Optimus.Environment;

namespace Knyaz.Optimus.ResourceProviders
{
	/// <summary>
	/// Allows to request files content from file system.
	/// </summary>
	class FileResourceProvider : IResourceProvider
	{
		/// <summary>
		/// Requests file resource..
		/// </summary>
		public Task<IResource> SendRequestAsync(Request request)
		{
			var uri = request.Url;

			var fileInfo = new FileInfo(uri.AbsolutePath);

			var inferredMimeTYpe = GetMimeTypeByExt(fileInfo.Extension);
			
			var response = new FileResponse(inferredMimeTYpe, fileInfo.OpenRead());
			
			//todo: try to get mime type from the request header first.
			
			return Task.Run(() => (IResource)response);
		}

		private string GetMimeTypeByExt(string ext)
		{
			switch (ext.ToUpperInvariant())
			{
				case ".BMP":
					return ResourceTypes.Bmp;
				case ".PNG":
					return ResourceTypes.Png;
				case ".JPG":
					return ResourceTypes.Jpg;
				case ".HTM":
				case ".HTML":
					return ResourceTypes.Html;
				case ".CSS":
					return ResourceTypes.Css;
				case ".JS":
					return ResourceTypes.JavaScript;
				default:
					return ResourceTypes.Html;
			}
		}
	}

	class FileResponse : IResource
	{
		internal FileResponse(string type, Stream stream)
		{
			Type = type;
			Stream = stream;
		}

		public string Type { get; private set; }
		public Stream Stream { get; private set; }
	}
}
