using System.Threading.Tasks;
using NUnit.Framework;
using Knyaz.Optimus.Tests.Resources;
using Knyaz.Optimus.Tests.TestingTools;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public class JQueryFormTests
	{
		[Test]
		public async Task Smoke()
		{
			var engine = TestingEngine.Build("<html><head><script> " + R.JQueryJs + " </script>" +
			                                 "<script>" + R.JQueryFormJs + "</script></head><body></body></html>");
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception);
			await engine.OpenUrl("http://localhost");
		}
	}
}
