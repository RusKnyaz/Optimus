using NUnit.Framework;
using Knyaz.Optimus.Tests.Resources;
using Knyaz.Optimus.Tests.TestingTools;
using Knyaz.Optimus.Tests.Tools;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public class JQueryFormTests
	{
		[Test]
		public void Smoke()
		{
			var engine = TestingEngine.BuildJint();
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception);
			engine.Load("<html><head><script> " + R.JQueryJs + " </script>" +
				"<script>" + R.JQueryFormJs + "</script></head><body></body></html>");
		}
	}
}
