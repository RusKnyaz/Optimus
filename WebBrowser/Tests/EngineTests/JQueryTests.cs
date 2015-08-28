#if NUNIT
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

		[TestCase(true, Result = "zaza")]
		[TestCase(false, Result = "")]
		public string JQueryIdSelectorInDeferScript(bool defer)
		{
			var resourceProvider = Mock.Of<IResourceProvider>().Resource("test.js", "$('#uca').html('zaza');");
			var engine = new Engine(resourceProvider);
			engine.Console.OnLog +=o => System.Console.WriteLine(o.ToString());
			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script><script src='test.js' "+ (defer?"defer":"") + "/></head><body><div id='uca'></div></body></html>");
			var ucaDiv = engine.Document.GetElementById("uca");
			return ucaDiv.InnerHTML;
		}

		[Test]
		public void JQueryIdSelectorIn()
		{
			var resourceProvider = Mock.Of<IResourceProvider>().Resource("test.js", "$('#uca').html('zaza');");
			var engine = new Engine(resourceProvider);
			engine.Console.OnLog += o => System.Console.WriteLine(o.ToString());
			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script><script src='test.js'/></head><body><div id='uca'></div></body></html>");
			var ucaDiv = engine.Document.GetElementById("uca");
			Assert.AreEqual("", ucaDiv.InnerHTML);
		}

		[Test]
		public void Post()
		{
			var resourceProvider = Mock.Of<IResourceProvider>();
			resourceProvider.Resource("test.js", "$.post('http://localhost/data').done(function(x){console.log(x);});");
			resourceProvider.Resource("http://localhost/data", "OK");

			var engine = new Engine(resourceProvider);
			var log = new List<string>();
			engine.Console.OnLog += o =>
			{
				System.Console.WriteLine(o ?? "<null>");
				log.Add(o.ToString());
			};

			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script><script src='test.js' defer/></head><body><div id='uca'></div></body></html>");
			System.Threading.Thread.Sleep(1000);
			CollectionAssert.AreEqual(new[]{"OK"}, log);
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
			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script><script defer>" + script + "</script></head><body><div id='uca'></div></body></html>");
			Assert.AreEqual("ok", result);
		}

		[Test]
		public void JQueryFind()
		{
			var script = "var tmpDiv = $('<div><script type=\"text/html\">A</script></div>');" +
						 "var res = tmpDiv.find(\"script[type='text/html']\");" +
			             "console.log(res.length);";
			var engine = new Engine();
			string result = null;
			engine.Console.OnLog += o =>
			{
				System.Console.WriteLine(o.ToString());
				result = o.ToString();
			};
			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script><script defer>" + script + "</script></head><body><div id='uca'></div></body></html>");
			Assert.AreEqual("1", result);
		}
	}

	public static class StringExtension
	{
		public static Stream ToStream(this string str)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(str));
		}
	}
}
#endif