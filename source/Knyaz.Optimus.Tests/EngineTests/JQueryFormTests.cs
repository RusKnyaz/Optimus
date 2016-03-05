using Knyaz.Optimus.Tests.Properties;
using NUnit.Framework;

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
			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script>" +
				"<script>" + Resources.jQuery_Form + "</script></head><body></body></html>");
		}
	}
}
