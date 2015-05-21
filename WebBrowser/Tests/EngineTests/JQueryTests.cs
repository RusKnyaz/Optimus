#if NUNIT
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Moq;
using NUnit.Framework;
using WebBrowser.Properties;
using WebBrowser.ResourceProviders;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture]
	public class JQueryTests
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
			var httpResourceProvider = Mock.Of<IHttpResourceProvider>(x => x.SendRequest(It.IsAny<HttpRequest>()) ==
				new HttpResponse(HttpStatusCode.OK, "OK"));

			var resourceProvider = Mock.Of<IResourceProvider>(x => x.HttpResourceProvider == httpResourceProvider);

			var engine = new Engine(resourceProvider);
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o.ToString());

			var script = "$.post('http://localhost/data').done(function(x){console.log(x)});";

			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script><script>" + script + "</script></head><body><div id='uca'></div></body></html>");
			System.Threading.Thread.Sleep(1000);
			Assert.AreEqual(1, log.Count);
			Assert.AreEqual("OK", log[0]);
		}

		[Test]
		public void JQueryCreate()
		{
			var script = "var a = $('<input type=\"file\">');console.log(a?'ok':'error');";
			var engine = new Engine();
			string result = null;
			engine.Console.OnLog += o => 
			{ 
				System.Console.WriteLine(o.ToString());
				result = o.ToString();
			};
			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script><script>" + script + "</script></head><body><div id='uca'></div></body></html>");
			Assert.AreEqual("ok", result);
		}
	}
}
#endif