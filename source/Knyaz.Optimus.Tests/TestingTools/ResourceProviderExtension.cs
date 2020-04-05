using Knyaz.Optimus.ResourceProviders;
using System.Threading.Tasks;
using Knyaz.Optimus.Tests.Tools;

namespace Knyaz.Optimus.Tests.TestingTools
{
	static class ResourceProviderExtension
	{
		public static Task<Page> OpenPage(this IResourceProvider resourceProvider, string url) =>
			TestingEngine.BuildJint(resourceProvider).OpenUrl(url);
	}
}
