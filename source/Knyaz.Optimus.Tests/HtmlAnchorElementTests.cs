using System.Threading;
using System.Threading.Tasks;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using Knyaz.Optimus.Tests.TestingTools;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlAnchorElementTests
	{
		/// <summary>
		/// The test checks that the java script code in the 'href' attribute called when 'click' method of anchor is called.
		/// Handler should be called after the script execution (asynchronously).
		/// </summary>
		[TestCase("a", Description = "Click on anchor leads to the href js execution.")]
		[TestCase("d", Description = "Click on anchor's child leads to the href js execution.")]
		public async Task HrefJavaScriptOnClick(string clickNodeId)
		{
			var signal = new AutoResetEvent(false);
			var console = new Mock<IConsole>();
			console.Setup(x => x.Log(1d)).Callback(() => signal.Set());
			
			var resourceProvider = Mocks.ResourceProvider("http://loc/",
				"<html><body><a id=a href='JavaScript:console.log(1)'><div id=d></div></a></body></html>");
			
			var engine = EngineBuilder.New()
				.SetResourceProvider(resourceProvider)
				.UseJint()
				.Window(w => w.SetConsole(console.Object))
				.Build();
			
			var page = await engine.OpenUrl("http://loc/");

			((HtmlElement)page.Document.GetElementById(clickNodeId)).Click();
			
			Assert.IsTrue(signal.WaitOne(1000), "Logged");
		}

		[Test]
		public async Task HrefJavaScriptOnClickPrevent()
		{
			var signal = new AutoResetEvent(false);
			var console = new Mock<IConsole>();
			console.Setup(x => x.Log(1d)).Callback(() => signal.Set());
			
			var resourceProvider = Mocks.ResourceProvider("http://loc/",
				"<html><body><a id=a href='JavaScript:console.log(1)'><div id=d></div></a></body></html>");
			
			var engine = EngineBuilder.New()
				.SetResourceProvider(resourceProvider)
				.UseJint()
				.Window(w => w.SetConsole(console.Object))
				.Build();
			
			var page = await engine.OpenUrl("http://loc/");

			var a = (HtmlAnchorElement)page.Document.GetElementById("a");
			a.OnClick += evt =>
			{
				evt.PreventDefault();
				return true;
			};

			a.Click();
			
			Assert.IsFalse(signal.WaitOne(1000), "Should not be called");
		}

		[Test]
		public async Task HrefJavaScriptOnClickOrder()
		{
			var signal = new AutoResetEvent(false);
			var console = new TestingConsole();

			console.OnLog += x => {
				if(console.LogHistory.Count == 2)
					signal.Set();
			};
			
			var resourceProvider = Mocks.ResourceProvider("http://loc/",
				"<html><body><a id=a href='JavaScript:console.log(1)'><div id=d></div></a>" +
				"<script>var a = document.getElementById('a');" +
				"a.click();" +
				"console.log(2);</script></body></html>");
			
			var engine = EngineBuilder.New()
				.SetResourceProvider(resourceProvider)
				.UseJint()
				.Window(w => w.SetConsole(console))
				.Build();
			
			await engine.OpenUrl("http://loc/");

			signal.WaitOne(5000);
			
			Assert.AreEqual(new object[]{2d,1d}, console.LogHistory);
		}
	}
}