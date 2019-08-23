using Knyaz.Optimus.ResourceProviders;
using System.Threading.Tasks;

namespace Knyaz.Optimus.Tests.TestingTools
{
	static class ResourceProviderExtension
	{
		public static Task<Page> OpenPage(this IResourceProvider resourceProvider, string url) =>
			new Engine(resourceProvider).OpenUrl(url);
	}
}
