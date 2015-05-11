using System;
using System.IO;
using System.Net;

namespace WebBrowser.ResourceProviders
{
	internal class ResourceProvider : IResourceProvider
	{
		public IResource GetResource(Uri uri)
		{
			var scheme = uri.GetLeftPart(UriPartial.Scheme).ToLowerInvariant();
			if (scheme == "http://")
			{
				return GetHttpResource(uri);
			}

			throw new Exception("Unsupported scheme: " + scheme);
		}

		private IResource GetHttpResource(Uri uri)
		{
			var request = WebRequest.CreateHttp(uri);
			var response = request.GetResponse();
			
			//todo: copy resource
			return new Response(ResourceTypes.Html, response.GetResponseStream());
		}
	}

	public class Response : IResource
	{
		public Response(ResourceTypes type, Stream stream)
		{
			Stream = stream;
			Type = type;
		}

		public ResourceTypes Type { get; private set; }
		public Stream Stream { get; private set; }
	}
}
