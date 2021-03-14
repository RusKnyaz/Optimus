using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.TestingTools;
using Moq;
using NUnit.Framework;
using Knyaz.Optimus.Tests.Resources;
using Knyaz.Optimus.Tests.TestingTools;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public class JQueryTests
	{
		[Test]
		public async Task Smoke()
		{
			var engine = TestingEngine.BuildJint("<html><head><script> " + R.JQueryJs + " </script></head><body></body></html>");
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception);
			await engine.OpenUrl("http://localhost");
		}

		[TestCase(true, ExpectedResult = "zaza")]
		[TestCase(false, ExpectedResult = "")]
		public string JQueryIdSelectorInDeferScript(bool defer)
		{
			var resourceProvider = Mock.Of<IResourceProvider>()
				.Resource("http://localhost",
					"<html><head><script> " + R.JQueryJs + " </script><script src='test.js' " + (defer ? "defer" : "") +
					"/></head><body><div id='uca'></div></body></html>")
				.Resource("http://localhost/test.js", "$('#uca').html('zaza');");
			var engine = TestingEngine.BuildJint(resourceProvider, SystemConsole.Instance);
			engine.OpenUrl("http://localhost").Wait();
			var ucaDiv = engine.Document.GetElementById("uca");
			return ucaDiv.InnerHTML;
		}

		[Test]
		public void JQueryIdSelectorIn()
		{
			var resourceProvider = Mock.Of<IResourceProvider>()
				.Resource("http://localhost",
					"<html><head><script> " + R.JQueryJs +
					" </script><script src='test.js'/></head><body><div id='uca'></div></body></html>")
				.Resource("http://localhost/test.js", "$('#uca').html('zaza');");
			var engine = TestingEngine.BuildJint(resourceProvider, SystemConsole.Instance);
			engine.OpenUrl("http://localhost").Wait();
			var ucaDiv = engine.Document.GetElementById("uca");
			Assert.AreEqual("", ucaDiv.InnerHTML);
		}

		[Test]
		public void Post()
		{
			var resourceProvider = Mock.Of<IResourceProvider>()
				.Resource("http://localhost",
					"<html><head><script> " + R.JQueryJs +
					" </script><script src='test.js' defer/></head><body><div id='uca'></div></body></html>")
				.Resource("http://localhost/test.js",
					"$.post('http://localhost/data').done(function(x){console.log(x);});")
				.Resource("http://localhost/data", "OK");

			var console = new Mock<IConsole>();
			var engine = TestingEngine.BuildJint(resourceProvider, console.Object);
			
			var log = new List<string>();
			var signal = new ManualResetEvent(false);
			
			console.Setup(x => x.Log(It.IsAny<string>(), It.IsAny<object[]>())).Callback<string, object[]>((msg,_) =>
			{
				System.Console.WriteLine(msg);
				log.Add(msg);
				signal.Set();
			});
			
			engine.OpenUrl("http://localhost");
			Assert.IsTrue(signal.WaitOne(10000));
			CollectionAssert.AreEqual(new[] {"OK"}, log);
		}


		[Test]
		public async Task JQueryCreate()
		{
			var script = "var a = $('<input type=\"file\">');console.log(a?'ok':'error');";
			var resources = Mocks.ResourceProvider("http://localhost",
				"<html><head><script> " + R.JQueryJs + " </script><script defer>" + script +
				"</script></head><body><div id='uca'></div></body></html>");
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resources, console);
			
			await engine.OpenUrl("http://localhost");
			Assert.AreEqual(new[]{"ok"}, console.LogHistory);
		}

		[Test]
		public async Task On()
		{
			var script = @"$('#b').on('click', function() {console.log('hi'); });
var e = document.createElement('div');
e.id = 'loaded';
document.body.appendChild(e);";

			var resources = Mocks.ResourceProvider(
				"http://localhost",
				"<html><head><script> " + R.JQueryJs +
				" </script></head><body><div id='b'></div></body><script>" + script +
				"</script></html>");
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resources, console);
			var page = await engine.OpenUrl("http://localhost");
			var loaded = page.Document.WaitId("loaded");
			Assert.IsNotNull(loaded);

			var e = engine.Document.CreateEvent("Event");
			e.InitEvent("click", true, true);
			engine.Document.GetElementById("b").DispatchEvent(e);

			Assert.AreEqual(new[]{"hi"}, console.LogHistory);
		}

		[Test]
		public async Task Bind()
		{
			var script = @"$('#b').bind('click', function() {console.log('hi'); });
var e = document.createElement('div');
e.id = 'loaded';
document.body.appendChild(e);";
			var resources = Mocks.ResourceProvider("http://localhost",
				"<html><head><script> " + R.JQueryJs +
				" </script></head><body><div id='b'></div></body><script>" +
				script + "</script></html>");

			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resources, console);
			var page = await engine.OpenUrl("http://localhost");
			var loaded = page.Document.WaitId("loaded");
			Assert.IsNotNull(loaded);

			((HtmlElement) engine.Document.GetElementById("b")).Click();

			Assert.AreEqual(new[]{"hi"}, console.LogHistory);
		}

		[Test]
		public void DocumentBody()
		{
			var script = @"$(function(){console.log(document.body);});";
			var console = new TestingConsole();
			var resources = Mocks.ResourceProvider(
				"http://localhost",
				"<html><head><script> " + R.JQueryJs + " </script><script>" + script +
				"</script></head><body><div id='b'></div></body></html>"); 
			var engine = TestingEngine.BuildJint(resources, console);
			var page = engine.OpenUrl("http://localhost");
			Thread.Sleep(1000);

			Assert.AreEqual(1, console.LogHistory.Count);
			Assert.IsNotNull(console.LogHistory[0]);
		}

		[TestCase(".u", 1)]
		[TestCase("div", 1)]
		[TestCase("#a", 1)]
		public async Task Selector(string selector, int exptectedCount)
		{
			var script = @"console.log($('" + selector + "').length);";
			var resources = Mocks.ResourceProvider(
				"http://localhost",
				"<html><head><script> " + R.JQueryJs +
				" </script></head><body><div class = 'u' id='a'></div><script>" + script +
				"</script></body></html>");
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resources, console);
			await engine.OpenUrl("http://localhost");
			Assert.AreEqual(new[]{exptectedCount}, console.LogHistory);
		}

		[Test]
		public async Task OnWithSelector()
		{
			var script = @"$('#a').on('click.some', '.u', function(){console.log('hi');});";
			var resources = Mocks.ResourceProvider(
				"http://localhost",
				"<html><head><script> " + R.JQueryJs +
				" </script></head><body><div id='a'><span class='u' id='u'></span></div><script>" + script +
				"</script></body></html>");
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resources, console);
			var page = await engine.OpenUrl("http://localhost");
			var u = page.Document.GetElementById("u") as HtmlElement;
			u.Click();
			Assert.AreEqual(new[]{"hi"}, console.LogHistory);
		}

		[Test]
		public async Task SetHtmlWithScript()
		{
			var script = "$('#target')['html']('<script>console.log(1);</script>');";
			var resources = Mocks.ResourceProvider(
				"http://localhost",
				"<html><head><script> " + R.JQueryJs +
				" </script></head><body><div id='target'></div><script>" + script + "</script></body></html>");
			var console = new Mock<IConsole>();
			var engine = TestingEngine.BuildJint(resources, console.Object); 
			await engine.OpenUrl("http://localhost");
			console.Verify(x => x.Log(1d), Times.Once);
		}
	}
}