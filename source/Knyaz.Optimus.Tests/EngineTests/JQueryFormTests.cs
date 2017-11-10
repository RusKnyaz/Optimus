using NUnit.Framework;
using R = Knyaz.Optimus.Tests.Resources.Resources;

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
			engine.Load("<html><head><script> " + R.jquery_2_1_3 + " </script>" +
				"<script>" + R.jQuery_Form + "</script></head><body></body></html>");
		}
	}
}
