#if NUNIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Moq;
using NUnit.Framework;
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
			engine.Load("<html><head><script>var c2 = document.getElementById('content2');document.documentElement.getElementsByTagName('body')[0].insertBefore(document.createElement('<div id=\"c3\">'), c2);</script></head><body><div id='content1'></div><div id='content2'></div></body></html>");
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
		public void LoadPage()
		{
			var resourceProviderMock = new Mock<IResourceProvider>();
			var resource = Mock.Of<IResource>(x => x.Stream == new MemoryStream(Encoding.UTF8.GetBytes("<html><body><div id='c'></div></body></html>")));

			resourceProviderMock.Setup(x => x.GetResource(It.IsAny<Uri>())).Returns(resource);

			var engine = new Engine(resourceProviderMock.Object);

			engine.OpenUrl("http://localhost");
			resourceProviderMock.Verify(x => x.GetResource(It.IsAny<Uri>()), Times.Once());

			Assert.AreEqual(1, engine.Document.Body.GetElementsByTagName("div").Length);
		}

		[Test]
		public void LoadScriptTest()
		{
			var resourceProviderMock = new Mock<IResourceProvider>();
			var resource = Mock.Of<IResource>(x => x.Stream == new MemoryStream(Encoding.UTF8.GetBytes("console.log('hello');")));
			
			resourceProviderMock.Setup(x => x.GetResource(It.IsAny<Uri>())).Returns(resource);

			var engine = new Engine(resourceProviderMock.Object);

			string loggedValue = null;
			engine.Console.OnLog += o => loggedValue = o.ToString();

			engine.Load("<html><head><script src='http://localhost/script.js'></script></head></html>");
		
			resourceProviderMock.Verify(x => x.GetResource(It.IsAny<Uri>()), Times.Once());
			Assert.AreEqual("hello", loggedValue);
		}

		[TestCase("http://okkamtech.com ")]
		[TestCase("http://ya.ru")]
		[TestCase("http://redmine.todosoft.org")]
		[TestCase("http://google.com")]
		public void OpenUrl(string url)
		{
			var engine = new Engine();
			engine.OpenUrl(url);
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
	}
}
#endif