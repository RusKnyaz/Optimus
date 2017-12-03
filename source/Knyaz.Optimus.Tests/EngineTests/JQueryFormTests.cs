using NUnit.Framework;
using Knyaz.Optimus.Tests.Resources;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public class JQueryFormTests
	{
		[Test]
		public void Smoke()
		{
			var engine = new Engine();
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception);
			engine.Load("<html><head><script> " + R.JQueryJs + " </script>" +
				"<script>" + R.JQueryFormJs + "</script></head><body></body></html>");
		}
	}
}
