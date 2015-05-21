#if NUNIT
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using WebBrowser.ResourceProviders;
using Text = WebBrowser.Dom.Text;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture]
	public class EngineGeneralTests
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
			Assert.AreEqual("Hello", contentDiv.InnerHtml);
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
			Assert.AreEqual("Hello", elem.InnerHtml);
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

		[TestCase("http://okkamtech.com")]
		[TestCase("http://ya.ru")]
		[TestCase("http://redmine.todosoft.org")]
		[TestCase("http://google.com")]
		[TestCase("https://html5test.com")]
		public void OpenUrl(string url)
		{
			var engine = new Engine();
			engine.OpenUrl(url);
		}

		[Test]
		public void Html5Score()
		{
			var engine = new Engine();
			engine.OpenUrl("https://html5test.com");

			var score = engine.Document.GetElementById("score");
			Assert.IsNotNull(score, "score");

			Thread.Sleep(1000);//wait calculation
			var tagWithValue = score.GetElementsByTagName("strong").FirstOrDefault();
			Assert.IsNotNull(tagWithValue, "strong");
			System.Console.WriteLine(tagWithValue.InnerHtml);
		}

		[Test]
		public void BrowseOkkam()
		{
			var engine = new Engine();
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception.ToString());
			engine.OpenUrl("http://okkamtech.com");
			Thread.Sleep(5000);
			var userName = engine.Document.GetElementById("UserName");
			Assert.IsNotNull(userName);
		}

		[Test]
		public void StyleTest()
		{
			var engine = new Engine();
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o == null ? "<null>" : o.ToString());
			engine.Load("<html><head><script>" +
@"var style = document.getElementById('content1').style;
console.log(style.getPropertyValue('width'));
console.log(style[0]);
console.log(style['width']);
</script></head><body><span id='content1' style='width:100pt; heigth:100pt'></span></body></html>");
			var elem = engine.Document.GetElementById("content1");
			Assert.IsNotNull(elem);
			CollectionAssert.AreEqual(new[] { "100pt", "width", "100pt" }, log);
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

		[Test]
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
				new HttpResponse(HttpStatusCode.OK, "hello"));
			
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
			Assert.AreEqual(1, log.Count);
			Assert.AreEqual("hello", log[0]);
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