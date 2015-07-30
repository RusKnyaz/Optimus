#if NUNIT
using System.Threading;
using Moq;
using NUnit.Framework;
using WebBrowser.Properties;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture]
	public class ComplexTests
	{
		private IResourceProvider _resourceProvider;

		private Engine CreateEngine()
		{
			var engine = new Engine(_resourceProvider);
			engine.DocumentChanged+= () =>
			{
				engine.Document.AfterScriptExecute += script => System.Console.WriteLine(
					"Executed:" + (script.Src ?? script.Id ?? "<script>"));
			};
			engine.Console.OnLog += o => System.Console.WriteLine(o ?? "<null>");
			return engine;
		}

		[SetUp]
		public void SetUp()
		{
			_resourceProvider = Mocks.ResourceProvider("jquery.js", Resources.jquery_2_1_3)
				.Resource("knockout.js", Resources.knockout)
				.Resource("require.js", Resources.requirejs)
				.Resource("template.js", Resources.template)
				.Resource("./text.js", Resources.text)
				.Resource("./stringTemplateEngine.js", Resources.stringTemplateEngine);
		}

		[Test]
		public void StringTemplateEngine()
		{
			_resourceProvider.Resource("template.htm",
				"<script id='t1'><div id='templateDiv'></div></script><script id='t2'></script>");

			_resourceProvider.Resource("http://localhost/index.html",
@"<html>
<head>
<script src='require.js'/>
<script src='template.js'/>
<script>require(['template!/template.htm'], function(){ console('loaded');});</script>
</head>
<body></body></html>");

			var engine = CreateEngine();
			engine.OpenUrl("http://localhost/index.html");

			Thread.Sleep(5000);

			Mock.Get(_resourceProvider).Verify(x => x.GetResourceAsync("http://localhost/index.html"), Times.Once());
			Mock.Get(_resourceProvider).Verify(x => x.GetResourceAsync("require.js"), Times.Once());
			Mock.Get(_resourceProvider).Verify(x => x.GetResourceAsync("template.js"), Times.Once());
			Mock.Get(_resourceProvider).Verify(x => x.GetResourceAsync("./text.js"), Times.Once());
			Mock.Get(_resourceProvider).Verify(x => x.GetResourceAsync("./stringTemplateEngine.js"), Times.Once());
			Mock.Get(_resourceProvider).Verify(x => x.GetResourceAsync("template.htm"), Times.Once());

			
			var template1 = engine.Document.GetElementById("t1");
			var template2 = engine.Document.GetElementById("t2");
			Assert.IsNotNull(template1, "template1 != null");
			Assert.IsNotNull(template2, "template2 != null");
		}

		[Test]
		public void KnockoutWithRequiredTemplate()
		{
			_resourceProvider.Resource("http://localhost/client.js", "require(['clientTemplate'], function(){ });");
			_resourceProvider.Resource("http://localhost/clientTemplate.html", "");

			_resourceProvider.Resource("http://localhost/index.html", "<html><head><script src='/jquery.js'/><script src='/knockout.js'/><script src='require.js'/></head>" +
			                                                          "<body><div data-bind='template:\"clientTemplate\"'></div></body></html>");

			var engine = new Engine(_resourceProvider);
			engine.Load("http://localhost/index.html");
		}
	}
}
#endif