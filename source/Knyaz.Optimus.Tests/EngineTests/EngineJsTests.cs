﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.TestingTools;
using Moq;
using NUnit.Framework;
using System.Text;
using System.Threading.Tasks;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public class EngineJsTests
	{
		private List<object> _log;
		private List<string> _alerts = new List<string>();


		private Engine CreateEngineWithScript(string js)
		{
			return Load("<html><head><script>" + js + "</script></head></html>");
		}

		private Engine Load(IResourceProvider resourceProvider)
		{
			var engine = new Engine(resourceProvider);
			_log = engine.Console.ToList();
			engine.Window.OnAlert += msg => _alerts.Add(msg);
			engine.OpenUrl("http://localhost").Wait();
			return engine;
		}
		
		private Engine Load(string html) => Load(Mock.Of<IResourceProvider>()
			.Resource("http://localhost", html));

		private Engine Load(string body, string js)
		{
			var resourceProvider = Mock.Of<IResourceProvider>()
				.Resource("http://localhost", "<html>"+(js != null ? "<head><script src='test.js' defer/></head>" : "")+"<body>" + body + "</body></html>");
			
			if(js != null)
				resourceProvider = resourceProvider.Resource("http://localhost/test.js", js);

			return Load(resourceProvider);
		}

		[Test]
		public void StoreValueInElement()
		{
			var engine = Load("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
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
			var engine = Load("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
				@"var e = document.getElementById('d');
e.someVal = 'x';
console.log(e.someVal);");

			CollectionAssert.AreEqual(new object[] {"x"}, _log);
		}

		[Test]
		public void SetInnerHtml()
		{
			var engine = Load("<div id='d'></div>",
				@"var e = document.getElementById('d');
console.log(e.hasChildNodes);
e.innerHTML = '<h1>1</h1><h2>2</h2><h3>3</h3>';
console.log(e.hasChildNodes);");
			CollectionAssert.AreEqual(new object[] {false, true}, _log);
		}

		[Test]
		public void NodeTest()
		{
			var engine = Load("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
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
			var engine = Load("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
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
			var engine = Load("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
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
			var engine = Load("<div id='d'></div><div></div><span></span>",
				@"var elems = document.body.getElementsByTagName('div');
console.log(elems.length);");

			CollectionAssert.AreEqual(new object[] {2}, _log);
		}

		[Test]
		public void ChildNodes()
		{
			var engine = Load("<div id='d'></div><div></div><span></span>",
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
			var log = engine.Console.ToList();

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
		public async Task AddScriptModifyingDom()
		{
			var modifyingScript = @"var form = document.body.getElementsByTagName('form')[0];
			             var div = document.createElement('div')
			             div.name = 'generatedDiv';
			             form.appendChild(document.createElement('div'));";

			var accesingScript = @"var div = document.getElementsByName('generatedDiv');
			                     console.log(div != null);"; 
			
			var resourceProvider = Mocks.ResourceProvider("http://localhost", 
				$"<html><body><form><script>{modifyingScript}</script><script>{accesingScript}</script></form></body></html>");
			
			var engine = new Engine(resourceProvider);

			var log = engine.Console.ToList();

			var page = await engine.OpenUrl("http://localhost");

			page.Assert(x => x.Document.Body.GetElementsByTagName("form")[0].ChildNodes.Count == 3);
			
			Assert.AreEqual(log, new[]{true});
		}

		[Test]
		public async Task AddScriptThatAddsScriptModifyingDom()
		{
			var modifyingScript = "var form = document.body.getElementsByTagName('form')[0];" +
			                      " var div = document.createElement('div');" +
			                      " div.name = 'generatedDiv';   " +
			                      "form.appendChild(document.createElement('div'));";
			
			var scriptThatAddsScript = 
				$@"var script = document.createElement('script');
				script.text = ""{modifyingScript}"";
				document.body.getElementsByTagName('form')[0].appendChild(script);";
			
			var accesingScript = @"var div = document.getElementsByName('generatedDiv');
			                     console.log(div != null);";
			
			var resourceProvider = Mocks.ResourceProvider("http://localhost", 
				$"<html><body><form><script id=adding>{scriptThatAddsScript}</script><script id=accessing>{accesingScript}</script></form></body></html>");
			
			var engine = new Engine(resourceProvider);

			var log = engine.Console.ToList();

			var page = await engine.OpenUrl("http://localhost");

			page.Assert(x => x.Document.Body.GetElementsByTagName("form")[0].ChildNodes.Count == 4);
			
			Assert.AreEqual(log, new[]{true});
		}

		[Test]
		public void Navigator()
		{
			var engine = CreateEngineWithScript(@"
console.log(navigator != null);
console.log(navigator.userAgent);");

			Assert.AreEqual(2, _log.Count);
			Assert.AreEqual(true, _log[0]);
			Assert.IsTrue(_log[1].ToString().Contains("Optimus"));
		}

		[Test]
		public void SetChildNode()
		{
			var engine = Load("<div id='a'><span></span></div>",
				@"var d = document.getElementById('a');
d.childNodes[0] = document.createElement('p');
console.log(d.childNodes[0].tagName);");

			CollectionAssert.AreEqual(new object[] {"P"}, _log);
		}

		//The test comes from bootstrap library
		[Test]
		public void StyleOfCustom()
		{
			var engine = Load("<span id='content1' style='width:100pt; heigth:100pt'></span>",
				@"var style = document.createElement('bootstrap').style;
console.log(style ? 'ok' : 'null');");
			CollectionAssert.AreEqual(new[] {"ok"}, _log);
		}

		[Test]
		public void StyleRead()
		{
			var engine = Load("<span id='content1' style='width:100pt; heigth:100pt'></span>",
				@"var style = document.getElementById('content1').style;
console.log(style.getPropertyValue('width'));
console.log(style[0]);
console.log(style['width']);");

			CollectionAssert.AreEqual(new[] {"100pt", "width", "100pt"}, _log);
		}

		[Test]
		public void StyleWrite()
		{
			var engine = Load("<span id='content1' style='width:100pt; heigth:100pt'></span>",
				@"var style = document.getElementById('content1').style;
style['width'] = '200pt';
console.log(style['width']);");

			CollectionAssert.AreEqual(new[] {"200pt"}, _log);
		}

		[Test]
		public void Location()
		{
			Load("","console.log(window.location.href);console.log(window.location.protocol);");
			CollectionAssert.AreEqual(new[] {"http://localhost/", "http:"}, _log);
		}

		[Test]
		public void HistoryExist()
		{
			var engine = Load("", @"console.log(history != null);console.log(window.history != null);");
			engine.Console.OnLog += x => _log.Add(x.ToString());
			
			CollectionAssert.AreEqual(new[] { true, true }, _log);
		}

		[Test]
		public void HistoryPushState()
		{
			var engine = Load("",@"window.history.pushState(null, null, 'a.html');");

			Assert.AreEqual("http://localhost/a.html", engine.Uri.AbsoluteUri);
		}

		[Test]
		public void SetLocationHref()
		{
			var resourceProvider = 
				Mocks.ResourceProvider("http://todosoft.org",Mocks.Page("window.location.href = 'http://todosoft.org/sub';"))
			   .Resource("http://todosoft.org/sub", Mocks.Page("console.log(window.location.href);console.log(window.location.protocol);"));
			
			var engine = new Engine(resourceProvider);
			engine.OpenUrl("http://todosoft.org").Wait();

			Thread.Sleep(1000);
//todo:			Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("http://todosoft.org"), Times.Once());
//todo:			Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("http://todosoft.org/sub"), Times.Once());

			Assert.AreEqual("http://todosoft.org/sub", engine.Window.Location.Href);
		}
		
		[Test]
		public void SetLocationHrefAndShareCookies()
		{
			var resourceProvider = Mocks.ResourceProvider("http://todosoft.org",
				Mocks.Page("document.cookie='user=ivan';" +
					       "window.location.href = 'http://todosoft.org/sub';"))
				.Resource("http://todosoft.org/sub", Mocks.Page("console.log(document.cookie);"));

			var engine = new Engine(resourceProvider);
			var log = engine.Console.ToList();
			engine.OpenUrl("http://todosoft.org").Wait();
			
			Thread.Sleep(5000);

			CollectionAssert.AreEqual(new object[]{"user=ivan"}, log);
		}

		[Test]
		public void XmlHttpRequestCtor()
		{
			Load("", @"var xhr = new XMLHttpRequest();
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
			var resourceProvider = Mock.Of<IResourceProvider>()
				.Resource("http://localhost/unicorn.xml", "hello");

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

			Mock.Get(resourceProvider).Verify(x => x.SendRequestAsync(It.IsAny<Request>()), Times.Once());
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
			var log = engine.Console.ToList();
			
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
			var engine = new Engine(
				Mocks.ResourceProvider("http://localhost/script.js", "console.log('in new script');"));
			var log = engine.Console.ToList();
			
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
			Assert.AreEqual("nodeadded,afterappend,in new script,onload", 
				string.Join(",", log));
		}

		[Test]
		public void AddScriptOnloadThisAccess()
		{
			var script = @"var script = document.createElement('script');
script.src = 'script.js';
script.someData = 'hello';
script.onload = function(){ console.log(this.someData); };
			document.head.appendChild(script); ";

			var engine = new Engine(
				Mocks.ResourceProvider("http://localhost/script.js", "console.log('in new script');")
					.Resource("http://localhost", Mocks.Page(script)));
			var log = engine.Console.ToList();
			engine.OpenUrl("http://localhost").Wait();

			Thread.Sleep(1000);
			Assert.AreEqual("in new script,hello", string.Join(",", log));
		}

		[Test]
		public void InstanceOfHtmlElement()
		{
			var engine = Load("<div id='d'></div>",
				@"
console.log(document.body instanceof String);
console.log(document.body instanceof Element);
console.log(document.body instanceof HTMLElement);");

			CollectionAssert.AreEqual(new object[] {false, true, true}, _log);
		}

		[Test]
		public void AttributesTest()
		{
			var engine = Load("<div id='d'></div>",
				@"
console.log(document.getElementById('d').attributes['id'].name);
console.log(document.getElementById('d').attributes[0].name);");

			CollectionAssert.AreEqual(new object[] {"id", "id"}, _log);
		}

		[Test, Description("The sample come from jquer source code")]
		public void PushApply()
		{
			var engine = Load("<div></div>", @"var arr = [];
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
			var engine = Load("<div></div>", @"var arr = [];
arr.push('x');
console.log(arr.length);");
			CollectionAssert.AreEqual(new object[] {1}, _log);
		}

		[Test]
		public void SliceCall()
		{
			var engine = Load("<div></div>", @"var arr = ['a'];
console.log(arr.slice().length);
console.log([].slice.call(arr).length);");
			CollectionAssert.AreEqual(new object[] {1, 1}, _log);
		}

		[Test]
		public void ChildNodesSlice()
		{
			var engine = Load("<div></div>", @"
console.log(document.body.childNodes.length);
console.log([].slice.call(document.body.childNodes).length);");
			CollectionAssert.AreEqual(new object[] {1, 1}, _log);
		}

		[Test]
		public void ResizeArray()
		{
			var engine = Load("<div></div>", @"var arr = [];
arr.length = 8;
console.log(arr.length);");
			CollectionAssert.AreEqual(new object[] {8}, _log);
		}

		[Test]
		public void ShiftArray()
		{
			var engine = Load("<div></div>", @"var arr = [1,2];
arr.shift();
console.log(arr[0]);");
			CollectionAssert.AreEqual(new object[] {2}, _log);
		}


		[Test]
		public void DocumentBody()
		{
			Load("HI", "document.addEventListener('DOMContentLoaded', function(){console.log(document.body ? 'hi' : 'nehi');}, true);");
			CollectionAssert.AreEqual(new[] {"hi"}, _log);
		}

		[Test]
		public void Splice()
		{
			Load("HI", "var x = [1,2,3]; x.splice(1,0,4);console.log(x);");
			CollectionAssert.AreEqual(new[] {1, 4, 2, 3}, _log[0] as object[]);
		}

		[Test]
		public void GetElementsByClassName()
		{
			Load("<html><body>" +
			     "<div class='a' id='d1'></div>" +
			     "<div class = 'b' id = 'd2'></div>" +
			     "<div class='a b' id='d3'></div><script>console.log(document.getElementsByClassName('a').length);</script></body></html>");
			
			CollectionAssert.AreEqual(new[] {2}, _log);
		}

		[Test]
		public void GetElementsByClassNameToString()
		{
			Load("<html><head><script>console.log(document.getElementsByClassName.toString());</script></head><body></body></html>");

			CollectionAssert.AreEqual(new[] {"function getElementsByClassName() { [native code] }"}, _log);
		}

		[Test]
		public void SetTimeout()
		{
			Load("<html><head><script>var x = setTimeout(function(){ console.log('called');});</script></head><body></body></html>");
			Thread.Sleep(1000);
			CollectionAssert.AreEqual(new[] { "called" }, _log);
		}
		[Test]
		public void ClearTimeout()
		{
			Load("<html><head><script>var x = setTimeout(function(){ console.log('called');}, 100);clearTimeout(x);</script></head><body></body></html>");
			Thread.Sleep(100);
			CollectionAssert.AreEqual(new object[0], _log);
		}

		[Test]
		public void SetInterval()
		{
			Load("<html><head><script>var x = setInterval(function(){ console.log('called'); clearInterval(x);});</script></head><body></body></html>");
			Thread.Sleep(100);
			CollectionAssert.AreEqual(new[]{"called"}, _log);
		}

		[Test]
		public void ClearInterval()
		{
			Load("<html><head><script>var x = setInterval(function(){}, 1000); clearInterval(x);</script></head><body></body></html>");
		}

		[Test]
		public void ResponseHeadersRegEx()
		{
			Load(@"<html><head><script>
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
			Load(@"<html><head><script>
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
			var engine = Load("<div></div>",
				@"var match = /quick\s(brown).+?(jumps)/ig.exec('The Quick Brown Fox Jumps Over The Lazy Dog');
match.shift();
console.log(match[0]);");
			CollectionAssert.AreEqual(new object[] {"Brown"}, _log);
		}

		[Test]
		public void AlertTest()
		{
			Load("<div></div>", @"alert('HI');");
			Assert.AreEqual(new[]{"HI"}, _alerts);
		}

		[Test]
		public void GlobalFuncEquality()
		{
			Load("<div></div>", @"console.log(alert == alert);console.log(setInterval == setInterval);");
			CollectionAssert.AreEqual(new object[] {true, true}, _log);
		}

		[Test]
		public void ClrFuncEquality()
		{
			Load("<div id=d></div>", @"var d = document.getElementById('d');  console.log(d.appendChild == d.appendChild);");
			CollectionAssert.AreEqual(new object[] { true }, _log);
		}

		[Test]
		public void WindowApi()
		{
			Load("<div></div>", @"console.log(window.setTimeout != null);
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
			Load("<div id=d></div>", @"
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
			Load("<div id=d></div>", @"
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
			Load("<div id=d></div>", @"
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
			Load("<select id=s></select>", "console.log(document.getElementById('s').length);");
			CollectionAssert.AreEqual(new []{0.0}, _log);
		}

		[Test]
		public void SelectLength()
		{
			Load("<select id=s><option/></select>", "console.log(document.getElementById('s').length);");
			CollectionAssert.AreEqual(new[] { 1.0 }, _log);
		}

		[Test]
		public void SelectOptionsItem()
		{
			Load("<select id=s><option id=X/></select>", "console.log(document.getElementById('s').options.Item(0).Id);" +
																 "console.log(document.getElementById('s').options[0].Id);");
			CollectionAssert.AreEqual(new[] { "X","X" }, _log);
		}

		[Test]
		public void ApplyToJsFunc()
		{
			Load("", "function log(x){console.log(x);} log.apply(console, ['asd']);");
			CollectionAssert.AreEqual(new[] { "asd" }, _log);
		}

		[Test]
		public void ApplyToClrFunc()
		{
			Load("", "console.log.apply(console, ['asd']);");
			CollectionAssert.AreEqual(new[] { "asd" }, _log);
		}

		[Test]
		public void Self()
		{
			Load("", "console.log(self == window);");
			CollectionAssert.AreEqual(new[] { true }, _log);
		}

		[Test]
		public void OverrideSelf()
		{
			Load("", "self = 'a'; console.log(self);");
			CollectionAssert.AreEqual(new[] { "a" }, _log);
		}

		[Test]
		public void GetComputedStyle()
		{
			var resourceProvider = Mock.Of<IResourceProvider>()
				.Resource("http://localhost", "<html><head><script src='test.js' defer/></head><body><div id=d></div></body></html>")
				.Resource("http://localhost/test.js", "console.log(window.getComputedStyle(document.getElementById('d')).getPropertyValue('display'));" +
				 				                                       "console.log(getComputedStyle(document.getElementById('d')).getPropertyValue('display'));");
			var engine = new Engine(resourceProvider);
			var log = engine.Console.ToList();
			engine.ComputedStylesEnabled = true;
			engine.OpenUrl("http://localhost").Wait();
			CollectionAssert.AreEqual(new[] { "block", "block" }, log);
		}

		[Test]
		public void SetStyleNumericValue()
		{
			Load("<div id=d></div>", 
				"var d = document.getElementById('d');" +
				"d.style['zoom'] = 1;" +
				"console.log(d.style['zoom'] == 1);" +
				"console.log(d.style['zoom'] === 1);" + 
				"console.log(typeof d.style['zoom']);");
			CollectionAssert.AreEqual(new object[] { true, false, "string" }, _log);
		}

		[Test]
		public void UndefinedTest()
		{
			Load("", "var data; if(data !== undefined) {console.log('a');}else{console.log('b');}");
			CollectionAssert.AreEqual(new object[] { "b" }, _log);
		}

		[Test]
		public void OnLoad()
		{
			Load("<html><head><script>function OnLoad() { console.log('b'); }</script></head><body onload='OnLoad()'></body></html>");
			
			CollectionAssert.AreEqual(new object[] { "b" }, _log);
		}

		[Test]
		public void CompareWithThis()
		{
			Load("<div></div>", @"console.log({}===this);");
			CollectionAssert.AreEqual(new object[] {false}, _log);
		}

		[Test]
		public void WindowOpen()
		{
			var resourceProvider = Mocks.ResourceProvider("http://site.net", Mocks.Page("",
				"<button id=download type=submit onclick=\"window.open('file.txt')\">Download!</button>"))
				.Resource("file.txt", "Hello");
			
			var engine = new Engine(resourceProvider);

			engine.OpenUrl("http://site.net").Wait();

			string calledUrl = null;
			string calledName = null;
			string calledOptions = null;
			engine.OnWindowOpen += (url, name, options) =>
			{
				calledUrl = url;
				calledName = name;
				calledOptions = options;
			};

			var button = engine.Document.GetElementById("download") as HtmlElement;
			button.Click();
			
			Assert.AreEqual("file.txt", calledUrl, "url");
			Assert.AreEqual(null, calledName, "name");
			Assert.AreEqual(null, calledOptions, "options");
		}

		[Test]
		public void FormAutoSubmit()
		{
			var resourceProvider = Mocks.ResourceProvider("http://site.net",
				@"<html><body><form method=get onsubmit=""console.log('onsubmit');event.preventDefault();"" id=f action='http://todosoft.ru/test/file.dat'>
				<input name=username/>
				<button type=submit id=b onclick=""console.log('onclick')"">Download!</button>
				</form><script>document.getElementById(""b"").click();</script></body></html>");
			
			var engine = new Engine(resourceProvider);
			var log = engine.Console.ToList();
			engine.OpenUrl("http://site.net/").Wait();
			Assert.AreEqual(new[]{"onclick", "onsubmit"}, log);
		}
		
		[Test]
		public void FormAutoSubmitPrevented()
		{
			var resourceProvider = Mocks.ResourceProvider("http://site.net",
				@"<html><body><form method=get onsubmit=""console.log('onsubmit');event.preventDefault();"" id=f action='http://todosoft.ru/test/file.dat'>
				<input name=username/>
				<button type=submit id=b onclick=""console.log('onclick');event.preventDefault();"">Download!</button>
				</form><script>document.getElementById(""b"").click();</script></body></html>");
			
			var engine = new Engine(resourceProvider);
			var log = engine.Console.ToList();
			engine.OpenUrl("http://site.net/").Wait();
			Assert.AreEqual(new[]{"onclick"}, log);
		}

		[Test]
		public void SubmitGetForm()
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/",
					"<form method=get action='/login'><input name=username type=text/><input name=password type=password/></form>")
				.Resource("http://site.net/login?username=John&password=123456", "<div id=d></div>");
			
			var engine = new Engine(new ResourceProvider(httpResources, null));
			engine.OpenUrl("http://site.net/").Wait();

			var doc = engine.Document;

			doc.Get<HtmlInputElement>("[name=username]").First().Value = "John";
			doc.Get<HtmlInputElement>("[name=password]").First().Value = "123456";
			doc.Get<HtmlFormElement>("form").First().Submit();
			Assert.IsNotNull(engine.WaitId("d"));

			doc.Assert(document => document.Location.Href == "http://site.net/login?username=John&password=123456");
		}

		[Test]
		public void SubmitPostForm()
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/",
					"<form method=post action='login'><input name=username type=text></form>")
				.Resource("http://site.net/login", "<div id=d></div>");

			var engine = new Engine(new ResourceProvider(httpResources, null));
			engine.OpenUrl("http://site.net").Wait();

			var doc = engine.Document;

			doc.Get<HtmlInputElement>("[name=username]").First().Value = "John";
			doc.Get<HtmlFormElement>("form").First().Submit();
			Assert.IsNotNull(engine.WaitId("d"));

			var data = 	Encoding.UTF8.GetString(httpResources.History[1].Data);

			Assert.AreEqual("username=John", data);
		}

		[Test]
		public void SubmitFormUtf8()
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/",
					"<form method=post action='login'><input name=username type=text></form>")
				.Resource("http://site.net/login", "<div id=d></div>");

			var engine = new Engine(new ResourceProvider(httpResources, null));
			engine.OpenUrl("http://site.net").Wait();

			var doc = engine.Document;

			doc.Get<HtmlInputElement>("[name=username]").First().Value = "✓";
			doc.Get<HtmlFormElement>("form").First().Submit();
			Assert.IsNotNull(engine.WaitId("d"));

			var data = Encoding.UTF8.GetString(httpResources.History[1].Data);

			Assert.AreEqual("username=%E2%9C%93", data);
		}

		[TestCase("get", "login?var2=y", "http://site.net/sub/login?username=John&password=123456")]
		[TestCase("get","/login?var2=y", "http://site.net/login?username=John&password=123456")]
		[TestCase("post", "login?var2=y", "http://site.net/sub/login?var2=y")]
		[TestCase("post","/login?var2=y", "http://site.net/login?var2=y")]
		public void SubmitFormInSubWithParams(string method, string action, string expected)
		{
			//1. initial query should be removed from request on form submit
			//2. Form action query should be ignored.
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/sub/?var1=x",
					"<form method=" + method + " action='" + action + "'><input name=username type=text/><input name=password type=password/></form>")
				.Resource(expected, "<div id=d></div>");

			var engine = new Engine(new ResourceProvider(httpResources, null));
			engine.OpenUrl("http://site.net/sub/?var1=x").Wait();
			
			var doc = engine.Document;
			
			doc.Get<HtmlInputElement>("[name=username]").First().Value = "John";
			doc.Get<HtmlInputElement>("[name=password]").First().Value = "123456";
			doc.Get<HtmlFormElement>("form").First().Submit();
			Assert.IsNotNull(engine.WaitId("d"));
			doc.Assert(document => document.Location.Href == expected);
		}

		[Test]
		public void SubmitNonHtmlResponse()
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/sub",
					"<form method=get action='download'></form>")
				.Resource("http://site.net/sub/download", "<div id=d></div>", "image/png");
			
			var engine = new Engine(new ResourceProvider(httpResources, null));
			engine.OpenUrl("http://site.net/sub").Wait();
			
			engine.Document.Get<HtmlFormElement>("form").First().Submit();
			
			engine.Document.Assert(doc => doc.Location.Href == "http://site.net/sub");
		}

		[Test]
		public void OverrideSubmitActionInButton()
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/",
					"<form action='/login'><button id=b formAction='/logout'></button></form>")
				.Resource("http://site.net/login", "<div id=d>login</div>")
				.Resource("http://site.net/logout", "<div id=d>logout</div>");
			
			var engine = new Engine(new ResourceProvider(httpResources, null));
			engine.OpenUrl("http://site.net").Wait();
			
			engine.Document.Get<HtmlButtonElement>("button").First().Click();
			
			Assert.AreEqual("logout", engine.WaitId("d").InnerHTML);
			engine.Document.Assert(doc => doc.Location.Href == "http://site.net/logout");
		}

		[Test]
		public void CancelResponse()
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/sub",
					"<form method=get action='download'></form>");
			
			var engine = new Engine(new ResourceProvider(httpResources, null));
			engine.PreHandleResponse += (sender, arags) => arags.Cancel = true;
			
			engine.OpenUrl("http://site.net/sub").Wait();
			
			Assert.False(engine.Document.Get<HtmlFormElement>("form").Any());
		}

		[Test]
		public void SetTimeoutArguments()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost",
				"<html><script>" +
				"setTimeout(function(a,b){" +
				"	console.log(a);" +
				"	console.log(b);" +
				"	document.body.innerHTML='<div id=d></div>';" +
				"}, 1, 2, 'x');" +
				"</script></html>");

			var engine = new Engine(resourceProvider);
			var log = engine.Console.ToList();
			engine.OpenUrl("http://localhost").Wait();
			Assert.IsNotNull(engine.WaitId("d"));
			Assert.AreEqual(new object[]{2d, "x"}, log);
		}

		[Test]
		public void SetIntervalArguments()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost",
				"<html><script>" +
				"var id = setInterval(function(a,b){" +
				"	console.log(a);" +
				"	console.log(b);" +
				"	document.body.innerHTML='<div id=d></div>';" +
				"	clearInterval(id);" +
				"}, 100, 2, 'x');" +
				"</script></html>");

			var engine = new Engine(resourceProvider);
			var log = engine.Console.ToList();
			engine.OpenUrl("http://localhost").Wait();
			Assert.IsNotNull(engine.WaitId("d"));
			Assert.AreEqual(new object[]{2d, "x"}, log);
		}

		[Test]
		public void DomImplementationInstanceIsHidden()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost",
				"<html><script>" +
				"console.log(document.implementation.instance);" +
				"console.log(document.implementation.Instance);" +
				"</script></html>");

			var engine = new Engine(resourceProvider);
			var log = engine.Console.ToList();
			engine.OpenUrl("http://localhost").Wait();
			Assert.AreEqual(new object[]{null,null}, log);
		}

		[Test]
		public void AddEventListenerNull()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost",
				"<html><script>" +
				"addEventListener('load', null);" +
				"console.log('no error');" +
				"</script></html>");

			var engine = new Engine(resourceProvider);
			var log = engine.Console.ToList();
			engine.OpenUrl("http://localhost").Wait();
			Assert.AreEqual(new object[]{"no error"}, log);
		}
	}
}
