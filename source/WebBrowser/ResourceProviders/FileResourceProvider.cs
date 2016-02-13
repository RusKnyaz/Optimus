using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WebBrowser.ResourceProviders
{
	class FileResourceProvider : ISpecResourceProvider
	{
		public IRequest CreateRequest(string url)
		{
			return new FileRequest(url);
		}

		public Task<IResource> SendRequestAsync(IRequest request)
		{
			var uri = new Uri(request.Url);

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

	public interface ISpecResourceProvider
	{
		IRequest CreateRequest(string url);
		Task<IResource> SendRequestAsync(IRequest request);
	}

	class FileResponse : IResource
	{
		public FileResponse(string type, Stream stream)
		{
			Type = type;
			Stream = stream;
		}

		public string Type { get; private set; }
		public Stream Stream { get; private set; }
	}

	class FileRequest : IRequest
	{
		public FileRequest(string url)
		{
			Url = url;
		}

		public string Url { get; private set; }
	}
}
