using System;
using System.IO;
using System.Threading.Tasks;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.ResourceProviders
{
	/// <summary>
	/// Gets the data from url like "data:..."
	/// </summary>
	class DataResourceProvider : IResourceProvider
	{
		public Task<IResource> SendRequestAsync(Request request)
		{
			var url = request.Url;
			return Task.Run(() => GetDataFromUrl(url));
		}

		IResource GetDataFromUrl(Uri uri)
		{
			var data = uri.GetUriData();
			
			return new Response(data.Type, new MemoryStream(data.Data));
		}
	}
}