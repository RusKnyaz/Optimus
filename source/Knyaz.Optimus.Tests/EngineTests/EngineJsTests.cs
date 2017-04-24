
using System.Collections.Generic;
using System.Threading;
using Knyaz.Optimus.ResourceProviders;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public class EngineJsTests
	{
		private List<object> _log;
		private Engine _engine;
		private IResourceProvider _resourceProvider;

		[SetUp]
		public void SetUp()
		{
			_log = new List<object>();
			_resourceProvider = Mock.Of<IResourceProvider>();
			_engine = new Engine(_resourceProvider);
			_engine.Console.OnLog += _log.Add;
		}

		private Engine CreateEngineWithScript(string js)
		{
			_engine.Load("<html><head><script>" + js + "</script></head></html>");
			return _engine;
		}

		private Engine CreateEngine(string body, string js)
		{
			_resourceProvider.Resource("test.js", js);
			_engine.Load("<html><head><script src='test.js' defer/></head><body>" + body + "</body></html>");
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

			CollectionAssert.AreEqual(new object[] {true, true}, _log);
		}

		[Test]
		public void PassElementValueToClr()
		{
			var engine = CreateEngine("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
				@"var e = document.getElementById('d');
e.someVal = 'x';
console.log(e.someVal);");

			CollectionAssert.AreEqual(new object[] {"x"}, _log);
		}

		[Test]
		public void SetInnerHtml()
		{
			var engine = CreateEngine("<div id='d'></div>",
				@"var e = document.getElementById('d');
console.log(e.hasChildNodes);
e.innerHTML = '<h1>1</h1><h2>2</h2><h3>3</h3>';
console.log(e.hasChildNodes);");
			CollectionAssert.AreEqual(new object[] {false, true}, _log);
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

			CollectionAssert.AreEqual(new object[] {true, true, "DIV", true, "H1", true, "H3", true, "BODY", true, "d"}, _log);
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

			CollectionAssert.AreEqual(new object[] {true, true, true, "click"}, _log);
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

			CollectionAssert.AreEqual(new object[] {true, "click", "click2"}, _log);
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

			CollectionAssert.AreEqual(new object[] {true, true, "DIV", true, true}, _log);
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

			CollectionAssert.AreEqual(new object[] {true, true, true, true, true, true, true}, _log);
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

			CollectionAssert.AreEqual(new object[] {true, true, true, true, true}, _log);
		}

		[Test]
		public void GetElementsByTagName()
		{
			var engine = CreateEngine("<div id='d'></div><div></div><span></span>",
				@"var elems = document.body.getElementsByTagName('div');
console.log(elems.length);");

			CollectionAssert.AreEqual(new object[] {2}, _log);
		}

		[Test]
		public void ChildNodes()
		{
			var engine = CreateEngine("<div id='d'></div><div></div><span></span>",
				@"var elems = document.body.childNodes;
console.log(elems.length);
console.log(elems[0] != null);");

			CollectionAssert.AreEqual(new object[] {3, true}, _log);
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

			Thread.Sleep(1000);
			Assert.AreEqual(2, log.Count);
			Assert.AreEqual("load", log[1]);
			Assert.AreEqual("hi from module", log[0]);
		}

		[Test]
		public void Navigator()
		{
			var engine = CreateEngineWithScript(@"
console.log(navigator != null);
console.log(navigator.userAgent);");

			CollectionAssert.AreEqual(new object[] {true, "Optimus"}, _log);
		}

		[Test]
		public void SetChildNode()
		{
			var engine = CreateEngine("<div id='a'><span></span></div>",
				@"var d = document.getElementById('a');
d.childNodes[0] = document.createElement('p');
console.log(d.childNodes[0].tagName);");

			CollectionAssert.AreEqual(new object[] {"P"}, _log);
		}

		//The test comes from bootstrap library
		[Test]
		public void StyleOfCustom()
		{
			var engine = CreateEngine("<span id='content1' style='width:100pt; heigth:100pt'></span>",
				@"var style = document.createElement('bootstrap').style;
console.log(style ? 'ok' : 'null');");
			CollectionAssert.AreEqual(new[] {"ok"}, _log);
		}

		[Test]
		public void StyleRead()
		{
			var engine = CreateEngine("<span id='content1' style='width:100pt; heigth:100pt'></span>",
				@"var style = document.getElementById('content1').style;
console.log(style.getPropertyValue('width'));
console.log(style[0]);
console.log(style['width']);");

			CollectionAssert.AreEqual(new[] {"100pt", "width", "100pt"}, _log);
		}

		[Test]
		public void StyleWrite()
		{
			var engine = CreateEngine("<span id='content1' style='width:100pt; heigth:100pt'></span>",
				@"var style = document.getElementById('content1').style;
style['width'] = '200pt';
console.log(style['width']);");

			CollectionAssert.AreEqual(new[] {"200pt"}, _log);
		}

		[Test]
		public void Location()
		{
			var resourceProvider = Mocks.ResourceProvider("http://todosoft.org",
				Mocks.Page("console.log(window.location.href);console.log(window.location.protocol);"));
			var engine = new Engine(resourceProvider);
			engine.Console.OnLog += x => _log.Add(x.ToString());
			engine.OpenUrl("http://todosoft.org");
			Thread.Sleep(1000);
			CollectionAssert.AreEqual(new[] {"http://todosoft.org/", "http:"}, _log);
		}

		[Test]
		public void HistoryExist()
		{
			var engine = CreateEngine("", @"console.log(history != null);console.log(window.history != null);");
			engine.Console.OnLog += x => _log.Add(x.ToString());
			
			CollectionAssert.AreEqual(new[] { true, true }, _log);
		}

		[Test]
		public void HistoryPushState()
		{
			var engine = CreateEngine("",@"window.history.pushState(null, null, 'a.html');");

			Assert.AreEqual("http://localhost/a.html", engine.Uri.AbsoluteUri);
		}

		[Test]
		public void SetLocationHref()
		{
			var resourceProvider = Mocks.ResourceProvider("http://todosoft.org",
				Mocks.Page("window.location.href = 'http://todosoft.org/sub';"))
			                            .Resource("http://todosoft.org/sub",
				                            Mocks.Page("console.log(window.location.href);console.log(window.location.protocol);"));
			var engine = new Engine(resourceProvider);
			engine.OpenUrl("http://todosoft.org");

			Thread.Sleep(1000);
//todo:			Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("http://todosoft.org"), Times.Once());
//todo:			Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("http://todosoft.org/sub"), Times.Once());

			Assert.AreEqual("http://todosoft.org/sub", engine.Window.Location.Href);
		}

		[Test]
		public void XmlHttpRequestCtor()
		{
			CreateEngine("", @"var xhr = new XMLHttpRequest();
console.log(xhr.UNSENT);
console.log(xhr.OPENED);
console.log(xhr.HEADERS_RECEIVED);
console.log(xhr.LOADING);
console.log(xhr.DONE);
console.log(XMLHttpRequest.UNSENT);
console.log(XMLHttpRequest.OPENED);
console.log(XMLHttpRequest.HEADERS_RECEIVED);
console.log(XMLHttpRequest.LOADING);
console.log(XMLHttpRequest.DONE);
console.log(xhr.readyState);");

			CollectionAssert.AreEqual(new []{0,1,2,3,4,0,1,2,3,4,0}, _log);
		}

		[Test]
		public void Ajax()
		{
			var resourceProvider = Mock.Of<IResourceProvider>();
			resourceProvider.Resource("http://localhost/unicorn.xml", "hello");

			var engine = new Engine(resourceProvider);
			var log = new List<string>();
			engine.Console.OnLog += o =>
				{
					log.Add(o == null ? "<null>" : o.ToString());
					System.Console.WriteLine(o == null ? "<null>" : o.ToString());
				};
			engine.Load(Mocks.Page(
				@"var client = new XMLHttpRequest();
client.onreadystatechange = function () {
  console.log(this.readyState);
  if(this.readyState == this.DONE) {
		console.log(this.status);
    if(this.status == 200 ) {
		console.log(this.responseText);
    }
  }
};
client.open(""GET"", ""http://localhost/unicorn.xml"", false);
client.send();"));

			Thread.Sleep(1000);

			Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync(It.IsAny<HttpRequest>()), Times.Once());
			CollectionAssert.AreEqual(new[] {"1", "4", "200", "hello"}, log);
		}

		[Test]
		public void AjaxExist()
		{
			var engine = new Engine();
			var log = new List<string>();
			engine.Console.OnLog += o =>
				{
					log.Add(o == null ? "<null>" : o.ToString());
					System.Console.WriteLine(o == null ? "<null>" : o.ToString());
				};
			engine.Load(Mocks.Page(@"console.log(typeof XMLHttpRequest !== 'undefined');"));

			CollectionAssert.AreEqual(new[] {"True"}, log);
		}

		[Test]
		public void AddEmbeddedScriptInsideEmbedded()
		{
			var engine = new Engine();
			var log = new List<string>();
			engine.Console.OnLog += o =>
				{
					log.Add(o == null ? "<null>" : o.ToString());
					System.Console.WriteLine(o == null ? "<null>" : o.ToString());
				};
			engine.Load(Mocks.Page(@"
			document.addEventListener(""DOMNodeInserted"", function(e){
console.log('node added');
}, false);

var d = document.createElement('script');
d.id='aaa';
d.async = true;
d.innerHTML = ""console.log('in new script');console.log(document.getElementById('aaa') != null ? 'ok' : 'null');"";
d.onload = function(){console.log('onload');};
document.head.appendChild(d);
console.log('afterappend');"));

			Thread.Sleep(1000);

			CollectionAssert.AreEqual(new[] {"in new script", "ok", "node added", "afterappend"}, log);
		}

		[Test]
		public void AddScriptAsync()
		{
			var engine = new Engine(Mocks.ResourceProvider("http://localhost/script.js", "console.log('in new script');"));
			var log = new List<string>();
			engine.Console.OnLog += o =>
				{
					log.Add(o == null ? "<null>" : o.ToString());
					System.Console.WriteLine(o == null ? "<null>" : o.ToString());
				};
			engine.Load(Mocks.Page(@"
			document.addEventListener(""DOMNodeInserted"", function(e){
console.log('nodeadded');
}, false);

var d = document.createElement('script');
d.id='aaa';
d.async = true;
d.src = ""http://localhost/script.js"";
d.onload = function(){console.log('onload');};
document.head.appendChild(d);
console.log('afterappend');"));

			Thread.Sleep(1000);
			CollectionAssert.AreEqual(new[] {"nodeadded", "afterappend", "in new script", "onload"}, log);
		}

		[Test]
		public void InstanceOfHtmlElement()
		{
			var engine = CreateEngine("<div id='d'></div>",
				@"
console.log(document.body instanceof String);
console.log(document.body instanceof Element);
console.log(document.body instanceof HTMLElement);");

			CollectionAssert.AreEqual(new object[] {false, true, true}, _log);
		}

		[Test]
		public void AttributesTest()
		{
			var engine = CreateEngine("<div id='d'></div>",
				@"
console.log(document.getElementById('d').attributes['id'].name);
console.log(document.getElementById('d').attributes[0].name);");

			CollectionAssert.AreEqual(new object[] {"id", "id"}, _log);
		}

		[Test, Description("The sample come from jquer source code")]
		public void PushApply()
		{
			var engine = CreateEngine("<div></div>", @"var arr = [];
var push = arr.push;
var slice = arr.slice;
var preferredDoc = document;
push.apply(
		(arr = slice.call(preferredDoc.childNodes)),
		preferredDoc.childNodes
	);
console.log(arr[ preferredDoc.childNodes.length ].nodeType);");
			CollectionAssert.AreEqual(new object[] {1}, _log);
		}

		[Test]
		public void ArrayPush()
		{
			var engine = CreateEngine("<div></div>", @"var arr = [];
arr.push('x');
console.log(arr.length);");
			CollectionAssert.AreEqual(new object[] {1}, _log);
		}

		[Test]
		public void SliceCall()
		{
			var engine = CreateEngine("<div></div>", @"var arr = ['a'];
console.log(arr.slice().length);
console.log([].slice.call(arr).length);");
			CollectionAssert.AreEqual(new object[] {1, 1}, _log);
		}

		[Test]
		public void ChildNodesSlice()
		{
			var engine = CreateEngine("<div></div>", @"
console.log(document.body.childNodes.length);
console.log([].slice.call(document.body.childNodes).length);");
			CollectionAssert.AreEqual(new object[] {1, 1}, _log);
		}

		[Test]
		public void ResizeArray()
		{
			var engine = CreateEngine("<div></div>", @"var arr = [];
arr.length = 8;
console.log(arr.length);");
			CollectionAssert.AreEqual(new object[] {8}, _log);
		}

		[Test]
		public void ShiftArray()
		{
			var engine = CreateEngine("<div></div>", @"var arr = [1,2];
arr.shift();
console.log(arr[0]);");
			CollectionAssert.AreEqual(new object[] {2}, _log);
		}


		[Test]
		public void DocumentBody()
		{
			_resourceProvider.Resource("test.js",
				"document.addEventListener('DOMContentLoaded', function(){console.log(document.body ? 'hi' : 'nehi');}, true);");
			_engine.Load("<html><head><script src='test.js'/></head><body>HI</body></html>");
			CollectionAssert.AreEqual(new[] {"hi"}, _log);
		}

		[Test]
		public void Splice()
		{
			_resourceProvider.Resource("test.js", "var x = [1,2,3]; x.splice(1,0,4);console.log(x);");
			_engine.Load("<html><head><script src='test.js'/></head><body>HI</body></html>");
			CollectionAssert.AreEqual(new[] {1, 4, 2, 3}, _log[0] as object[]);
		}

		[Test]
		public void GetElementsByClassName()
		{

			_engine.Load("<html><body>" +
				"<div class='a' id='d1'></div>" +
				"<div class = 'b' id = 'd2'></div>" +
				"<div class='a b' id='d3'></div><script>console.log(document.getElementsByClassName('a').length);</script></body></html>");

			CollectionAssert.AreEqual(new[] {2}, _log);
		}

		[Test]
		public void GetElementsByClassNameToString()
		{
			_engine.Load(
				"<html><head><script>console.log(document.getElementsByClassName.toString());</script></head><body></body></html>");

			CollectionAssert.AreEqual(new[] {"function getElementsByClassName() { [native code] }"}, _log);
		}

		[Test]
		public void SetTimeout()
		{
			_engine.Load(
				"<html><head><script>var x = setTimeout(function(){ console.log('called');});</script></head><body></body></html>");
			Thread.Sleep(100);
			CollectionAssert.AreEqual(new[] { "called" }, _log);
		}
		[Test]
		public void ClearTimeout()
		{
			_engine.Load(
				"<html><head><script>var x = setTimeout(function(){ console.log('called');}, 100);clearTimeout(x);</script></head><body></body></html>");
			Thread.Sleep(100);
			CollectionAssert.AreEqual(new object[0], _log);
		}

		[Test]
		public void SetInterval()
		{
			_engine.Load(
				"<html><head><script>var x = setInterval(function(){ console.log('called'); clearInterval(x);});</script></head><body></body></html>");
			Thread.Sleep(100);
			CollectionAssert.AreEqual(new[]{"called"}, _log);
		}

		[Test]
		public void ClearInterval()
		{
			_engine.Load(
				"<html><head><script>var x = setInterval(function(){}, 1000); clearInterval(x);</script></head><body></body></html>");
		}

		[Test]
		public void ResponseHeadersRegEx()
		{
			_engine.Load(@"<html><head><script>
var rheaders = /^(.*?):[ \t]*([^\r\n]*)$/mg;
var headersString = 'X-AspNetMvc-Version: 4.0\nX-Powered-By: ASP.NET\n\n';
while ( match = rheaders.exec( headersString ) ) { 
console.log(match[1].toLowerCase());
console.log(match[ 2 ]);
}
</script></html>");


			CollectionAssert.AreEqual(new[] {"x-aspnetmvc-version", "4.0", "x-powered-by", "ASP.NET"}, _log);
		}

		//There is a difference between js and .net regexp - end line detection.
		//To fix the issue, i replaced $ with ($:\r|\n|\r\n) in RegExpConstructor;
		[Test]
		public void ResponseHeadersRegExBug()
		{
			_engine.Load(@"<html><head><script>
var rheaders = /^(.*?):[ \t]*([^\r\n]*)$/mg;
var headersString = 'X-AspNetMvc-Version: 4.0\r\nX-Powered-By: ASP.NET\r\n\r\n';
while ( match = rheaders.exec( headersString ) ) { 
console.log(match[1].toLowerCase());
console.log(match[ 2 ]);
}
</script></html>");


			CollectionAssert.AreEqual(new[] {"x-aspnetmvc-version", "4.0", "x-powered-by", "ASP.NET"}, _log);
		}

		//Bug in jint RegExpPrototype.InitReturnValueArray
		//to resolve the issue, i removed 
		// array.DefineOwnProperty("length", new PropertyDescriptor(value: lengthValue, writable: false, enumerable: false, configurable: false), true);
		// in RegExpPrototype.InitReturnValueArray
		[Test]
		public void ShiftMatchResult()
		{
			var engine = CreateEngine("<div></div>",
				@"var match = /quick\s(brown).+?(jumps)/ig.exec('The Quick Brown Fox Jumps Over The Lazy Dog');
match.shift();
console.log(match[0]);");
			CollectionAssert.AreEqual(new object[] {"Brown"}, _log);
		}

		[Test]
		public void AlertTest()
		{
			string alert = null;
			_engine.Window.OnAlert += s => { alert = s; };
			CreateEngine("<div></div>", @"alert('HI');");
			Assert.AreEqual("HI", alert);
		}

		[Test]
		public void GlobalFuncEquality()
		{
			CreateEngine("<div></div>", @"console.log(alert == alert);console.log(setInterval == setInterval);");
			CollectionAssert.AreEqual(new object[] {true, true}, _log);
		}

		[Test]
		public void ClrFuncEquality()
		{
			CreateEngine("<div id=d></div>", @"var d = document.getElementById('d');  console.log(d.appendChild == d.appendChild);");
			CollectionAssert.AreEqual(new object[] { true }, _log);
		}

		[Test]
		public void WindowApi()
		{
			CreateEngine("<div></div>", @"console.log(window.setTimeout != null);
console.log(window.clearTimeout != null);
console.log(window.addEventListener != null);
console.log(window.removeEventListener != null);
console.log(window.dispatchEvent != null);
console.log(window.setInterval != null);
console.log(window.clearInterval != null);");

			CollectionAssert.AreEqual(new object[] { true, true, true, true, true, true, true }, _log);
		}

		[Test]
		public void WindowAddEventListener()
		{
			CreateEngine("<div id=d></div>", @"
var listener = function(){console.log('ok');};
addEventListener('click', listener, true);
var evt = document.createEvent('Event');
evt.initEvent('click', true,true);
dispatchEvent(evt);");

			CollectionAssert.AreEqual(new object[] { "ok" }, _log);
		}

		[Test]
		public void WindowAddEventListenerNotBoolArg()
		{
			CreateEngine("<div id=d></div>", @"
var listener = function(){console.log('ok');};
addEventListener('click', listener, 1);
var evt = document.createEvent('Event');
evt.initEvent('click', true,true);
dispatchEvent(evt);");

			CollectionAssert.AreEqual(new object[] { "ok" }, _log);
		}

		[Test]
		public void WindowRemoveEventListener()
		{
			CreateEngine("<div id=d></div>", @"
var listener = function(){console.log('ok');};
addEventListener('click', listener, true);
removeEventListener('click', listener, true);
var evt = document.createEvent('Event');
evt.initEvent('click', true,true);
dispatchEvent(evt);");

			CollectionAssert.AreEqual(new object[0], _log);
		}

		[Test]
		public void SelectZeroLength()
		{
			CreateEngine("<select id=s></select>", "console.log(document.getElementById('s').length);");
			CollectionAssert.AreEqual(new []{0.0}, _log);
		}

		[Test]
		public void SelectLength()
		{
			CreateEngine("<select id=s><option/></select>", "console.log(document.getElementById('s').length);");
			CollectionAssert.AreEqual(new[] { 1.0 }, _log);
		}

		[Test]
		public void SelectOptionsItem()
		{
			CreateEngine("<select id=s><option id=X/></select>", "console.log(document.getElementById('s').options.Item(0).Id);" +
																 "console.log(document.getElementById('s').options[0].Id);");
			CollectionAssert.AreEqual(new[] { "X","X" }, _log);
		}

		[Test]
		public void ApplyToJsFunc()
		{
			CreateEngine("", "function log(x){console.log(x);} log.apply(console, ['asd']);");
			CollectionAssert.AreEqual(new[] { "asd" }, _log);
		}

		[Test]
		public void ApplyToClrFunc()
		{
			CreateEngine("", "console.log.apply(console, ['asd']);");
			CollectionAssert.AreEqual(new[] { "asd" }, _log);
		}

		[Test]
		public void Self()
		{
			CreateEngine("", "console.log(self == window);");
			CollectionAssert.AreEqual(new[] { true }, _log);
		}

		[Test]
		public void OverrideSelf()
		{
			CreateEngine("", "self = 'a'; console.log(self);");
			CollectionAssert.AreEqual(new[] { "a" }, _log);
		}

		[Test]
		public void GetComputedStyle()
		{
			_resourceProvider.Resource("test.js", "console.log(window.getComputedStyle(document.getElementById('d')).getPropertyValue('display'));" +
												  "console.log(getComputedStyle(document.getElementById('d')).getPropertyValue('display'));");
			_engine.ComputedStylesEnabled = true;
			_engine.Load("<html><head><script src='test.js' defer/></head><body>" + "<div id=d></div>" + "</body></html>");
			CollectionAssert.AreEqual(new[] { "block", "block" }, _log);
		}
	}
}
