using System;
using WebBrowser.ResourceProviders;

namespace WebBrowser
{
	public interface IResourceProvider
	{
		IResource GetResource(string uri);
		IHttpResourceProvider HttpResourceProvider { get; }
		string Root { get; set; }
	}
}
