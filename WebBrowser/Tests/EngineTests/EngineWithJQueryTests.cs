#if NUNIT
using System;
using NUnit.Framework;
using WebBrowser.Properties;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture]
	public class EngineWithJQueryTests
	{
		[Test]
		public void JQuery()
		{
			var engine = new Engine();
			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script></head><body></body></html>");
		}

		[Test]
		public void JQueryIdSelector()
		{
			var script = "$('#uca').html('zaza');";
			var engine = new Engine();
			engine.Console.OnLog +=o => System.Console.WriteLine(o.ToString());
			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script><script>" + script + "</script></head><body><div id='uca'></div></body></html>");
			var ucaDiv = engine.Document.GetElementById("uca");
			Assert.AreEqual("zaza", ucaDiv.InnerHtml);
		}
	}
}
#endif