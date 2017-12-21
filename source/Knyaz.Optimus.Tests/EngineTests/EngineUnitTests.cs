using System.Collections.Generic;
using System.Threading;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.TestingTools;
using Moq;
using NUnit.Framework;
using Text = Knyaz.Optimus.Dom.Elements.Text;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public class EngineUnitTests
	{
		private IResourceProvider _resourceProvider;
		private Engine _engine;

		[SetUp]
		public void SetUp()
		{
			_resourceProvider = Mock.Of<IResourceProvider>();
			_engine = new Engine(_resourceProvider);
		}

		[Test]
		public void EmptyHtml()
		{
			_engine.Load("<html></html>");
		}
		
		[Test]
		public void GenerateContent()
		{
			_resourceProvider.Resource("test.js", "var elem = document.getElementById('content');elem.innerHTML = 'Hello';");
			_engine.Load("<html><head><script src='test.js' defer/></head><body><div id='content'></div></body></html>");
			var contentDiv = _engine.Document.GetElementById("content");
			Assert.AreEqual("Hello", contentDiv.InnerHTML);
			Assert.AreEqual(1, contentDiv.ChildNodes.Count);
			var text = contentDiv.ChildNodes[0] as Text;
			Assert.IsNotNull(text);
			Assert.AreEqual("Hello", text.Data);
		}

		[Test]
		public void DomManipulation()
		{
			var resourceProvider = Mock.Of<IResourceProvider>();
			resourceProvider.Resource("test.js", "var div = document.createElement('div');" +
			                                     "div.setAttribute('id', 'c3');" +
			                                     "var c2 = document.getElementById('content2');" +
			                                     "document.documentElement.getElementsByTagName('body')[0].insertBefore(div, c2);");
			var engine = new Engine(resourceProvider);
			engine.Load("<html><head><script src='test.js' defer/></head><body><div id='content1'></div><div id='content2'></div></body></html");
			Assert.AreEqual(3, engine.Document.DocumentElement.GetElementsByTagName("body")[0].ChildNodes.Count);
			var elem = engine.Document.GetElementById("c3");
			Assert.IsNotNull(elem);
		}

		[Test]
		public void Text()
		{
			var engine = new Engine();
			engine.Console.OnLog += System.Console.WriteLine;
			engine.Load(Mocks.Page(@"var c2 = document.getElementById('content1').innerHTML = 'Hello';", "<span id='content1'></span>"));
			var elem = engine.Document.GetElementById("content1");
			Assert.AreEqual("Hello", elem.InnerHTML);
		}

		[Test]
		public void GetAttribute()
		{
			string attr = null;
			_engine.Console.OnLog += o => attr = o.ToString();
			_engine.Load(Mocks.Page(@"console.log(document.getElementById('content1').getAttribute('id'));",
				"<span id='content1'></span>"));
			Assert.AreEqual("content1", attr);
		}

		[Test]
		public void LoadPage()
		{
			var resourceProvider = Mocks.ResourceProvider(
				"http://localhost", "<html><body><div id='c'></div></body></html>");

			var engine = new Engine(resourceProvider);
			engine.OpenUrl("http://localhost");
			engine.WaitId("c");
			Assert.AreEqual(1, engine.Document.Body.GetElementsByTagName("div").Length);
		}

		[Test]
		public void NonJsScript()
		{
			var engine = new Engine();

			string loggedValue = null;
			engine.Console.OnLog += o => loggedValue = o.ToString();

			engine.Load("<html><head><script id='template' type='text/html'><div>a</div></script></head></html>");
			var script = engine.Document.GetElementById("template");
			Assert.AreEqual("<div>a</div>", script.InnerHTML);
		}

		[Test]
		public void LoadScriptTest()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost/script.js", "console.log('hello');");
			var engine = new Engine(resourceProvider);

			string loggedValue = null;
			engine.Console.OnLog += o => loggedValue = o.ToString();

			engine.Load("<html><head><script src='http://localhost/script.js'></script></head></html>");
			Assert.AreEqual("hello", loggedValue);
		}

		[Test]
		public void DocumentRadyStateComplete()
		{
			var engine = new Engine();
			engine.Load("<html><body></body></html>");
			Assert.AreEqual(DocumentReadyStates.Complete, engine.Document.ReadyState);
		}

		[Test]
		public void LoadAndRunScriptAddedInRuntime()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost/script.js", "console.log('hello');");
			var engine = new Engine(resourceProvider);

			string loggedValue = null;
			engine.Console.OnLog += o => loggedValue = o.ToString();

			engine.Load("<html><head></head></html>");

			var script = (Script)engine.Document.CreateElement("script");
			script.Src = "http://localhost/script.js";
			engine.Document.Head.AppendChild(script);

			Thread.Sleep(1000);

			//todo: Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("http://localhost/script.js"), Times.Once());
			Assert.AreEqual("hello", loggedValue);
		}

		
		[Test]
		public void SetTimeoutWithData()
		{
			var engine = new Engine();
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o == null ? "<null>" : o.ToString());
			engine.Load(Mocks.Page(@"var timer = window.setTimeout(function(x){console.log(x);}, 300, 'ok');"));
			Assert.AreEqual(0, log.Count);
			Thread.Sleep(1000);
			CollectionAssert.AreEqual(new[] { "ok" }, log);
		}

		[Test]
		public void SetTimeout()
		{
			var engine = new Engine();
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o == null ? "<null>" : o.ToString());
			engine.Load(Mocks.Page(@"var timer = window.setTimeout(function(x){console.log('ok');}, 300);"));
			Assert.AreEqual(0, log.Count);
			Thread.Sleep(1000);
			CollectionAssert.AreEqual(new[]{"ok"}, log);
		}

		[Test, Ignore("For manual run")]
		public void ClearTimeout()
		{
			var engine = new Engine();
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o == null ? "<null>" : o.ToString());
			engine.Load(Mocks.Page(
@"var timer = window.setTimeout(function(){console.log('ok');}, 500);
window.clearTimeout(timer);"));
			Assert.AreEqual(0, log.Count);
			Thread.Sleep(1000);
			Assert.AreEqual(0, log.Count);
		}

		

		[Test]
		public void Location()
		{
			var resourceProvider = Mocks.ResourceProvider("http://todosoft.ru", "");
			var engine = new Engine(resourceProvider);
			engine.OpenUrl("http://todosoft.ru");
			Assert.AreEqual("http://todosoft.ru/", engine.Window.Location.Href);
			Assert.AreEqual("http:", engine.Window.Location.Protocol);
		}

		[Test]
		public void SetLocationHref()
		{
			var resourceProvider = Mocks.ResourceProvider("http://todosoft.ru", "");
			var engine = new Engine(resourceProvider);
			//todo: write similar test on js
			engine.Window.Location.Href = "http://todosoft.ru";
//todo:			Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("http://todosoft.ru"), Times.Once());
		}

		[Test]
		public void GetElementsByTagName()
		{
			var engine = new Engine();
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o == null ? "<null>" : o.ToString());
			engine.Load(Mocks.Page("console.log(document.getElementsByTagName('div').length);", "<div></div><div></div>"));
			Assert.AreEqual(1, log.Count);
			Assert.AreEqual("2", log[0]);
		}

		[Test]
		public void Ajax()
		{
			var resourceProvider = Mock.Of<IResourceProvider>();
			resourceProvider.Resource("http://localhost/unicorn.xml", "hello");

			var engine = new Engine(resourceProvider);
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o == null ? "<null>" : o.ToString());

			var client = new XmlHttpRequest(engine.ResourceProvider, () => this);

			client.OnReadyStateChange += () =>
				{
					if (client.ReadyState == XmlHttpRequest.DONE)
					{
						if (client.Status == 200)
						{
							engine.Console.Log(client.ResponseText);
						}
					}
				};
			client.Open("GET", "http://localhost/unicorn.xml", false);
			client.Send();
			
			Mock.Get(resourceProvider).Verify(x => x.SendRequestAsync(It.IsAny<HttpRequest>()), Times.Once());
			CollectionAssert.AreEqual(new[]{"hello"}, log);
		}

		[Test, Ignore("For manual run")] //is not working under Mono
		public void AddScriptAsync()
		{
			var engine = new Engine(Mocks.ResourceProvider("http://localhost/script.js", "console.log('in new script');"));
			var log = new List<string>();
			engine.Console.OnLog += o =>
			{
				log.Add(o == null ? "<null>" : o.ToString());
				System.Console.WriteLine(o == null ? "<null>" : o.ToString());
			};
			engine.Load("<html><head></head></html>");
			engine.Document.AddEventListener("DOMNodeInserted", @event =>
			{
				engine.Console.Log("nodeadded");
			}, false);

			var d = (Script)engine.Document.CreateElement("script");
			d.Id = "aaa";
			d.Async = true;
			d.Src = "http://localhost/script.js";
			d.OnLoad += () => engine.Console.Log("onload");
			engine.Document.Head.AppendChild(d);
			engine.Console.Log("afterappend");
			
			Thread.Sleep(1000);
			CollectionAssert.AreEqual(new[] { "nodeadded", "afterappend", "in new script", "onload" }, log);
		}

		[Test]
		public void RaiseExecuteScript()
		{
			var engine = new Engine();
			engine.Load("<html><head></head><body></body></html>");
			var doc = engine.Document;
			
			var beforeCount = 0;
			var afterCount = 0;
			doc.AddEventListener("BeforeScriptExecute", @event => beforeCount++, false);
			doc.AddEventListener("AfterScriptExecute", @event => afterCount++, false);

			var script = doc.CreateElement("script");
			script.InnerHTML = "console.log('hi');";
			doc.Head.AppendChild(script);

			Assert.AreEqual(1, beforeCount, "BeforeScriptExecute Event handlers calls count");
			Assert.AreEqual(1, afterCount, "AfterScriptExecute Event handlers calls count");
		}

		[Test]
		public void AppendScriptAsInnerHtml()
		{
			var engine = new Engine();
			var log = new List<string>();
			engine.Console.OnLog += o =>
			{
				log.Add(o == null ? "<null>" : o.ToString());
				System.Console.WriteLine(o == null ? "<null>" : o.ToString());
			};
			engine.Load("<html><head></head><body></body></html>");
			engine.Document.Body.InnerHTML = "<script>console.log('HI');</script>";
			Assert.AreEqual(new[]{"HI"}, log);
		}

		[TestCase("http://todosoft.ru", "http://todosoft.ru/")]
		[TestCase("http://todosoft.ru/", "http://todosoft.ru/")]
		[TestCase("http://todosoft.ru/subfolder", "http://todosoft.ru/subfolder/")]
		[TestCase("http://todosoft.ru/index.html", "http://todosoft.ru/index.html")]
		public void OpenUrl(string url, string expectedRoot)
		{
			var resourceProvider = Mocks.ResourceProvider(url.TrimEnd('/'), "<html><head></head></html>");

			var engine = new Engine(resourceProvider);
			engine.OpenUrl(url).Wait();
			
			Assert.AreEqual(expectedRoot, resourceProvider.Root);
		}

		[TestCase("http://a.ru/index.html", "k.js", "http://a.ru/k.js")]
		[TestCase("http://a.ru/test", "./js/k.js", "http://a.ru/test/js/k.js")]
		[TestCase("http://a.ru/test", "/js/k.js", "http://a.ru/js/k.js")]
		[TestCase("http://chromium.github.io/octane", "js/jquery.js", "http://chromium.github.io/octane/js/jquery.js")]
		public void OpenUrlWithResource(string url, string resUrl, string expectedResUrl)
		{
			var httpResourceProvider = Mock.Of<ISpecResourceProvider>().Resource(
				url.TrimEnd('/'), 
				"<html><head><script src='"+resUrl+"'></script></head></html>");

			httpResourceProvider.Resource(expectedResUrl, "console.Log('ok');");

			var resourceProvider = new ResourceProvider(httpResourceProvider, null);
			
			var engine = new Engine(resourceProvider);
			var log = new List<string>();
			engine.Console.OnLog += o =>
			{
				log.Add(o == null ? "<null>" : o.ToString());
				System.Console.WriteLine(o == null ? "<null>" : o.ToString());
			};

			engine.OpenUrl(url).Wait();
		}
	}
}