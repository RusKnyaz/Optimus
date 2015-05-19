#if NUNIT
using System.Collections.Generic;
using System.IO;
using System.Text;
using Moq;
using NUnit.Framework;
using WebBrowser.Properties;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture]
	public class EngineWithJQueryTests
	{
		[Test]
		public void Smoke()
		{
			var engine = new Engine();
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception);
			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script></head><body></body></html>");
		}

		[Test]
		public void JQueryIdSelector()
		{
			var script = "$('#uca').html('zaza');";
			var engine = new Engine();
			engine.Console.OnLog +=o => System.Console.WriteLine(o.ToString());
			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script><script>" + script + "</script></head><body><div id='uca'></div></body></html>");
			var ucaDiv = engine.Document.GetElementById("uca");
			Assert.AreEqual("zaza", ucaDiv.InnerHtml);
		}

		[Test]
		public void Ajax()
		{
			var resourceProviderMock = new Mock<IResourceProvider>();
			var resource = Mock.Of<IResource>(x => x.Stream == new MemoryStream(Encoding.UTF8.GetBytes("OK")));

			resourceProviderMock.Setup(x => x.GetResource(It.IsAny<string>())).Returns(resource);

			var engine = new Engine(resourceProviderMock.Object);
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o.ToString());

			var script = "$.post('http://localhost/data').done(function(x){console.log(x)});";

			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script><script>" + script + "</script></head><body><div id='uca'></div></body></html>");
			System.Threading.Thread.Sleep(1000);
			Assert.AreEqual(1, log.Count);
			Assert.AreEqual("OK", log[0]);
		}
	}
}
#endif