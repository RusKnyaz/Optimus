using System.Collections.Generic;
using System.Threading.Tasks;
using Knyaz.Optimus.ResourceProviders;
using NUnit.Framework;
using Knyaz.Optimus.Tests.Resources;
using Knyaz.Optimus.Tests.TestingTools;
using Moq;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture("requirejs")]
	[TestFixture("requirejs.2.3.5")]
	public class RequireJsTests
	{
		private string requireJs;
		
		public RequireJsTests(string file)
		{
			requireJs = R.GetString("Knyaz.Optimus.Tests.Resources." + file + ".js");
		}
		
		[Test]
		public async Task Smoke()
		{
			var engine = TestingEngine.BuildJint("<html><head><script>\r\n" + requireJs + "\r\n</script></head><body></body></html>");
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception);

			await engine.OpenUrl("http://localhost");
		}

		[Test]
		public void RequireEmbeddedLib()
		{
			var script = @"require(['data'], function(x){console.log('main');console.log(x);});";
			
			var resourceProvider = Mock.Of<IResourceProvider>()
				.Resource("http://root.ru", "<html><head><script> " + requireJs + " </script><script>" + script + "</script></head><body><div id='uca'></div></body></html>")
				.Resource("http://root.ru/data.js", "define(function(){console.log('dependency'); return 'val';});");

			var log = OpenUrlAndGetLog("http://root.ru", resourceProvider);
			
			CollectionAssert.AreEqual(new[]{"dependency", "main", "val"}, log);
		}

		[Test]
		public void Require()
		{
			var script = @"require(['data'], function(x){console.log('main');console.log(x);});";
			
			var resourceProvider = Mock.Of<IResourceProvider>()
				.Resource("http://root.ru", "<html><head><script src='require.js'/><script>" + script + "</script></head><body><div id='uca'></div></body></html>")
				.Resource("http://root.ru/data.js", "define(function(){console.log('dependency'); return 'val';});")
				.Resource("http://root.ru/require.js", requireJs);

			var log = OpenUrlAndGetLog("http://root.ru", resourceProvider);

			CollectionAssert.AreEqual(new[] { "dependency", "main", "val" }, log);
		}
		
		private static List<object> OpenUrlAndGetLog(string url, IResourceProvider resourceProvider)
		{
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			engine.OpenUrl(url).Wait();
			System.Threading.Thread.Sleep(5000);
			return console.LogHistory;
		}
	}
}