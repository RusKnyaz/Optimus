using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Knyaz.Optimus.Tests.Resources;

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
		public void Smoke()
		{
			var engine = new Engine();
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception);
			engine.Load("<html><head><script>\r\n" + requireJs + "\r\n</script></head><body></body></html>");
		}

		[Test]
		public void RequireEmbededLib()
		{
			var resourceProvider = Mocks.ResourceProvider("./data.js", "define(function(){console.log('dependency'); return 'val';});");

			var engine = new Engine(resourceProvider);
			var log = new List<string>();
			engine.Console.OnLog += o =>
			{
				System.Console.WriteLine(o ?? "<null>");
				log.Add(o.ToString());
			};

			var script = @"require(['data'], function(x){console.log('main');console.log(x);});";

			engine.Load("<html><head><script> " + requireJs + " </script><script>" + script + "</script></head><body><div id='uca'></div></body></html>");
			System.Threading.Thread.Sleep(5000);
			//todo: Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("./data.js"), Times.Once);
			CollectionAssert.AreEqual(new[]{"dependency", "main", "val"}, log);
		}

		[Test]
		public void Require()
		{
			var resourceProvider = Mocks
				.ResourceProvider("./data.js", "define(function(){console.log('dependency'); return 'val';});")
				.Resource("require.js", requireJs);

			var engine = new Engine(resourceProvider);
			var log = new List<string>();
			engine.Console.OnLog += o =>
			{
				System.Console.WriteLine(o ?? "<null>");
				log.Add(o.ToString());
			};

			var script = @"require(['data'], function(x){console.log('main');console.log(x);});";

			engine.Load("<html><head><script src='require.js'/><script>" + script + "</script></head><body><div id='uca'></div></body></html>");
			System.Threading.Thread.Sleep(5000);
//todo:			Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("./data.js"), Times.Once);
			CollectionAssert.AreEqual(new[] { "dependency", "main", "val" }, log);
		}
	}
}