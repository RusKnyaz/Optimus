using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Environment;
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

		[Test]
		public void EmptyHtml()
		{
			var engine = new Engine(Mock.Of<IResourceProvider>());
			engine.Load("<html></html>");
		}
		
		private Engine Load(IResourceProvider resourceProvider)
		{
			var engine = new Engine(resourceProvider);
			engine.OpenUrl("http://localhost").Wait();
			return engine;
		}
		
		[Test]
		public void GenerateContent()
		{
			var engine = Load(Mock.Of<IResourceProvider>()
				.Resource("http://localhost", "<html><head><script src='test.js' defer/></head><body><div id='content'></div></body></html>")
				.Resource("http://localhost/test.js", "var elem = document.getElementById('content');elem.innerHTML = 'Hello';"));

			
			var contentDiv = engine.Document.GetElementById("content");
			Assert.AreEqual("Hello", contentDiv.InnerHTML);
			Assert.AreEqual(1, contentDiv.ChildNodes.Count);
			var text = contentDiv.ChildNodes[0] as Text;
			Assert.IsNotNull(text);
			Assert.AreEqual("Hello", text.Data);
		}

		[Test]
		public void DomManipulation()
		{
			var engine = Load(Mock.Of<IResourceProvider>()
				.Resource("http://localhost", "<html><head><script src='test.js' defer/></head><body><div id='content1'></div><div id='content2'></div></body></html")
				.Resource("http://localhost/test.js", "var div = document.createElement('div');" +
			                                     "div.setAttribute('id', 'c3');" +
			                                     "var c2 = document.getElementById('content2');" +
			                                     "document.documentElement.getElementsByTagName('body')[0].insertBefore(div, c2);"));
			
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
			var engine = new Engine(Mock.Of<IResourceProvider>());
			string attr = null;
			engine.Console.OnLog += o => attr = o.ToString();
			engine.Load(Mocks.Page(@"console.log(document.getElementById('content1').getAttribute('id'));",
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
			var log = engine.Console.ToList();
			engine.Load(Mocks.Page("console.log(document.getElementsByTagName('div').length);", "<div></div><div></div>"));
			Assert.AreEqual(1, log.Count);
			Assert.AreEqual(2d, log[0]);
		}

		[Test]
		public void Ajax()
		{
			var resourceProvider = Mock.Of<IResourceProvider>().Resource("http://localhost/unicorn.xml", "hello");

			var engine = new Engine(resourceProvider);
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o == null ? "<null>" : o.ToString());

			var client = new XmlHttpRequest(engine.ResourceProvider, () => this, engine.Document, (u,m) => engine.CreateRequest(u,m));

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
			
			Mock.Get(resourceProvider).Verify(x => x.SendRequestAsync(It.IsAny<Request>()), Times.Once());
			CollectionAssert.AreEqual(new[]{"hello"}, log);
		}

		[Test, Ignore("Unstable")]
		public void AddScriptAsync()
		{
			//todo: rewrite it with JS. current test is not stbable due to multithreading.
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

			var onloadSignal = new ManualResetEvent(false);
			
			var d = (Script)engine.Document.CreateElement("script");
			d.Id = "aaa";
			d.Async = true;
			d.Src = "http://localhost/script.js";
			d.OnLoad += e =>
			{
				engine.Console.Log("onload");
				onloadSignal.Set();
			};
			engine.Document.Head.AppendChild(d);

			onloadSignal.WaitOne(10000);
			CollectionAssert.AreEqual(new[] { "nodeadded", "in new script", "onload" }, log);
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
			var log = engine.Console.ToConsole().ToList();
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
			
			Assert.AreEqual(expectedRoot, engine.LinkProvider.Root);
		}

		[TestCase("http://a.ru/index.html", "k.js", "http://a.ru/k.js")]
		[TestCase("http://a.ru/test", "./js/k.js", "http://a.ru/test/js/k.js")]
		[TestCase("http://a.ru/test", "/js/k.js", "http://a.ru/js/k.js")]
		[TestCase("http://chromium.github.io/octane", "js/jquery.js", "http://chromium.github.io/octane/js/jquery.js")]
		public void OpenUrlWithResource(string url, string resUrl, string expectedResUrl)
		{
			var httpResourceProvider = Mocks.HttpResourceProvider()
				.Resource(url.TrimEnd('/'), "<html><head><script src='"+resUrl+"'></script></head></html>");

			httpResourceProvider.Resource(expectedResUrl, "console.Log('ok');");

			var resourceProvider = new ResourceProvider(httpResourceProvider, null);
			
			var engine = new Engine(resourceProvider);
			var log = engine.Console.ToConsole().ToList();

			engine.OpenUrl(url).Wait();
			
			Assert.AreEqual(new[]{"ok"}, log);
		}
		
		[Test]
		public void SetUpUserAgent()
		{
			var httpResourceProvider = Mocks.HttpResourceProvider()
				.Resource("http://localhost/", "<html><script src='sc.js'/></html>")
				.Resource("http://localhost/sc.js", "console.log(navigator.userAgent);");

			var resourceProvider = new ResourceProvider(httpResourceProvider, null);
			
			var engine = new Engine(resourceProvider).UseCustomUserAgent("My favorite browser");

			var log = engine.Console.ToList();

			engine.OpenUrl("http://localhost").Wait();
			
			Assert.AreEqual(new[]{"My favorite browser"}, log);
			Assert.AreEqual(2, httpResourceProvider.History.Count);
			Assert.AreEqual("My favorite browser", httpResourceProvider.History[0].Headers["User-Agent"]);
			Assert.AreEqual("My favorite browser", httpResourceProvider.History[1].Headers["User-Agent"]);
		}

		[Test]
		public void XmlHttpRequestUserAgent()
		{
			var httpResourceProvider = Mocks.HttpResourceProvider()
				.Resource("http://localhost/", @"<html><script>
		
function reqListener () {
		console.log(this.responseText);
}
		var oReq = new XMLHttpRequest();
		oReq.onload = reqListener;
		oReq.open('get', 'data.txt', false);
		oReq.send();</script></html>")
				.Resource("http://localhost/data.txt", "data!");

			var resourceProvider = new ResourceProvider(httpResourceProvider, null);
			
			var engine = new Engine(resourceProvider);
			((Navigator)engine.Window.Navigator).UserAgent = "My favorite browser";

			var log = engine.Console.ToList();

			engine.OpenUrl("http://localhost").Wait();
			
			Assert.AreEqual(new[]{"data!"}, log);
			Assert.AreEqual(2, httpResourceProvider.History.Count);
			Assert.AreEqual("My favorite browser", httpResourceProvider.History[0].Headers["User-Agent"]);
			Assert.AreEqual("My favorite browser", httpResourceProvider.History[1].Headers["User-Agent"]);
		}

		[TestCase("{\"data\":\"hello\"}", "hello")]
		[TestCase("hello", null)]//bad json
		public void XmlHttpRequestJson(string json, string expectedMsg)
		{
			var httpResourceProvider = Mocks.HttpResourceProvider()
				.Resource("http://localhost/", @"<html><script>
	var xhr = new XMLHttpRequest();
	xhr.open('GET', 'data.json', true);
	xhr.responseType = 'json';
	xhr.onload = function () {
		if (xhr.readyState === xhr.DONE) {
			if (xhr.status === 200) {
				console.log(xhr.response == null ? null : xhr.response.data);
				var div = document.createElement('div');
				div.id='finished';
				document.body.appendChild(div);
			}
		}
	};
	xhr.send(null);</script></html>")
				.Resource("http://localhost/data.json", json);

			var resourceProvider = new ResourceProvider(httpResourceProvider, null);
			
			var engine = new Engine(resourceProvider);
			((Navigator)engine.Window.Navigator).UserAgent = "My favorite browser";

			var log = engine.Console.ToList();

			engine.OpenUrl("http://localhost").Wait();
			
			Assert.IsNotNull(engine.WaitId("finished"));
			Assert.AreEqual(new[]{expectedMsg}, log);
		}

		[Test]
		public void LoadScriptFromBase64()
		{
			var scriptCode = System.Convert.ToBase64String( System.Text.Encoding.UTF8.GetBytes("console.log('hi');"));
			
			var httpResourceProvider = Mocks.HttpResourceProvider()
				.Resource("http://localhost/", $"<html><script src='data:text/javascript;base64,{scriptCode}'/></html>");

			var resourceProvider = new ResourceProvider(httpResourceProvider, null);
			
			var engine = new Engine(resourceProvider);

			var log = engine.Console.ToList();
			
			engine.OpenUrl("http://localhost").Wait();
			
			Assert.AreEqual(new []{"hi"}, log);
		}

		[Test]
		public void LoadPageFromBase64()
		{
			var htmlCode = System.Convert.ToBase64String( System.Text.Encoding.UTF8.GetBytes("<html><body>HELLO</body></html>"));
			
			var engine = new Engine();

			engine.OpenUrl($"data:text/html;base64,{htmlCode}").Wait();
			
			Assert.AreEqual("HELLO", engine.Document.Body.TextContent);
		}

		[Test]
		public void SendCookies()
		{
			var index = "<html><head><script>document.cookie = 'name=ivan'</script><script src='test.js'/>";
			var script = "console.log(document.cookie)";
			
			var requests = new List<Request>();

			var resourceProvider = Mock.Of<IResourceProvider>();
			Mock.Get(resourceProvider).Setup(x => x.SendRequestAsync(It.IsAny<Request>()))
				.Returns<Request>(req =>
				{
					requests.Add(req);
					return Task.Run(() =>
						req.Url.ToString() == "http://localhost/"
							? new Response("text/html", new MemoryStream(Encoding.UTF8.GetBytes(index)))
							: req.Url.ToString() == "http://localhost/test.js"
								? new Response("text/html", new MemoryStream(Encoding.UTF8.GetBytes(script)))
								: (IResource) null);
				});

			var engine = new Engine(resourceProvider);
			engine.OpenUrl("http://localhost").Wait();
			
			Assert.AreEqual(2, requests.Count, "requests count");
			Assert.IsNotNull(requests[1].Cookies);
			Assert.AreEqual(1, requests[1].Cookies.Count);
			Assert.AreEqual("ivan", requests[1].Cookies.GetCookies(new Uri("http://localhost/"))["name"].Value);
		}

		[Test]
		public async Task NotFoundHttpStatusCode()
		{
			var resourceProvider = new ResourceProvider(Mocks.HttpResourceProvider(), null);
			var engine = new Engine(resourceProvider);

			var result = await engine.OpenUrl("http://some.site") as HttpPage;

			Assert.AreEqual(HttpStatusCode.NotFound, result.HttpStatusCode);
		}

		[Test]
		public void PreloadResourceUsingPredictedResourceProvider()
		{
			var prerequests = new List<Request>();
			var index = "<html><head><script src='test.js'/>";
			var script = "console.log('hi')";
			
			var resourceProvider = Mock.Of<IPredictedResourceProvider>();
			Mock.Get(resourceProvider).Setup(x => x.SendRequestAsync(It.IsAny<Request>()))
				.Returns<Request>(req => Task.Run(() =>
					req.Url.ToString() == "http://localhost/"
						? new Response("text/html", new MemoryStream(Encoding.UTF8.GetBytes(index)))
						: req.Url.ToString() == "http://localhost/test.js"
							? new Response("text/html", new MemoryStream(Encoding.UTF8.GetBytes(script)))
							: (IResource) null));
			
			Mock.Get(resourceProvider).Setup(x => x.Preload(It.IsAny<Request>())).Callback<Request>(req => prerequests.Add(req));
			
			var engine = new Engine(resourceProvider);
			engine.OpenUrl("http://localhost").Wait();
			
			Assert.AreEqual(1, prerequests.Count);
			Assert.Greater(prerequests[0].Headers.Count, 0);
		}

		[TestCase("data:image/bmp;base64,Qk2WAAAAAAAAADYAAAAoAAAACAAAAAQAAAABABgAAAAAAGAAAAAAAAAAAAAAAAAAAAAAAAAA////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////")]
		[TestCase("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAgAAAAECAYAAACzzX7wAAAABHNCSVQICAgIfAhkiAAAABZJREFUCJlj/P///38GPIAJnyR1FAAABqwEBGR0hh0AAAAASUVORK5CYII=")]
		public async Task LoadImageFromData(string url)
		{
			var resourceProvider = new ResourceProvider(Mocks.HttpResourceProvider().Resource("http://localhost/",""), null);
			var engine = new Engine(resourceProvider);
			var page = await engine.OpenUrl("http://localhost");
			var img = (HtmlImageElement)page.Document.CreateElement(TagsNames.Img);
			var loadSignal = new ManualResetEvent(false);
			img.OnLoad += evt => { loadSignal.Set(); };
			img.Src = url;
			Assert.IsTrue(loadSignal.WaitOne(1000));
			Assert.AreEqual(8, img.NaturalWidth);
			Assert.AreEqual(4, img.NaturalHeight);
		}

		[Test]
		public async Task LoadBmpFromUrl()
		{
			var httpResourceProvider = Mocks.HttpResourceProvider()
				.Resource("http://localhost/", "")
				.Resource("http://localhost/image.bmp",
					Convert.FromBase64String(
						"Qk2WAAAAAAAAADYAAAAoAAAACAAAAAQAAAABABgAAAAAAGAAAAAAAAAAAAAAAAAAAAAAAAAA////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////"),
					"image/bmp");
			
			var resourceProvider = new ResourceProvider(httpResourceProvider, null);
			var engine = new Engine(resourceProvider);
			var page = await engine.OpenUrl("http://localhost");
			var img = (HtmlImageElement)page.Document.CreateElement(TagsNames.Img);
			var loadSignal = new ManualResetEvent(false);
			img.OnLoad += evt => { loadSignal.Set(); };
			img.Src = "image.bmp";
			Assert.IsTrue(loadSignal.WaitOne(1000));
			Assert.AreEqual(8, img.NaturalWidth);
			Assert.AreEqual(4, img.NaturalHeight);
		}

		[Test]
		public async Task LoadBmpError()
		{
			var httpResourceProvider = Mocks.HttpResourceProvider().Resource("http://localhost/", "");
			
			var resourceProvider = new ResourceProvider(httpResourceProvider, null);
			var engine = new Engine(resourceProvider);
			var page = await engine.OpenUrl("http://localhost");
			var img = (HtmlImageElement)page.Document.CreateElement(TagsNames.Img);
			var errorSignal = new ManualResetEvent(false);
			img.OnError += evt => { errorSignal.Set(); };
			img.Src = "image.bmp";
			Assert.IsTrue(errorSignal.WaitOne(1000));
		}

		[Test]
		public async Task StyleOnLoad()
		{
			var engine = new Engine(Mocks.ResourceProvider("http://loc/", 
@"<html><body></body>
<script>
	document.body.onload=function(){console.log('body onload')};
	var style=document.createElement('style');
	style.innerHTML='div{border:1px solid red}';
	var onLoadCalled = false;
	style.onload=function(){ console.log('style onload')};

	var cnt = document.createElement('div');
	cnt.appendChild(style);

	console.log('add');
	document.head.appendChild(style)
	console.log('added');
</script>

<script>
		console.log('script2')
</script></html>"));

			var log = engine.Console.ToList();

			var page = await engine.OpenUrl("http://loc/");
			
			Assert.IsNotNull(page.Document.GetElementsByTagName("style").FirstOrDefault());
			Assert.AreEqual(new[]{"add", "added", "script2", "style onload", "body onload"}, log);
		}
	}
}