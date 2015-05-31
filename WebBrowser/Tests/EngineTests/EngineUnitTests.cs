#if NUNIT
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;
using WebBrowser.ResourceProviders;
using Text = WebBrowser.Dom.Text;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture]
	public class EngineUnitTests
	{
		[Test]
		public void EmptyHtml()
		{
			var engine = new Engine();
			engine.Load("<html></html>");
		}
		
		[Test]
		public void GenerateContent()
		{
			var engine = new Engine();
			engine.Load("<html><head><script>var elem = document.getElementById('content');elem.innerHTML = 'Hello';</script></head><body><div id='content'></div></body></html>");
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
			var engine = new Engine();
			engine.Load("<html><head><script>" +
				"var div = document.createElement('div');" +
				"div.setAttribute('id', 'c3');" +
				"var c2 = document.getElementById('content2');" +
				"document.documentElement.getElementsByTagName('body')[0].insertBefore(div, c2);" +
				"</script></head><body><div id='content1'></div><div id='content2'></div></body></html>");
			Assert.AreEqual(3, engine.Document.DocumentElement.GetElementsByTagName("body")[0].ChildNodes.Count);
			var elem = engine.Document.GetElementById("c3");
			Assert.IsNotNull(elem);
		}

		[Test]
		public void Text()
		{
			var engine = new Engine();
			engine.Console.OnLog += System.Console.WriteLine;
			engine.Load("<html><head><script>"+
			@"var c2 = document.getElementById('content1').innerHTML = 'Hello';" +
				"</script></head><body><span id='content1'></span></body></html>");
			var elem = engine.Document.GetElementById("content1");
			Assert.AreEqual("Hello", elem.InnerHTML);
		}

		[Test]
		public void GetAttribute()
		{
			var engine = new Engine();
			string attr = null;
			engine.Console.OnLog += o => attr = o.ToString();
			engine.Load("<html><head><script>" +
			@"console.log(document.getElementById('content1').getAttribute('id'));" +
				"</script></head><body><span id='content1'></span></body></html>");
			Assert.AreEqual("content1", attr);
		}

		[Test]
		public void LoadPage()
		{
			var resourceProviderMock = new Mock<IResourceProvider>();
			var resource = Mock.Of<IResource>(x => x.Stream == new MemoryStream(Encoding.UTF8.GetBytes("<html><body><div id='c'></div></body></html>")));

			resourceProviderMock.Setup(x => x.GetResource(It.IsAny<string>())).Returns(resource);

			var engine = new Engine(resourceProviderMock.Object);

			engine.OpenUrl("http://localhost");
			resourceProviderMock.Verify(x => x.GetResource(It.IsAny<string>()), Times.Once());

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
			var resourceProviderMock = new Mock<IResourceProvider>();
			var resource = Mock.Of<IResource>(x => x.Stream == new MemoryStream(Encoding.UTF8.GetBytes("console.log('hello');")));

			resourceProviderMock.Setup(x => x.GetResource(It.IsAny<string>())).Returns(resource);

			var engine = new Engine(resourceProviderMock.Object);

			string loggedValue = null;
			engine.Console.OnLog += o => loggedValue = o.ToString();

			engine.Load("<html><head><script src='http://localhost/script.js'></script></head></html>");

			resourceProviderMock.Verify(x => x.GetResource(It.IsAny<string>()), Times.Once());
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
			var resourceProvider =
				Mock.Of<IResourceProvider>(
					x => x.GetResource(It.IsAny<string>()).Stream == new MemoryStream(Encoding.UTF8.GetBytes("console.log('hello');")));

			var engine = new Engine(resourceProvider);

			string loggedValue = null;
			engine.Console.OnLog += o => loggedValue = o.ToString();

			engine.Load("<html><head></head></html>");

			var script = (Script)engine.Document.CreateElement("script");
			script.Src = "http://localhost/script.js";
			engine.Document.Head.AppendChild(script);

			Mock.Get(resourceProvider).Verify(x => x.GetResource("http://localhost/script.js"), Times.Once());
			Assert.AreEqual("hello", loggedValue);
		}

		
		[Test]
		public void SetTimeout()
		{
			var engine = new Engine();
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o == null ? "<null>" : o.ToString());
			engine.Load("<html><head><script>" +
@"var timer = window.setTimeout(function(x){console.log(x);}, 300, 'ok');
</script></head><body></body></html>");
			Assert.AreEqual(0, log.Count);
			Thread.Sleep(1000);
			Assert.AreEqual(1, log.Count);
			Assert.AreEqual("ok",  log[0]);
		}

		[Test, Ignore]
		public void ClearTimeout()
		{
			var engine = new Engine();
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o == null ? "<null>" : o.ToString());
			engine.Load("<html><head><script>" +
@"var timer = window.setTimeout(function(){console.log('ok');}, 500);
window.clearTimeout(timer);
</script></head><body></body></html>");
			Assert.AreEqual(0, log.Count);
			Thread.Sleep(1000);
			Assert.AreEqual(0, log.Count);
		}

		[Test]
		public void Ajax()
		{
			var httpResourceProvider = Mock.Of<IHttpResourceProvider>(x => x.SendRequest(It.IsAny<HttpRequest>()) == 
				new HttpResponse(HttpStatusCode.OK, "hello", null));
			
			var resourceProvider = Mock.Of<IResourceProvider>(x => x.HttpResourceProvider == httpResourceProvider);
			
			var engine = new Engine(resourceProvider);
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o == null ? "<null>" : o.ToString());
			engine.Load("<html><head><script>" +
@"var client = new XMLHttpRequest();
client.onreadystatechange = function () {
  if(this.readyState == this.DONE) {
    if(this.status == 200 ) {
		console.log(this.responseXML);
    }
  }
};
client.open(""GET"", ""http://localhost/unicorn.xml"", false);
client.send();
</script></head><body></body></html>");
			Mock.Get(httpResourceProvider).Verify(x => x.SendRequest(It.IsAny<HttpRequest>()), Times.Once());
			Assert.AreEqual(1, log.Count);
			Assert.AreEqual("hello", log[0]);
		}

		[Test]
		public void Location()
		{
			var resourceProvider = Mocks.ResourceProvider("http://todosoft.ru", "");
			var engine = new Engine(resourceProvider);
			engine.OpenUrl("http://todosoft.ru");
			Assert.AreEqual("http://todosoft.ru", engine.Window.Location.Href);
			Assert.AreEqual("http:", engine.Window.Location.Protocol);
		}

		[Test]
		public void SetLocationHref()
		{
			var resourceProvider = Mocks.ResourceProvider("http://todosoft.ru", "");
			var engine = new Engine(resourceProvider);
			//todo: write similar test on js
			engine.Window.Location.Href = "http://todosoft.ru";
			Mock.Get(resourceProvider).Verify(x => x.GetResource("http://todosoft.ru"), Times.Once());
		}

		[Test]
		public void GetElementsByTagName()
		{
			var engine = new Engine();
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o == null ? "<null>" : o.ToString());
			engine.Load("<html><head><script>" +
@"console.log(document.getElementsByTagName('div').length);
</script></head><body><div></div><div></div></body></html>");
			Assert.AreEqual(1, log.Count);
			Assert.AreEqual("2", log[0]);
		}

		
	}
}
#endif