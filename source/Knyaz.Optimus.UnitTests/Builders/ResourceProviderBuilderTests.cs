using System;
using System.Threading.Tasks;
using Knyaz.Optimus.ResourceProviders;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Builders
{
	[TestFixture]
	public class ResourceProviderBuilderTests
	{
		[Test]
		public static async Task NotifyTest()
		{
			string method= null;
			
			var http = Mock.Of<IResourceProvider>();
			Mock.Get(http)
				.Setup(x => x.SendRequestAsync(It.IsAny<Request>()))
				.Returns<Request>(x =>
				{
					method = x.Method;
					return Task.Run(() => (IResource)new Response("text/html", null));
				});
			
			var resources = new ResourceProviderBuilder().Notify(rq => { rq.Method = "POST"; }, null).Http(http).Build();
			
			var request = new Request("GET", new Uri("http://knyaz.optimus", UriKind.Absolute));

			var response = await resources.SendRequestAsync(request);
			
			Assert.AreEqual("POST", method);
			Assert.AreEqual("text/html", response.Type);
		}
	}
}