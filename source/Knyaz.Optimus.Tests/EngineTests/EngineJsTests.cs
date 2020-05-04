using System.Linq;
using System.Threading;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.TestingTools;
using Moq;
using NUnit.Framework;
using System.Text;
using System.Threading.Tasks;
using Knyaz.Optimus.Configure;
using Knyaz.Optimus.Tests.TestingTools;
using Knyaz.Optimus.ScriptExecuting.Jint;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public static class EngineJsTests
	{
		private static Engine CreateEngineWithScript(string js)
		{
			return Load("<html><head><script>" + js + "</script></head></html>");
		}

		private static Engine Load(IResourceProvider resourceProvider)
		{
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			engine.OpenUrl("http://localhost").Wait();
			return engine;
		}
		
		private static Engine Load(string html) => Load(Mock.Of<IResourceProvider>()
			.Resource("http://localhost", html));

		private static Engine Load(string body, string js)
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost", 
				"<html>"+(js != null ? "<head><script src='test.js' defer/></head>" : "")+"<body>" + body + "</body></html>");
			
			if(js != null)
				resourceProvider = resourceProvider.Resource("http://localhost/test.js", js);

			return Load(resourceProvider);
		}

		[Test]
		public static void StoreValueInElement()
		{
			var engine = Load("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
				@"var e = document.getElementById('d');
e.someVal = 'x';
console.log(e.someVal == 'x');
var e2 = document.getElementById('d');
console.log(e2.someVal == 'x');");

			CollectionAssert.AreEqual(new object[] {true, true}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void PassElementValueToClr()
		{
			var engine = Load("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
				@"var e = document.getElementById('d');
e.someVal = 'x';
console.log(e.someVal);");

			CollectionAssert.AreEqual(new object[] {"x"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void SetInnerHtml()
		{
			var engine = Load("<div id='d'></div>",
				@"var e = document.getElementById('d');
console.log(e.hasChildNodes);
e.innerHTML = '<h1>1</h1><h2>2</h2><h3>3</h3>';
console.log(e.hasChildNodes);");
			CollectionAssert.AreEqual(new object[] {false, true}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void NodeTest()
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

			CollectionAssert.AreEqual(new object[] {true, true, "DIV", true, "H1", true, "H3", true, "BODY", true, "d"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void NodeAddEventListenerTest()
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

			CollectionAssert.AreEqual(new object[] {true, true, true, "click"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void EventSubscribeTests()
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

			CollectionAssert.AreEqual(new object[] {true, "click", "click2"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void CreateDivTest()
		{
			var engine = CreateEngineWithScript(
				@"var div = document.createElement('div');
console.log(div != null);
console.log(div.ownerDocument == document);
console.log(div.tagName);
console.log(div.parentNode == null);
console.log(div.appendChild != null);");

			CollectionAssert.AreEqual(new object[] {true, true, "DIV", true, true}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		
		[Test]
		public static void DocumentElement()
		{
			var engine = CreateEngineWithScript(
				@"var e = document.documentElement;
console.log(e == e);
console.log(e.ownerDocument === document);
console.log(e.parentNode === document);
console.log(e.removeChild != null);
console.log(e.appendChild != null);");

			CollectionAssert.AreEqual(new object[] {true, true, true, true, true}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void GetElementsByTagName()
		{
			var engine = Load("<div id='d'></div><div></div><span></span>",
				@"var elems = document.body.getElementsByTagName('div');
console.log(elems.length);");

			CollectionAssert.AreEqual(new object[] {2}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[TestCase("document.body.childNodes.length", 3)]
		[TestCase("document.body.childNodes[0] != null", true)]
		public static void ChildNodes(string code, object expectedResult)
		{
			var engine = Load("<div id='d'></div><div></div><span></span>", $"console.log({code});");
			
			CollectionAssert.AreEqual(new object[] {expectedResult}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static async Task AddScriptAndExecute()
		{
			var script =
				@"var s = document.createElement('script');
s.onload = function(){console.log('load');};
s.setAttribute('async','true');
s.setAttribute('src', 'http://localhost/module');
document.head.appendChild(s);";
			var resourceProvider = Mocks.ResourceProvider("http://localhost/module", "console.log('hi from module');")
				.Resource("http://localhost", "<html><head><script>" + script + "</script></head><body><div id='uca'></div></body></html>");
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			await engine.OpenUrl("http://localhost");

			Thread.Sleep(1000);
			Assert.AreEqual(2, console.LogHistory.Count);
			Assert.AreEqual("load", console.LogHistory[1]);
			Assert.AreEqual("hi from module", console.LogHistory[0]);
		}

		[Test]
		public static async Task AddScriptModifyingDom()
		{
			var modifyingScript = @"var form = document.body.getElementsByTagName('form')[0];
			             var div = document.createElement('div')
			             div.name = 'generatedDiv';
			             form.appendChild(document.createElement('div'));";

			var accesingScript = @"var div = document.getElementsByName('generatedDiv');
			                     console.log(div != null);"; 
			
			var resourceProvider = Mocks.ResourceProvider("http://localhost", 
				$"<html><body><form><script>{modifyingScript}</script><script>{accesingScript}</script></form></body></html>");
			
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);

			var page = await engine.OpenUrl("http://localhost");

			page.Assert(x => x.Document.Body.GetElementsByTagName("form")[0].ChildNodes.Count == 3);
			
			Assert.AreEqual(console.LogHistory, new[]{true});
		}

		[Test]
		public static async Task AddScriptThatAddsScriptModifyingDom()
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
			
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);

			var page = await engine.OpenUrl("http://localhost");

			page.Assert(x => x.Document.Body.GetElementsByTagName("form")[0].ChildNodes.Count == 4);
			
			Assert.AreEqual(new[]{true}, console.LogHistory);
		}

		[Test]
		public static void Navigator()
		{
			var engine = CreateEngineWithScript(@"
console.log(navigator != null);
console.log(navigator.userAgent);");

			Assert.AreEqual(2, ((TestingConsole)engine.Window.Console).LogHistory.Count);
			Assert.AreEqual(true, ((TestingConsole)engine.Window.Console).LogHistory[0]);
			Assert.IsTrue(((TestingConsole)engine.Window.Console).LogHistory[1].ToString().Contains("Optimus"));
		}

		[Test]
		public static void SetChildNode()
		{
			var engine = Load("<div id='a'><span></span></div>",
				@"var d = document.getElementById('a');
d.childNodes[0] = document.createElement('p');
console.log(d.childNodes[0].tagName);");

			CollectionAssert.AreEqual(new object[] {"P"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		//The test comes from bootstrap library
		[Test]
		public static void StyleOfCustom()
		{
			var engine = Load("<span id='content1' style='width:100pt; heigth:100pt'></span>",
				@"var style = document.createElement('bootstrap').style;
console.log(style ? 'ok' : 'null');");
			CollectionAssert.AreEqual(new[] {"ok"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void StyleRead()
		{
			var engine = Load("<span id='content1' style='width:100pt; heigth:100pt'></span>",
				@"var style = document.getElementById('content1').style;
console.log(style.getPropertyValue('width'));
console.log(style[0]);
console.log(style['width']);");

			CollectionAssert.AreEqual(new[] {"100pt", "width", "100pt"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void StyleWrite()
		{
			var engine = Load("<span id='content1' style='width:100pt; heigth:100pt'></span>",
				@"var style = document.getElementById('content1').style;
style['width'] = '200pt';
console.log(style['width']);");

			CollectionAssert.AreEqual(new[] {"200pt"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void Location()
		{
			var engine = Load("","console.log(window.location.href);console.log(window.location.protocol);");
			CollectionAssert.AreEqual(new[] {"http://localhost/", "http:"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void HistoryExist()
		{
			var engine = Load("", @"console.log(history != null);console.log(window.history != null);");
			CollectionAssert.AreEqual(new[] { true, true }, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void HistoryPushState()
		{
			var engine = Load("",@"window.history.pushState(null, null, 'a.html');");

			Assert.AreEqual("http://localhost/a.html", engine.Uri.AbsoluteUri);
		}

		[Test]
		public static void SetLocationHref()
		{
			var resourceProvider = 
				Mocks.ResourceProvider("http://todosoft.org",Mocks.Page("window.location.href = 'http://todosoft.org/sub';"))
			   .Resource("http://todosoft.org/sub", Mocks.Page("console.log(window.location.href);console.log(window.location.protocol);"));
			
			var engine = TestingEngine.BuildJint(resourceProvider);
			engine.OpenUrl("http://todosoft.org").Wait();

			Thread.Sleep(1000);
//todo:			Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("http://todosoft.org"), Times.Once());
//todo:			Mock.Get(resourceProvider).Verify(x => x.GetResourceAsync("http://todosoft.org/sub"), Times.Once());

			Assert.AreEqual("http://todosoft.org/sub", engine.Window.Location.Href);
		}
		
		[Test]
		public static void SetLocationHrefAndShareCookies()
		{
			var console = new TestingConsole();
			var resourceProvider = Mocks.ResourceProvider("http://todosoft.org",
				Mocks.Page("document.cookie='user=ivan';" +
					       "window.location.href = 'http://todosoft.org/sub';"))
				.Resource("http://todosoft.org/sub", Mocks.Page("console.log(document.cookie);"));

			var engine = TestingEngine.BuildJint(resourceProvider, console);
			engine.OpenUrl("http://todosoft.org").Wait();
			
			Thread.Sleep(5000);

			CollectionAssert.AreEqual(new object[]{"user=ivan"}, console.LogHistory);
		}

		[Test]
		public static void XmlHttpRequestCtor()
		{
			var engine = Load("", @"var xhr = new XMLHttpRequest();
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

			CollectionAssert.AreEqual(new []{0,1,2,3,4,0,1,2,3,4,0}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static async Task XmlHttpRequestSend()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost", Mocks.Page(
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
client.send();"))
				.Resource("http://localhost/unicorn.xml", "hello");

			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			await engine.OpenUrl("http://localhost");

			Thread.Sleep(1000);

			Mock.Get(resourceProvider).Verify(x => x.SendRequestAsync(It.IsAny<Request>()), Times.Exactly(2));
			CollectionAssert.AreEqual(new object[] {1.0d, 4.0d, 200.0d, "hello"}, console.LogHistory);
		}

		[Test]
		public static async Task AjaxExist()
		{
			var resources = Mocks.ResourceProvider(
				"http://localhost",
				Mocks.Page(@"console.log(typeof XMLHttpRequest !== 'undefined');"));
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resources, console);
			await engine.OpenUrl("http://localhost");
			CollectionAssert.AreEqual(new[] {true}, console.LogHistory);
		}

		[Test]
		public static async Task AddEmbeddedScriptInsideEmbedded()
		{
			var resources = Mocks.ResourceProvider(
				"http://localhost",
				Mocks.Page(@"
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
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resources, console);

			await engine.OpenUrl("http://localhost");

			Thread.Sleep(1000);

			CollectionAssert.AreEqual(new[] {"in new script", "ok", "node added", "afterappend"}, console.LogHistory);
		}

		[Test]
		public static async Task AddScriptAsync()
		{
			var resources = Mocks.ResourceProvider(
				"http://localhost",
				Mocks.Page(@"
			document.addEventListener(""DOMNodeInserted"", function(e){
console.log('nodeadded');
}, false);

var d = document.createElement('script');
d.id='aaa';
d.async = true;
d.src = ""http://localhost/script.js"";
d.onload = function(){console.log('onload');};
document.head.appendChild(d);
console.log('afterappend');")).Resource("http://localhost/script.js", "console.log('in new script');");
			
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resources, console);
			
			await engine.OpenUrl("http://localhost");

			Thread.Sleep(1000);
			Assert.AreEqual("nodeadded,afterappend,in new script,onload", 
				string.Join(",", console.LogHistory));
		}

		[Test]
		public static async Task AddScriptOnloadThisAccess()
		{
			var script = @"var script = document.createElement('script');
script.src = 'script.js';
script.someData = 'hello';
script.onload = function(){ console.log(this.someData); document.body.innerHtml='<div id=x></div>';};
			document.head.appendChild(script); ";

			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(
				Mocks.ResourceProvider("http://localhost/script.js", "console.log('in new script');")
					.Resource("http://localhost", Mocks.Page(script)), console);
			var page = await engine.OpenUrl("http://localhost");
			page.Document.WaitId("x", 1000);
			Assert.AreEqual(new[]{"in new script", "hello"}, console.LogHistory);
		}

		[TestCase("document.getElementById('d').attributes['id'].name", "id")]
		[TestCase("document.getElementById('d').attributes[0].name", "id")]
		public static void AttributesTest(string code, string expected)
		{
			var engine = Load("<div id='d'></div>", $"console.log({code});");

			CollectionAssert.AreEqual(new object[] {expected}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test, Description("The sample come from jquery source code")]
		public static void PushApply()
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
			CollectionAssert.AreEqual(new object[] {1}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void ArrayPush()
		{
			var engine = Load("<div></div>", @"var arr = [];
arr.push('x');
console.log(arr.length);");
			CollectionAssert.AreEqual(new object[] {1}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void SliceCall()
		{
			var engine = Load("<div></div>", @"var arr = ['a'];
console.log(arr.slice().length);
console.log([].slice.call(arr).length);");
			CollectionAssert.AreEqual(new object[] {1, 1}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void ChildNodesSlice()
		{
			var engine = Load("<div></div>", @"
console.log(document.body.childNodes.length);
console.log([].slice.call(document.body.childNodes).length);");
			CollectionAssert.AreEqual(new object[] {1, 1}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void ResizeArray()
		{
			var engine = Load("<div></div>", @"var arr = [];
arr.length = 8;
console.log(arr.length);");
			CollectionAssert.AreEqual(new object[] {8}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void ShiftArray()
		{
			var engine = Load("<div></div>", @"var arr = [1,2];
arr.shift();
console.log(arr[0]);");
			CollectionAssert.AreEqual(new object[] {2}, ((TestingConsole)engine.Window.Console).LogHistory);
		}


		[Test]
		public static void DocumentBody()
		{
			var engine = Load("HI", "document.addEventListener('DOMContentLoaded', function(){console.log(document.body ? 'hi' : 'nehi');}, true);");
			CollectionAssert.AreEqual(new[] {"hi"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void GetElementsByClassName()
		{
			var engine = Load("<html><body>" +
			     "<div class='a' id='d1'></div>" +
			     "<div class = 'b' id = 'd2'></div>" +
			     "<div class='a b' id='d3'></div><script>console.log(document.getElementsByClassName('a').length);</script></body></html>");
			
			CollectionAssert.AreEqual(new[] {2}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void GetElementsByClassNameToString()
		{
			var engine = Load("<html><head><script>console.log(document.getElementsByClassName.toString());</script></head><body></body></html>");

			CollectionAssert.AreEqual(new[] {"function getElementsByClassName() { [native code] }"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void SetTimeout()
		{
			var engine = Load("<html><head><script>var x = setTimeout(function(){ console.log('called');});</script></head><body></body></html>");
			Thread.Sleep(1000);
			CollectionAssert.AreEqual(new[] { "called" }, ((TestingConsole)engine.Window.Console).LogHistory);
		}
		[Test]
		public static void ClearTimeout()
		{
			var engine = Load("<html><head><script>var x = setTimeout(function(){ console.log('called');}, 100);clearTimeout(x);</script></head><body></body></html>");
			Thread.Sleep(100);
			CollectionAssert.AreEqual(new object[0], ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void SetInterval()
		{
			var engine = Load("<html><head><script>var x = setInterval(function(){ console.log('called'); clearInterval(x);});</script></head><body></body></html>");
			Thread.Sleep(100);
			CollectionAssert.AreEqual(new[]{"called"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void ClearInterval()
		{
			Load("<html><head><script>var x = setInterval(function(){}, 1000); clearInterval(x);</script></head><body></body></html>");
		}

		[Test]
		public static void ResponseHeadersRegEx()
		{
			var engine = Load(@"<html><head><script>
var rheaders = /^(.*?):[ \t]*([^\r\n]*)$/mg;
var headersString = 'X-AspNetMvc-Version: 4.0\nX-Powered-By: ASP.NET\n\n';
while ( match = rheaders.exec( headersString ) ) { 
console.log(match[1].toLowerCase());
console.log(match[ 2 ]);
}
</script></html>");


			CollectionAssert.AreEqual(new[] {"x-aspnetmvc-version", "4.0", "x-powered-by", "ASP.NET"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		//There is a difference between js and .net regexp - end line detection.
		//To fix the issue, i replaced $ with ($:\r|\n|\r\n) in RegExpConstructor;
		[Test]
		public static void ResponseHeadersRegExBug()
		{
			var engine = Load(@"<html><head><script>
var rheaders = /^(.*?):[ \t]*([^\r\n]*)$/mg;
var headersString = 'X-AspNetMvc-Version: 4.0\r\nX-Powered-By: ASP.NET\r\n\r\n';
while ( match = rheaders.exec( headersString ) ) { 
console.log(match[1].toLowerCase());
console.log(match[ 2 ]);
}
</script></html>");


			CollectionAssert.AreEqual(new[] {"x-aspnetmvc-version", "4.0", "x-powered-by", "ASP.NET"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		//Bug in jint RegExpPrototype.InitReturnValueArray
		//to resolve the issue, i removed 
		// array.DefineOwnProperty("length", new PropertyDescriptor(value: lengthValue, writable: false, enumerable: false, configurable: false), true);
		// in RegExpPrototype.InitReturnValueArray
		[Test]
		public static void ShiftMatchResult()
		{
			var engine = Load("<div></div>",
				@"var match = /quick\s(brown).+?(jumps)/ig.exec('The Quick Brown Fox Jumps Over The Lazy Dog');
match.shift();
console.log(match[0]);");
			CollectionAssert.AreEqual(new object[] {"Brown"}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		
		[Test]
		public static void WindowAddEventListener()
		{
			var engine = Load("<div id=d></div>", @"
var listener = function(){console.log('ok');};
addEventListener('click', listener, true);
var evt = document.createEvent('Event');
evt.initEvent('click', true,true);
dispatchEvent(evt);");

			CollectionAssert.AreEqual(new object[] { "ok" }, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void WindowAddEventListenerNotBoolArg()
		{
			var engine = Load("<div id=d></div>", @"
var listener = function(){console.log('ok');};
addEventListener('click', listener, 1);
var evt = document.createEvent('Event');
evt.initEvent('click', true,true);
dispatchEvent(evt);");

			CollectionAssert.AreEqual(new object[] { "ok" }, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void WindowRemoveEventListener()
		{
			var engine = Load("<div id=d></div>", @"
var listener = function(){console.log('ok');};
addEventListener('click', listener, true);
removeEventListener('click', listener, true);
var evt = document.createEvent('Event');
evt.initEvent('click', true,true);
dispatchEvent(evt);");

			CollectionAssert.AreEqual(new object[0], ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void SelectZeroLength()
		{
			var engine = Load("<select id=s></select>", "console.log(document.getElementById('s').length);");
			CollectionAssert.AreEqual(new []{0.0}, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void SelectLength()
		{
			var engine = Load("<select id=s><option/></select>", "console.log(document.getElementById('s').length);");
			CollectionAssert.AreEqual(new[] { 1.0 }, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[TestCase("document.getElementById('s').options.item(0).id")]
		[TestCase("document.getElementById('s').options[0].id")]
		public static void SelectOptionsItem(string expr)
		{
			var engine = Load("<select id=s><option id=X/></select>", $"console.log({expr});");
			CollectionAssert.AreEqual(new[] { "X" }, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void ApplyToJsFunc()
		{
			var engine = Load("", "function log(x){console.log(x);} log.apply(console, ['asd']);");
			CollectionAssert.AreEqual(new[] { "asd" }, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void ApplyToClrFunc()
		{
			var engine = Load("", "console.log.apply(console, ['asd']);");
			CollectionAssert.AreEqual(new[] { "asd" }, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void Self()
		{
			var engine = Load("", "console.log(self == window);");
			CollectionAssert.AreEqual(new[] { true }, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void OverrideSelf()
		{
			var engine = Load("", "self = 'a'; console.log(self);");
			CollectionAssert.AreEqual(new[] { "a" }, ((TestingConsole)engine.Window.Console).LogHistory);
		}

		[Test]
		public static void GetComputedStyle()
		{
			var console = new TestingConsole();
			var resourceProvider = Mock.Of<IResourceProvider>()
				.Resource("http://localhost", "<html><head><script src='test.js' defer/></head><body><div id=d></div></body></html>")
				.Resource("http://localhost/test.js", "console.log(window.getComputedStyle(document.getElementById('d')).getPropertyValue('display'));" +
				 				                                       "console.log(getComputedStyle(document.getElementById('d')).getPropertyValue('display'));");
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			engine.ComputedStylesEnabled = true;
			engine.OpenUrl("http://localhost").Wait();
			CollectionAssert.AreEqual(new[] { "block", "block" }, console.LogHistory);
		}

		[TestCase("d.style['zoom'] == 1", ExpectedResult = true)]
		[TestCase("d.style['zoom'] === 1", ExpectedResult = true)]
		[TestCase("d.style['zoom']", ExpectedResult = 1)]
		[TestCase("typeof d.style['zoom']", ExpectedResult = "number")]
		[TestCase("d.style['color'] == 1", ExpectedResult = false, Ignore = "Color value to be validated.")]
		[TestCase("d.style['color'] === 1", ExpectedResult = false)]
		[TestCase("typeof d.style['color']", ExpectedResult = "string")]
		
		public static object SetStyleNumericValue(string expression)
		{
			var engine = Load("<div id=d></div>", 
				"var d = document.getElementById('d');" +
				"d.style['zoom'] = 1;" +
				"d.style['color'] = 1;" +
				$"console.log({expression});");
			return ((TestingConsole)engine.Window.Console).LogHistory[0];
		}

		[Test]
		public static void OnLoad()
		{
			var engine = Load("<html><head><script>function OnLoad() { console.log('b'); }</script></head><body onload='OnLoad()'></body></html>");
			
			CollectionAssert.AreEqual(new object[] { "b" }, ((TestingConsole)engine.Window.Console).LogHistory);
		}
		
		[Test]
		public static void WindowOpen()
		{
			var resourceProvider = Mocks.ResourceProvider("http://site.net", Mocks.Page("",
				"<button id=download type=submit onclick=\"window.open('file.txt')\">Download!</button>"))
				.Resource("file.txt", "Hello");

			bool called = false;
			string calledUrl = null;
			string calledName = null;
			string calledOptions = null;
			
			var engine = EngineBuilder.New()
				.UseJint()
				.SetResourceProvider(resourceProvider)
				.Window(w => w.SetWindowOpenHandler((url, name, options) =>
				{
					called = true;
					calledUrl = url;
					calledName = name;
					calledOptions = options;
				}))
				.Build(); 
				
				TestingEngine.BuildJint(resourceProvider);

			engine.OpenUrl("http://site.net").Wait();
			
			engine.ScriptExecutor.Execute("text/javascript", "window.open('file.txt')");

			var button = engine.Document.GetElementById("download") as HtmlElement;
			button.Click();
			
			Assert.IsTrue(called, "window.open have to be called");
			Assert.AreEqual("file.txt", calledUrl, "url");
			Assert.AreEqual(null, calledName, "name");
			Assert.AreEqual(null, calledOptions, "options");
		}

		[Test]
		public static void FormAutoSubmit()
		{
			var resourceProvider = Mocks.ResourceProvider("http://site.net",
				@"<html><body><form method=get onsubmit=""console.log('onsubmit');event.preventDefault();"" id=f action='http://todosoft.ru/test/file.dat'>
				<input name=username/>
				<button type=submit id=b onclick=""console.log('onclick')"">Download!</button>
				</form><script>document.getElementById(""b"").click();</script></body></html>");
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			engine.OpenUrl("http://site.net/").Wait();
			Assert.AreEqual(new[]{"onclick", "onsubmit"}, console.LogHistory);
		}
		
		[Test]
		public static void FormAutoSubmitPrevented()
		{
			var resourceProvider = Mocks.ResourceProvider("http://site.net",
				@"<html><body><form method=get onsubmit=""console.log('onsubmit');event.preventDefault();"" id=f action='http://todosoft.ru/test/file.dat'>
				<input name=username/>
				<button type=submit id=b onclick=""console.log('onclick');event.preventDefault();"">Download!</button>
				</form><script>document.getElementById(""b"").click();</script></body></html>");
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			engine.OpenUrl("http://site.net/").Wait();
			Assert.AreEqual(new[]{"onclick"}, console.LogHistory);
		}

		[Test]
		public static void SubmitGetForm()
		{
			var resources = Mocks.ResourceProvider("http://site.net/",
					"<form method=get action='/login'><input name=username type=text/><input name=password type=password/></form>")
				.Resource("http://site.net/login?username=John&password=123456", "<div id=d></div>");
			
			var engine = TestingEngine.BuildJint(resources);
			engine.OpenUrl("http://site.net/").Wait();

			var doc = engine.Document;

			doc.Get<HtmlInputElement>("[name=username]").First().Value = "John";
			doc.Get<HtmlInputElement>("[name=password]").First().Value = "123456";
			doc.Get<HtmlFormElement>("form").First().Submit();
			Assert.IsNotNull(engine.WaitId("d"));

			doc.Assert(document => document.Location.Href == "http://site.net/login?username=John&password=123456");
		}

		[Test]
		public static void SubmitPostForm()
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/",
					"<form method=post action='login'><input name=username type=text></form>")
				.Resource("http://site.net/login", "<div id=d></div>");

			var engine = TestingEngine.BuildJint(new ResourceProvider(httpResources, null));
			engine.OpenUrl("http://site.net").Wait();

			var doc = engine.Document;

			doc.Get<HtmlInputElement>("[name=username]").First().Value = "John";
			doc.Get<HtmlFormElement>("form").First().Submit();
			Assert.IsNotNull(engine.WaitId("d"));

			var data = 	Encoding.UTF8.GetString(httpResources.History[1].Data);

			Assert.AreEqual("username=John", data);
		}

		[Test]
		public static void SubmitFormUtf8()
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/",
					"<form method=post action='login'><input name=username type=text></form>")
				.Resource("http://site.net/login", "<div id=d></div>");

			var engine = TestingEngine.BuildJint(new ResourceProvider(httpResources, null));
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
		public static void SubmitFormInSubWithParams(string method, string action, string expected)
		{
			//1. initial query should be removed from request on form submit
			//2. Form action query should be ignored.
			var resources = Mocks.ResourceProvider("http://site.net/sub/?var1=x",
					"<form method=" + method + " action='" + action + "'><input name=username type=text/><input name=password type=password/></form>")
				.Resource(expected, "<div id=d></div>");

			var engine = TestingEngine.BuildJint(resources);
			engine.OpenUrl("http://site.net/sub/?var1=x").Wait();
			
			var doc = engine.Document;
			
			doc.Get<HtmlInputElement>("[name=username]").First().Value = "John";
			doc.Get<HtmlInputElement>("[name=password]").First().Value = "123456";
			doc.Get<HtmlFormElement>("form").First().Submit();
			Assert.IsNotNull(engine.WaitId("d"));
			doc.Assert(document => document.Location.Href == expected);
		}

		[Test]
		public static void SubmitNonHtmlResponse()
		{
			var resources = Mocks.ResourceProvider("http://site.net/sub",
					"<form method=get action='download'></form>")
				.Resource("http://site.net/sub/download", "<div id=d></div>", "image/png");
			
			var engine = TestingEngine.BuildJint(resources);
			engine.OpenUrl("http://site.net/sub").Wait();
			
			engine.Document.Get<HtmlFormElement>("form").First().Submit();
			
			engine.Document.Assert(doc => doc.Location.Href == "http://site.net/sub");
		}

		[Test]
		public static void OverrideSubmitActionInButton()
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/",
					"<form action='/login'><button id=b formAction='/logout'></button></form>")
				.Resource("http://site.net/login", "<div id=d>login</div>")
				.Resource("http://site.net/logout", "<div id=d>logout</div>");
			
			var engine = TestingEngine.BuildJint(new ResourceProvider(httpResources, null));
			engine.OpenUrl("http://site.net").Wait();
			
			engine.Document.Get<HtmlButtonElement>("button").First().Click();
			
			Assert.AreEqual("logout", engine.WaitId("d").InnerHTML);
			engine.Document.Assert(doc => doc.Location.Href == "http://site.net/logout");
		}

		
		[Test]
		public static void SetTimeoutArguments()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost",
				"<html><script>" +
				"setTimeout(function(a,b){" +
				"	console.log(a);" +
				"	console.log(b);" +
				"	document.body.innerHTML='<div id=d></div>';" +
				"}, 1, 2, 'x');" +
				"</script></html>");

			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			engine.OpenUrl("http://localhost").Wait();
			Assert.IsNotNull(engine.WaitId("d"));
			Assert.AreEqual(new object[]{2d, "x"}, console.LogHistory);
		}

		[Test]
		public static void SetIntervalArguments()
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

			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			engine.OpenUrl("http://localhost").Wait();
			Assert.IsNotNull(engine.WaitId("d"));
			Assert.AreEqual(new object[]{2d, "x"}, console.LogHistory);
		}

		[Test]
		public static void DomImplementationInstanceIsHidden()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost",
				"<html><script>" +
				"console.log(document.implementation.instance);" +
				"console.log(document.implementation.Instance);" +
				"</script></html>");
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			engine.OpenUrl("http://localhost").Wait();
			Assert.AreEqual(new object[]{null,null}, console.LogHistory);
		}

		[Test]
		public static void AddEventListenerNull()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost",
				"<html><script>" +
				"addEventListener('load', null);" +
				"console.log('no error');" +
				"</script></html>");
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			engine.OpenUrl("http://localhost").Wait();
			Assert.AreEqual(new object[]{"no error"}, console.LogHistory);
		}
		
				
		[Test]
		public static void CustomGlobalFunc()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost",
				"<html><script>" +
				"myfunc = function(msg) {console.log(msg);};" +
				"myfunc('1');" +
				"window.myfunc('2');" +
				"window['myfunc']('3');" +
				"</script></html>");

			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			engine.OpenUrl("http://localhost").Wait();
			Assert.AreEqual(new object[]{"1", "2", "3"}, console.LogHistory);
		}

		[Test]
		public static void NavigatorMimeTypesSmoke()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost",
				"<html><script>" +
				"console.log(navigator.mimeTypes.length);"+
				"console.log(navigator.mimeTypes[\"application/x-shockwave-flash\"]);" +
				"console.log('ok');" +
				"</script></html>");

			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console);
			engine.OpenUrl("http://localhost").Wait();
			Assert.AreEqual(new object[]{0, null, "ok"}, console.LogHistory);
		}

		[Test]
		public static async Task NavigatorPlugins()
		{
			var plugins = new [] {
				new PluginInfo("Pdf reader", "Pdf document reader", "", "",
					new PluginMimeTypeInfo[] {new PluginMimeTypeInfo("application/pdf", "", "pdf")}),
				new PluginInfo("Video plugin", "", "", "", new PluginMimeTypeInfo[]
				{
					new PluginMimeTypeInfo("application/mpeg","","mpg"),
					new PluginMimeTypeInfo("application/avi","","avi"),
				}), 
			};
			
			var resourceProvider = Mocks.ResourceProvider("http://localhost",
				"<html><script>" +
				"console.log(navigator.mimeTypes.length);"+
				"console.log(navigator.plugins.length);" +
				"console.log(navigator.plugins[0].name);"+
				"console.log(navigator.plugins[0].length);"+
				"console.log(navigator.plugins[0][0].type)"+
				"</script></html>");
			
			var console = new TestingConsole();
			var engine = EngineBuilder.New()
				.SetResourceProvider(resourceProvider)
				.UseJint()
				.Window(w => w.SetNavigatorPlugins(plugins).SetConsole(console))
				.Build();

			await engine.OpenUrl("http://localhost");
			
			Assert.AreEqual(new object[]{3, 2, "Pdf reader", 1, "application/pdf"}, console.LogHistory);
		}

		[Test]
		public static async Task PredefineCustomFunction()
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost",
				"<html><script>" +
				"console.log(myFunc(\"a b\"));"+
				"</script></html>");
			
			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(resourceProvider, console); 

			engine.ScriptExecutor.Execute("text/javascript", "function myFunc(str){return encodeURI(str)};");
			
			await engine.OpenUrl("http://localhost", false);
			
			Assert.AreEqual(new[]{"a%20b"}, console.LogHistory);
		}
	}
}
