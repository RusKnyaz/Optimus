using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
	/// <summary>
	/// Allows to request files content from file system.
	/// </summary>
	class FileResourceProvider : ISpecResourceProvider
	{
		/// <summary>
		/// Creates resource request.
		/// </summary>
		public IRequest CreateRequest(Uri url) => new FileRequest(url);

		/// <summary>
		/// Requests file resource..
		/// </summary>
		public Task<IResource> SendRequestAsync(IRequest request)
		{
			var uri = request.Url;

			var fielInfo = new FileInfo(uri.AbsolutePath);

			using (var stream = fielInfo.OpenRead())
			using(var reader = new StreamReader(stream))
			{
				/*return reader.ReadToEndAsync().ContinueWith(task => 
					new FileResponse(ResourceTypes.Html /*todo: extract from extension#1#, new MemoryStream(Encoding.UTF8.GetBytes(task.Result))));*/

				var data = reader.ReadToEnd();
				IResource response = new FileResponse(ResourceTypes.Html /*todo: extract from extension*/, new MemoryStream(Encoding.UTF8.GetBytes(data)));
				return Task.Run(() => response);
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

	class FileRequest : IRequest
	{
		public FileRequest(Uri url) => Url = url;

		public Uri Url { get; }
	}
}
