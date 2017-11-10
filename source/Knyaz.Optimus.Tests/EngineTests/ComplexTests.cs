using Knyaz.Optimus.ResourceProviders;
using NUnit.Framework;
using R = Knyaz.Optimus.Tests.Resources.Resources;

namespace Knyaz.Optimus.Tests.EngineTests
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
				engine.Scripting.BeforeScriptExecute += script => System.Console.WriteLine(
					"Executing:" + (script.Src ?? script.Id ?? "<script>"));

				engine.Scripting.AfterScriptExecute += script => System.Console.WriteLine(
					"Executed:" + (script.Src ?? script.Id ?? "<script>"));

				engine.Scripting.ScriptExecutionError += (script, exception) => System.Console.WriteLine(
					"Error script execution:" + (script.Src ?? script.Id ?? "<script>") + " " + exception.Message);
			};
			engine.Console.OnLog += o => System.Console.WriteLine(o ?? "<null>");
			return engine;
		}

		[SetUp]
		public void SetUp()
		{
			_resourceProvider = Mocks.ResourceProvider("jquery.js", R.jquery_2_1_3)
				.Resource("knockout.js", R.knockout)
				.Resource("require.js", R.requirejs)
				.Resource("./template.js", R.template)
				.Resource("./text.js", R.text)
				.Resource("./stringTemplateEngine.js", R.stringTemplateEngine);
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