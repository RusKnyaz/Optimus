using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WebBrowser.ResourceProviders;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture]
	public class EngineJsTests
	{
		private List<object> _log;
		private Engine _engine;

		[SetUp]
		public void SetUp()
		{
			_log =new List<object>();
			_engine = new Engine();
			_engine.Console.OnLog += _log.Add;
		}

		private Engine CreateEngineWithScript(string js)
		{
			_engine.Load("<html><head><script>"+js+"</script></head></html>");
			return _engine;
		}

		private Engine CreateEngine(string body, string js){
			_engine.Load("<html><head><script defer>" + js + "</script></head><body>"+body+"</body></html>");
			return _engine;
		}

		[Test]
		public void StoreValueInElement()
		{
			var engine = CreateEngine("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
				@"var e = document.getElementById('d');
e.someVal = 'x';
console.log(e.someVal == 'x');
var e2 = document.getElementById('d');
console.log(e2.someVal == 'x');");
			
			CollectionAssert.AreEqual(new object[] { true, true}, _log);
		}

		[Test]
		public void PassElementValueToClr()
		{
			var engine = CreateEngine("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
				@"var e = document.getElementById('d');
e.someVal = 'x';
console.log(e.someVal);");

			CollectionAssert.AreEqual(new object[] { "x"}, _log);
		}

		[Test]
		public void SetInnerHtml()
		{
			var engine = CreateEngine("<div id='d'></div>",
				@"var e = document.getElementById('d');
console.log(e.hasChildNodes);
e.innerHTML = '<h1>1</h1><h2>2</h2><h3>3</h3>';
console.log(e.hasChildNodes);");
			CollectionAssert.AreEqual(new object[] { false, true }, _log);
		}

		[Test]
		public void NodeTest()
		{
			var engine = CreateEngine("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
@"var e = document.getElementById('d');
console.log(e != null);
console.log(e == document.getElementById('d'));
console.log(e.tagName);
console.log(e.firstChild != null);
console.log(e.firstChild.tagName);
console.log(e.lastChild != null);
console.log(e.lastChild.tagName);
console.log(e.parentNode != null);
console.log(e.parentNode.tagName);
console.log(e.ownerDocument == document);
console.log(e.getAttribute('id'));");

			CollectionAssert.AreEqual(new object[] { true, true, "div", true, "h1", true, "h3", true, "body", true, "d" }, _log);
		}

		[Test]
		public void NodeAddEventListenerTest()
		{
			var engine = CreateEngine("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
				@"var e = document.getElementById('d');
console.log(e.addEventListener != null);
console.log(e.removeEventListener != null);
console.log(e.dispatchEvent != null);
e.addEventListener('click', function(){console.log('click');}, true);
var ev = document.createEvent('Event');
ev.initEvent('click', false, false);
e.dispatchEvent(ev);");

			CollectionAssert.AreEqual(new object[] { true, true, true, "click" }, _log);
		}

		[Test]
		public void EventSubscribeTests()
		{
			var engine = CreateEngine("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
				@"var e = document.getElementById('d');
var handler = function(){console.log('click');};
e.onclick = handler;
console.log(e.onclick == handler);
var ev = document.createEvent('Event');
ev.initEvent('click', false, false);
e.dispatchEvent(ev);
e.onclick = function(){console.log('click2');};
e.click();");

			CollectionAssert.AreEqual(new object[] { true, "click", "click2" }, _log);
		}

		[Test]
		public void CreateDivTest()
		{
			var engine = CreateEngineWithScript(
@"var div = document.createElement('div');
console.log(div != null);
console.log(div.ownerDocument == document);
console.log(div.tagName);
console.log(div.parentNode == null);
console.log(div.appendChild != null);");

			CollectionAssert.AreEqual(new object[]{true, true, "div", true, true}, _log);
		}

		[Test]
		public void Document()
		{
			var engine = CreateEngineWithScript(
@"console.log(document == document);
console.log(document != null);
console.log(document.hasOwnProperty('ownerDocument'));
console.log(document.ownerDocument === null);
console.log(document.parentNode === null);
console.log(document.documentElement != null);
console.log(document.appendChild != null);");

			CollectionAssert.AreEqual(new object[] { true, true, true, true, true, true, true}, _log);
		}

		[Test]
		public void DocumentElement()
		{
			var engine = CreateEngineWithScript(
@"var e = document.documentElement;
console.log(e == e);
console.log(e.ownerDocument === document);
console.log(e.parentNode === document);
console.log(e.removeChild != null);
console.log(e.appendChild != null);");

			CollectionAssert.AreEqual(new object[] { true, true, true, true, true }, _log);
		}

		[Test]
		public void GetElementsByTagName()
		{
			var engine = CreateEngine("<div id='d'></div><div></div><span></span>",
				@"var elems = document.body.getElementsByTagName('div');
console.log(elems.length);");

			CollectionAssert.AreEqual(new object[] { 2 }, _log);
		}

		[Test]
		public void ChildNodes()
		{
			var engine = CreateEngine("<div id='d'></div><div></div><span></span>",
				@"var elems = document.body.childNodes;
console.log(elems.length);
console.log(elems[0] != null);");

			CollectionAssert.AreEqual(new object[] { 3, true }, _log);
		}

		[Test]
		public void AddScriptAndExecute()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost/module", "console.log('hi from module');");
			var engine = new Engine(resourceProvider);
			var log = new List<string>();
			engine.Console.OnLog += o =>
			{
				System.Console.WriteLine(o ?? "<null>");
				log.Add(o.ToString());
			};

			var script = 
@"var s = document.createElement('script');
s.onload = function(){console.log('load');};
s.setAttribute('async','true');
s.setAttribute('src', 'http://localhost/module');
document.head.appendChild(s);";

			engine.Load("<html><head><script>" + script + "</script></head><body><div id='uca'></div></body></html>");

			Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("http://localhost/module"), Times.Once());
			Assert.AreEqual(2, log.Count);
			Assert.AreEqual("load", log[0]);
			Assert.AreEqual("hi from module", log[1]);
		}

		[Test]
		public void Navigator()
		{
			var engine = CreateEngineWithScript(@"
console.log(navigator != null);
console.log(navigator.userAgent);");

			CollectionAssert.AreEqual(new object[] { true, "Optimus" }, _log);
		}

		[Test]
		public void SetChildNode()
		{
			var engine = CreateEngine("<div id='a'><span></span></div>",
@"var d = document.getElementById('a');
d.childNodes[0] = document.createElement('p');
console.log(d.childNodes[0].tagName);");

			CollectionAssert.AreEqual(new object[] { "p" }, _log);
		}

		[Test]
		public void StyleRead()
		{
			var engine = CreateEngine("<span id='content1' style='width:100pt; heigth:100pt'></span>",
@"var style = document.getElementById('content1').style;
console.log(style.getPropertyValue('width'));
console.log(style[0]);
console.log(style['width']);");

			CollectionAssert.AreEqual(new[] { "100pt", "width", "100pt" }, _log);
		}

		[Test]
		public void StyleWrite()
		{
			var engine = CreateEngine("<span id='content1' style='width:100pt; heigth:100pt'></span>",
@"var style = document.getElementById('content1').style;
style['width'] = '200pt';
console.log(style['width']);");

			CollectionAssert.AreEqual(new[] { "200pt" }, _log);
		}

		[Test]
		public void Location()
		{
			var resourceProvider = Mocks.ResourceProvider("http://todosoft.org", 
				Mocks.Page("console.log(window.location.href);console.log(window.location.protocol);"));
			var engine = new Engine(resourceProvider);
			engine.Console.OnLog+= x => _log.Add(x.ToString());
			engine.OpenUrl("http://todosoft.org");
			CollectionAssert.AreEqual(new[] { "http://todosoft.org", "http:" }, _log);
		}

		[Test]
		public void SetLocationHref()
		{
			var resourceProvider = Mocks.ResourceProvider("http://todosoft.org",
				Mocks.Page("window.location.href = 'http://todosoft.org/sub';"))
				.Resource("http://todosoft.org/sub", Mocks.Page("console.log(window.location.href);console.log(window.location.protocol);"));
			var engine = new Engine(resourceProvider);
			engine.OpenUrl("http://todosoft.org");

			Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("http://todosoft.org"), Times.Once());
			Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("http://todosoft.org/sub"), Times.Once());

			Assert.AreEqual("http://todosoft.org/sub", engine.Window.Location.Href);
		}

		[Test]
		public void Ajax()
		{
			var httpResourceProvider = Mock.Of<IHttpResourceProvider>(x => x.SendRequest(It.IsAny<HttpRequest>()) ==
				new HttpResponse(HttpStatusCode.OK, "hello".ToStream(), null));

			var resourceProvider = Mock.Of<IResourceProvider>(x => x.HttpResourceProvider == httpResourceProvider);

			var engine = new Engine(resourceProvider);
			var log = new List<string>();
			engine.Console.OnLog += o => log.Add(o == null ? "<null>" : o.ToString());
			engine.Load(Mocks.Page(
@"var client = new XMLHttpRequest();
client.onreadystatechange = function () {
  console.log(this.readyState);
  if(this.readyState == this.DONE) {
		console.log(this.status);
    if(this.status == 200 ) {
		console.log(this.responseXML);
		console.log(this.responseText);
    }
  }
};
client.open(""GET"", ""http://localhost/unicorn.xml"", false);
client.send();"));
			Mock.Get(httpResourceProvider).Verify(x => x.SendRequest(It.IsAny<HttpRequest>()), Times.Once());
			CollectionAssert.AreEqual(new object[]{"4", "200", "hello", "hello"}, log);
		}
	}
}
