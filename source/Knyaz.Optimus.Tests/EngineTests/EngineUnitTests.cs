using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Knyaz.NUnit.AssertExpressions;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.Tests.TestingTools;
using Moq;
using NUnit.Framework;
using Text = Knyaz.Optimus.Dom.Elements.Text;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public partial class EngineUnitTests
	{

		[Test]
		public async Task EmptyHtml()
		{
			var engine = TestingEngine.BuildJint(Mocks.ResourceProvider("http://localhost", "<html></html>"));
			await engine.OpenUrl("http://localhost");
		}
		
		private async Task<Document> Load(IResourceProvider resourceProvider)
		{
			var engine = TestingEngine.BuildJint(resourceProvider);
			return (await engine.OpenUrl("http://localhost")).Document;
		}


		[Test]
		public async Task GenerateContent()
		{
			var document = await Load(Mock.Of<IResourceProvider>()
				.Resource("http://localhost", "<html><head><script src='test.js' defer/></head><body><div id='content'></div></body></html>")
				.Resource("http://localhost/test.js", "var elem = document.getElementById('content');elem.innerHTML = 'Hello';"));

			
			var contentDiv = document.GetElementById("content");
			Assert.AreEqual("Hello", contentDiv.InnerHTML);
			Assert.AreEqual(1, contentDiv.ChildNodes.Count);
			var text = contentDiv.ChildNodes[0] as Text;
			Assert.IsNotNull(text);
			Assert.AreEqual("Hello", text.Data);
		}

		[Test]
		public async Task DomManipulation()
		{
			var document = await Load(Mock.Of<IResourceProvider>()
				.Resource("http://localhost", "<html><head><script src='test.js' defer/></head><body><div id='content1'></div><div id='content2'></div></body></html")
				.Resource("http://localhost/test.js", "var div = document.createElement('div');" +
			                                     "div.setAttribute('id', 'c3');" +
			                                     "var c2 = document.getElementById('content2');" +
			                                     "document.documentElement.getElementsByTagName('body')[0].insertBefore(div, c2);"));
			
			Assert.AreEqual(3, document.DocumentElement.GetElementsByTagName("body")[0].ChildNodes.Count);
			var elem = document.GetElementById("c3");
			Assert.IsNotNull(elem);
		}

		[Test]
		public async Task Text()
		{
			var html = Mocks.Page(@"var c2 = document.getElementById('content1').innerHTML = 'Hello';",
				"<span id='content1'></span>");
			var resources = Mocks.ResourceProvider("http://localhost", html);
			var engine =TestingEngine.BuildJint(resources, SystemConsole.Instance);
			var page = await engine.OpenUrl("http://localhost");
			var elem = page.Document.GetElementById("content1");
			Assert.AreEqual("Hello", elem.InnerHTML);
		}

		[Test]
		public async Task GetAttribute()
		{
			var resources = Mocks.ResourceProvider("http://localhost",
				Mocks.Page(@"console.log(document.getElementById('content1').getAttribute('id'));",
					"<span id='content1'></span>"));
			var consoleMock = new Mock<IConsole>();

			var engine = TestingEngine.BuildJint(resources, consoleMock.Object);
			string attr = null;
			await engine.OpenUrl("http://localhost");
			
			consoleMock.Verify(x => x.Log("content1"), Times.Once);
		}

		[Test]
		public void LoadPage()
		{
			var resourceProvider = Mocks.ResourceProvider(
				"http://localhost", "<html><body><div id='c'></div></body></html>");

			var engine = Builder(resourceProvider).Build();
			engine.OpenUrl("http://localhost");
			engine.WaitId("c");
			Assert.AreEqual(1, engine.Document.Body.GetElementsByTagName("div").Count);
		}

		[Test]
		public async Task NonJsScript()
		{
			var resources = Mocks.ResourceProvider("http://localhost",
				"<html><head><script id='template' type='text/html'><div>a</div></script></head></html>");
			var engine = TestingEngine.BuildJint(resources);
			var page = await engine.OpenUrl("http://localhost");
			var script = page.Document.GetElementById("template");
			Assert.AreEqual("<div>a</div>", script.InnerHTML);
		}

		[Test]
		public async Task LoadScriptTest()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost/script.js", "console.log('hello');")
				.Resource("http://localhost", "<html><head><script src='http://localhost/script.js'></script></head></html>");
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			await engine.OpenUrl("http://localhost");
			Assert.AreEqual(new[]{"hello"}, console.LogHistory);
		}

		[Test]
		public async Task DocumentReadyStateComplete()
		{
			var resources = Mocks.ResourceProvider("http://localhost", "<html><body></body></html>");
			var engine = TestingEngine.BuildJint(resources);
			var page = await engine.OpenUrl("http://localhost");
			Assert.AreEqual(DocumentReadyStates.Complete, page.Document.ReadyState);
		}

		[Test]
		public async Task LoadAndRunScriptAddedInRuntime()
		{
			var resourceProvider = 
				Mocks.ResourceProvider("http://localhost/script.js", "console.log('hello');")
					.Resource("http://localhost", "<html><head></head></html>");
			
			var signal = new ManualResetEvent(false);
			var console = new Mock<IConsole>();
			console.Setup(x => x.Log(It.IsAny<string>())).Callback(() => signal.Set());

			var engine = TestingEngine.BuildJint(resourceProvider, console.Object); 

			var page = await engine.OpenUrl("http://localhost");

			var script = (HtmlScriptElement)page.Document.CreateElement("script");
			script.Src = "http://localhost/script.js";
			page.Document.Head.AppendChild(script);

			Assert.True(signal.WaitOne(1000));

			//todo: Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("http://localhost/script.js"), Times.Once());
			console.Verify(x => x.Log("hello"), Times.Once);
		}

		[TestCase("var timer = window.setTimeout(function(x){console.log(x);}, 300, 'ok');")]
		[TestCase("var timer = window.setTimeout(function(x){console.log('ok');}, 300);")]
		public async Task SetTimeout(string code)
		{
			var resources = Mocks.ResourceProvider("http://localhost", Mocks.Page(code));
			var signal = new ManualResetEvent(false);
			var console = new Mock<IConsole>();
			console.Setup(x => x.Log(It.IsAny<string>())).Callback(() => signal.Set());
			var engine = TestingEngine.BuildJint(resources, console.Object); 
			await engine.OpenUrl("http://localhost");
			Assert.IsTrue(signal.WaitOne(1000));
			console.Verify(x => x.Log("ok"), Times.Once);
		}

		[Test, Ignore("For manual run")]
		public async Task ClearTimeout()
		{
			var resources = Mocks.ResourceProvider(
				"http://localhost", Mocks.Page(
					@"var timer = window.setTimeout(function(){console.log('ok');}, 500);
                                    window.clearTimeout(timer);"));
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resources, console);
			await engine.OpenUrl("http://localhost");
			Assert.AreEqual(0, console.LogHistory.Count);
			Thread.Sleep(1000);
			Assert.AreEqual(0, console.LogHistory.Count);
		}
		

		[Test]
		public void Location()
		{
			var resourceProvider = Mocks.ResourceProvider("http://todosoft.ru", "");
			var engine = TestingEngine.BuildJint(resourceProvider);
			engine.OpenUrl("http://todosoft.ru");
			Assert.AreEqual("http://todosoft.ru/", engine.Window.Location.Href);
			Assert.AreEqual("http:", engine.Window.Location.Protocol);
		}

		[Test]
		public void SetLocationHref()
		{
			var resourceProvider = Mocks.ResourceProvider("http://todosoft.ru", "");
			var engine = TestingEngine.BuildJint(resourceProvider);
			//todo: write similar test on js
			engine.Window.Location.Href = "http://todosoft.ru";
//todo:			Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("http://todosoft.ru"), Times.Once());
		}

		[Test]
		public void Ajax()
		{
			var resourceProvider = Mock.Of<IResourceProvider>().Resource("http://localhost/unicorn.xml", "hello");

			string responseText = null;
			
			var engine = EngineBuilder.New()
				.SetResourceProvider(resourceProvider)
				.UseJint()
				.Build();
				
			var client = new XmlHttpRequest(engine.ResourceProvider, () => this, engine.Document, (u,m) => engine.CreateRequest(u,m));

			client.OnReadyStateChange += () =>
				{
					if (client.ReadyState == XmlHttpRequest.DONE)
					{
						if (client.Status == 200)
						{
							responseText = client.ResponseText;
						}
					}
				};
			client.Open("GET", "http://localhost/unicorn.xml", false);
			client.Send();
			
			Mock.Get(resourceProvider).Verify(x => x.SendRequestAsync(It.IsAny<Request>()), Times.Once());
			Assert.AreEqual("hello", responseText);
		}

		[Test, Ignore("Unstable")]
		public async Task AddScriptAsync()
		{
			var resources = Mocks.ResourceProvider("http://localhost/script.js", "console.log('in new script');")
				.Resource("http://localhost", "<html><head></head></html>");
			
			var console = new TestingConsole();
			//todo: rewrite it with JS. current test is not stable due to multithreading.
			var engine = TestingEngine.BuildJint(resources, console); 

			var page = await engine.OpenUrl("http://localhost");
			page.Document.AddEventListener("DOMNodeInserted", @event =>
			{
				console.Log("nodeadded");
			}, false);

			var onloadSignal = new ManualResetEvent(false);
			
			var d = (HtmlScriptElement)page.Document.CreateElement("script");
			d.Id = "aaa";
			d.Async = true;
			d.Src = "http://localhost/script.js";
			d.OnLoad += e =>
			{
				console.Log("onload");
				onloadSignal.Set();
			};
			page.Document.Head.AppendChild(d);

			onloadSignal.WaitOne(10000);
			CollectionAssert.AreEqual(new[] { "nodeadded", "in new script", "onload" }, console.LogHistory);
		}

		[Test]
		public async Task RaiseExecuteScript()
		{
			var resources = Mocks.ResourceProvider(
				"http://localhost", "<html><head></head><body></body></html>");
			var engine = TestingEngine.BuildJint(resources); 
			var page = await engine.OpenUrl("http://localhost");
			var doc = page.Document;
			
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
		public async Task AppendScriptAsInnerHtml()
		{
			var console = new Mock<IConsole>();
			var resources = Mocks.ResourceProvider("http://localhost", "<html><head></head><body></body></html>");
			var engine = TestingEngine.BuildJint(resources, console.Object); 
			var page = await engine.OpenUrl("http://localhost");
			page.Document.Body.InnerHTML = "<script>console.log('HI');</script>";
			console.Verify(x => x.Log("HI"), Times.Once);
		}

		[TestCase("http://todosoft.ru", "http://todosoft.ru/")]
		[TestCase("http://todosoft.ru/", "http://todosoft.ru/")]
		[TestCase("http://todosoft.ru/subfolder", "http://todosoft.ru/subfolder/")]
		[TestCase("http://todosoft.ru/index.html", "http://todosoft.ru/index.html")]
		public async Task OpenUrl(string url, string expectedRoot)
		{
			var resourceProvider = Mocks.ResourceProvider(url.TrimEnd('/'), "<html><head></head></html>");

			var engine = Builder(resourceProvider).Build();
			await engine.OpenUrl(url);
			
			Assert.AreEqual(expectedRoot, engine.LinkProvider.Root);
		}

		[TestCase("http://a.ru/index.html", "k.js", "http://a.ru/k.js")]
		[TestCase("http://a.ru/test", "./js/k.js", "http://a.ru/test/js/k.js")]
		[TestCase("http://a.ru/test", "/js/k.js", "http://a.ru/js/k.js")]
		[TestCase("http://chromium.github.io/octane", "js/jquery.js", "http://chromium.github.io/octane/js/jquery.js")]
		public async Task OpenUrlWithResource(string url, string resUrl, string expectedResUrl)
		{
			var resourceProvider = Mocks.ResourceProvider(url.TrimEnd('/'), "<html><head><script src='"+resUrl+"'></script></head></html>")
				.Resource(expectedResUrl, "console.log('ok');");

			var consoleMock = new Mock<IConsole>();

			var engine = Builder(resourceProvider).Window(w => w.SetConsole(consoleMock.Object)).Build();

			await engine.OpenUrl(url);
			
			consoleMock.Verify(x => x.Log("ok"), Times.Once);
		}
		
		[Test]
		public async Task SetUpUserAgent()
		{
			var httpResourceProvider = Mocks.HttpResourceProvider()
				.Resource("http://localhost/", "<html><script src='sc.js'/></html>")
				.Resource("http://localhost/sc.js", "console.log(navigator.userAgent);");

			var resourceProvider = new ResourceProvider(httpResourceProvider, null);

			var console = new TestingConsole();
			var engine = Builder(resourceProvider)
				.Window(w => w.SetConsole(console))
				.Build()
				.UseCustomUserAgent("My favorite browser");

			await engine.OpenUrl("http://localhost");
			
			Assert.AreEqual(new[]{"My favorite browser"}, console.LogHistory);
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
			
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			((Navigator)engine.Window.Navigator).UserAgent = "My favorite browser";

			engine.OpenUrl("http://localhost").Wait();
			
			Assert.AreEqual(new[]{"data!"}, console.LogHistory);
			Assert.AreEqual(2, httpResourceProvider.History.Count);
			Assert.AreEqual("My favorite browser", httpResourceProvider.History[0].Headers["User-Agent"]);
			Assert.AreEqual("My favorite browser", httpResourceProvider.History[1].Headers["User-Agent"]);
		}

		[TestCase("{\"data\":\"hello\"}", "hello")]
		[TestCase("hello", null)]//bad json
		public void XmlHttpRequestJson(string json, string expectedMsg)
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost/", @"<html><script>
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

			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			((Navigator)engine.Window.Navigator).UserAgent = "My favorite browser";

			engine.OpenUrl("http://localhost").Wait();
			
			Assert.IsNotNull(engine.WaitId("finished"));
			Assert.AreEqual(new[]{expectedMsg}, console.LogHistory);
		}

		[Test]
		public void XmlHttpRequestThis()
		{
			var console = new Mock<IConsole>();
		
			var resourceProvider = Mocks.ResourceProvider("http://localhost/", @"<html><script>
	var xhr = new XMLHttpRequest();
	xhr.open('GET', 'data.json', false);
	xhr.responseType = 'json';
	xhr.onload = function () {
		console.log(xhr == this);		
	};
	xhr.send(null);</script></html>")
				.Resource("http://localhost/data.json", "");

			var engine = Builder(resourceProvider, console.Object).Build();
			((Navigator)engine.Window.Navigator).UserAgent = "My favorite browser";

			engine.OpenUrl("http://localhost").Wait();

			console.Verify(x => x.Log(true), Times.Once);
		}

		[Test]
		public async Task LoadScriptFromBase64()
		{
			
			var scriptCode = Convert.ToBase64String(Encoding.UTF8.GetBytes("console.log('hi');"));
			
			var httpResourceProvider = Mocks.HttpResourceProvider()
				.Resource("http://localhost/", $"<html><script src='data:text/javascript;base64,{scriptCode}'/></html>");

			var resourceProvider = new ResourceProvider(httpResourceProvider, null);

			var console = new Mock<IConsole>();

			var engine = Builder(resourceProvider, console.Object).Build();
			
			await engine.OpenUrl("http://localhost");
			
			console.Verify(x => x.Log("hi"), Times.Once);
		}

		[Test]
		public async Task LoadPageFromBase64()
		{
			var htmlCode = Convert.ToBase64String(Encoding.UTF8.GetBytes("<html><body>HELLO</body></html>"));
			
			var engine = EngineBuilder.New().Build();

			await engine.OpenUrl($"data:text/html;base64,{htmlCode}");
			
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

			var engine = TestingEngine.BuildJint(resourceProvider);
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
			var engine = TestingEngine.BuildJint(resourceProvider);

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

			var engine = EngineBuilder.New().SetResourceProvider(resourceProvider).Build(); 
			engine.OpenUrl("http://localhost").Wait();
			
			Assert.AreEqual(1, prerequests.Count);
			Assert.Greater(prerequests[0].Headers.Count, 0);
		}

		[TestCase("data:image/bmp;base64,Qk2WAAAAAAAAADYAAAAoAAAACAAAAAQAAAABABgAAAAAAGAAAAAAAAAAAAAAAAAAAAAAAAAA////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////")]
		[TestCase("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAgAAAAECAYAAACzzX7wAAAABHNCSVQICAgIfAhkiAAAABZJREFUCJlj/P///38GPIAJnyR1FAAABqwEBGR0hh0AAAAASUVORK5CYII=")]
		public async Task LoadImageFromData(string url)
		{
			var resourceProvider = new ResourceProvider(Mocks.HttpResourceProvider().Resource("http://localhost/",""), null);
			var engine = TestingEngine.BuildJint(resourceProvider);
			var page = await engine.OpenUrl("http://localhost");
			var img = (HtmlImageElement)page.Document.CreateElement(TagsNames.Img);
			var loadSignal = new ManualResetEvent(false);
			img.OnLoad += evt => { loadSignal.Set(); };
			img.Src = url;
			Assert.IsTrue(loadSignal.WaitOne(1000));
			Assert.IsTrue(img.Complete);
			Assert.AreEqual(8, img.NaturalWidth);
			Assert.AreEqual(4, img.NaturalHeight);
		}

		[Test]
		public async Task LoadBmpFromUrlAndGetSize()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost/", "")
				.Resource("http://localhost/image.bmp",
					Convert.FromBase64String(
						"Qk2WAAAAAAAAADYAAAAoAAAACAAAAAQAAAABABgAAAAAAGAAAAAAAAAAAAAAAAAAAAAAAAAA////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////"),
					"image/bmp");
			
			var engine = TestingEngine.BuildJint(resourceProvider);
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
		public async Task GetImageRawData()
		{
			var resourceProvider = 
				Mocks.ResourceProvider("http://localhost/", "")
				.Resource("http://localhost/image.bmp", new byte[]{1,2,3,2,1},"image/bmp");
			
			var engine = TestingEngine.BuildJint(resourceProvider);
			var page = await engine.OpenUrl("http://localhost");
			var img = (HtmlImageElement)page.Document.CreateElement(TagsNames.Img);
			var loadSignal = new ManualResetEvent(false);
			img.OnLoad += evt => { loadSignal.Set(); };
			img.Src = "image.bmp";
			Assert.IsTrue(loadSignal.WaitOne(1000));

			var data = new MemoryStream();
			img.ImageData.Data.CopyTo(data);
			Assert.IsTrue(img.Complete);
			Assert.AreEqual(new byte[]{1,2,3,2,1}, data.ToArray());
		}
		
		[Test]
		public async Task DocumentWithImage()
		{
			var resourceProvider = 
				Mocks.ResourceProvider("http://localhost/", "<html><body><img src='image.bmp'/></body></html>")
				.Resource("http://localhost/image.bmp", new byte[]{1,2,3,2,1},"image/bmp");
			
			var engine = TestingEngine.BuildJint(resourceProvider);
			var page = await engine.OpenUrl("http://localhost");
			var img = (HtmlImageElement)page.Document.GetElementsByTagName("img").First();
			
			var loadSignal = new ManualResetEvent(false);
			img.OnLoad += evt => { loadSignal.Set(); };
			img.Src = "image.bmp";
			Assert.IsTrue(loadSignal.WaitOne(1000));
			Assert.IsTrue(img.Complete);
		}

		private static string Img64 =
			"Qk2WAAAAAAAAADYAAAAoAAAACAAAAAQAAAABABgAAAAAAGAAAAAAAAAAAAAAAAAAAAAAAAAA////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////";
		
		[Test, Ignore("To be fixed")]
		public async Task DocumentWithEmbeddedImageEventsOrder()
		{
			var httpResourceProvider = Mocks.HttpResourceProvider()
				.Resource("http://localhost/",
					"<html><body onload='console.log(\"body\")'>" +
					"<img src='data:image/bmp;base64,{Img64}'/>" +
					"<script>" +
					"document.getElementsByTagName('img')[0].onload=function(){console.log('img');}"+
					"document.addEventListener('DOMContentLoaded', function(){console.log('DOMContentLoaded-C');}, true);"+
					"document.addEventListener('DOMContentLoaded', function(){console.log('DOMContentLoaded-B');}, false);"+
					"</script>" +
					"</body></html>"
				); 
			
			var resourceProvider = new ResourceProvider(httpResourceProvider, null);
			
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			var page = await engine.OpenUrl("http://localhost");
			Thread.Sleep(5000);//rewrite
			
			Assert.AreEqual(new[] {
				"DOMContentLoaded-C",
				"DOMContentLoaded-B",
				"img",
				"body"
			}, console.LogHistory);
			
			((HtmlImageElement) page.Document.GetElementsByTagName(TagsNames.Img).First())
				.Assert(img => img.NaturalWidth == 8 && img.NaturalHeight == 4);
		}
		
		[Test]
		public async Task DocumentWithEmbeddedImage()
		{
			var httpResourceProvider = Mocks.HttpResourceProvider()
				.Resource("http://localhost/",
					"<html><body><img " +
					"onload='document.body.appendChild(document.createElement(\"span\"))' " +
					$"src='data:image/bmp;base64,{Img64}'/></body></html>"
				); 
			
			var resourceProvider = new ResourceProvider(httpResourceProvider, null);
			
			var engine = TestingEngine.BuildJint(resourceProvider);
			var page = await engine.OpenUrl("http://localhost");
			
			Assert.IsNotNull(page.Document.WaitSelector("span", 1000), "Onload fired");

			((HtmlImageElement) page.Document.GetElementsByTagName(TagsNames.Img).First())
				.Assert(img => img.NaturalWidth == 8 && img.NaturalHeight == 4);
		}
		
		[Test]
		public async Task SetAndResetImage()
		{
			var httpResourceProvider = Mocks.HttpResourceProvider()
				.Resource("http://localhost/", "")
				.Resource("http://localhost/image.bmp",
					Convert.FromBase64String(Img64),
					"image/bmp");
			
			var resourceProvider = new ResourceProvider(httpResourceProvider, null);
			var engine = TestingEngine.BuildJint(resourceProvider);
			var page = await engine.OpenUrl("http://localhost");
			var img = (HtmlImageElement)page.Document.CreateElement(TagsNames.Img);
			var loadSignal = new ManualResetEvent(false);
			img.OnLoad += evt => { loadSignal.Set(); };
			img.Src = "image.bmp";
			Assert.IsTrue(loadSignal.WaitOne(1000));

			img.Src = "";
			Assert.IsTrue(img.Complete);
			Assert.AreEqual(0, img.NaturalWidth);
			Assert.AreEqual(0, img.NaturalHeight);
		}

		[Test]
		public async Task LoadBmpError()
		{
			var httpResourceProvider = Mocks.HttpResourceProvider().Resource("http://localhost/", "");
			
			var resourceProvider = new ResourceProvider(httpResourceProvider, null);
			var engine = TestingEngine.BuildJint(resourceProvider);
			var page = await engine.OpenUrl("http://localhost");
			var img = (HtmlImageElement)page.Document.CreateElement(TagsNames.Img);
			var errorSignal = new ManualResetEvent(false);
			img.OnError += evt => { errorSignal.Set(); };
			img.Src = "image.bmp";
			Assert.IsTrue(errorSignal.WaitOne(1000));
		}

		[Test]
		public async Task UrlWithHash()
		{
			var httpResourceProvider = Mocks.HttpResourceProvider().Resource("http://localhost/index.html", "hello");
			
			var resourceProvider = new ResourceProvider(httpResourceProvider, null);
			
			var engine = TestingEngine.BuildJint(resourceProvider);

			var page = await engine.OpenUrl("http://localhost/index.html#some");
			
			page.Document.Assert(document => 
				document.InnerHTML == "<HTML><HEAD></HEAD><BODY>hello</BODY></HTML>"
				&& page.Document.Location.Href == "http://localhost/index.html#some"
				&& page.Document.Location.Hash == "#some");
		}

		[Test, Ignore("Failed")]
		public async Task StyleOnLoad()
		{
			var console = new TestingConsole();
			var engine = Builder(Mocks.ResourceProvider("http://loc/", 
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
</script></html>"))
				.Window(w => w.SetConsole(console))
				.Build();

			var page = await engine.OpenUrl("http://loc/");
			
			Assert.IsNotNull(page.Document.GetElementsByTagName("style").FirstOrDefault());
			Assert.AreEqual(new[]{"add", "added", "script2", "style onload", "body onload"}, console.LogHistory);
		}

		[Test, Ignore("Failed")]
		public async Task OverrideBodyOnLoadFromScript()
		{
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(Mocks.ResourceProvider("http://loc/", 
@"<html><body onload='console.log(""body onload attr"")'></body>
<script>
	document.body.onload=function(){console.log('body onload script')};
</script></html>"), console);

			var page = await engine.OpenUrl("http://loc/");
			
			Assert.AreEqual(new[]{"body onload script"}, console.LogHistory);
		}

		private static EngineBuilder Builder(IResourceProvider resources) =>
			EngineBuilder.New().SetResourceProvider(resources).UseJint();
		
		private static EngineBuilder Builder(IResourceProvider resources, IConsole console) =>
			EngineBuilder.New().SetResourceProvider(resources)
				.Window(w => w.SetConsole(console))
				.UseJint();
		
		[Test]
		public async Task BodyOnLoad()
		{
			var resources = Mocks.ResourceProvider("http://loc/",
				@"<html><body onload='console.log(""body onload attr"")'></body>");

			var consoleMock = new Mock<IConsole>();
			
			var engine = Builder(resources).Window(w => w.SetConsole(consoleMock.Object)).Build(); 
				
			var page = await engine.OpenUrl("http://loc/");
			
			consoleMock.Verify(x => x.Log("body onload attr"), Times.Once);
		}

		[Test, Ignore("Failed")]
		public async Task LinkOnError()
		{
			var console = new TestingConsole();
			var resourceProvider = Mocks.ResourceProvider("http://loc/",
				@"<html><body></body>
<script>	
	document.body.onload=function(){console.log('body onload');};
	var link = document.createElement('link');
	link.onload  = function(){console.log('ok')};
	link.onerror = function(){console.log('link onerror')};
	link.rel='stylesheet';
	link.href='mystylesheet.css';
	document.head.appendChild(link);
	console.log('added');
</script>
</html>");
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			await engine.OpenUrl("http://loc/");
			Assert.AreEqual(new[]{"added","link onerror","body onload"}, console.LogHistory);
		}
		
		[Test, Ignore("Failed")]
		public async Task LinkOnLoad()
		{
			var resourceProvider = Mocks.ResourceProvider("http://loc/",
				@"<html><body></body>
<script>	
	document.body.onload=function(){console.log('body onload');};
	var link = document.createElement('link');
	link.onload  = function(){console.log('ok')};
	link.onerror = function(){console.log('link onerror')};
	link.rel='stylesheet';
	link.href='mystylesheet.css';
	document.head.appendChild(link);
	console.log('added');
</script>
</html>").Resource("http://loc/mystyesheet.css","*{border:1px solid black}");
			
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			await engine.OpenUrl("http://loc/");
			Assert.AreEqual(new[]{"added","ok","body onload"}, console.LogHistory);
		}
	}
}