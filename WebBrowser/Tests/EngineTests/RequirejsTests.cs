#if NUNIT
using System.Collections.Generic;
using System.Net;
using Moq;
using NUnit.Framework;
using WebBrowser.Properties;
using WebBrowser.ResourceProviders;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture]
	public class RequireJsTests
	{
		[Test]
		public void Smoke()
		{
			var engine = new Engine();
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception);
			engine.Load("<html><head><script>\r\n" + Resources.requirejs + "\r\n</script></head><body></body></html>");
		}

		[Test]
		public void Require()
		{
			var httpResourceProvider = Mock.Of<IHttpResourceProvider>(
				x => x.SendRequest(It.IsAny<HttpRequest>()) == new HttpResponse(HttpStatusCode.OK, "OK", null));
			var resourceProvider = Mock.Of<IResourceProvider>(x => x.HttpResourceProvider == httpResourceProvider);

			var engine = new Engine(resourceProvider);
			var log = new List<string>();
			engine.Console.OnLog += o =>
			{
				System.Console.WriteLine(o ?? "<null>");
				log.Add(o.ToString());
			};

			var script = @"require(['data'], function(x){console.log('OK');});";

			engine.Load("<html><head><script> " + Resources.requirejs + " </script><script>" + script + "</script></head><body><div id='uca'></div></body></html>");
			System.Threading.Thread.Sleep(15000);
			Mock.Get(httpResourceProvider).Verify(x => x.SendRequest(It.IsAny<HttpRequest>()), Times.Once);
			Assert.AreEqual(1, log.Count);
			Assert.AreEqual("OK", log[0]);
		}
	}
}
#endif