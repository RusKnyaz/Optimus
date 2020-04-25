using System;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.ScriptExecuting.Jint;
using Knyaz.Optimus.Scripting.Jurassic;
using Knyaz.Optimus.Tests.TestingTools;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.ScriptExecuting
{
    public enum JsEngines { Jint, Jurassic}
    
    
    [TestFixture(JsEngines.Jint)]
    [TestFixture(JsEngines.Jurassic)]
    public class ScriptExecutorTests
    {
        private readonly JsEngines _jsEngine;

        private IJsScriptExecutor CreateExecutor(IWindowEx window)
        {
            switch (_jsEngine)
            {
                case JsEngines.Jint: return JintFactory.Create(new ScriptExecutionContext(window));
                case JsEngines.Jurassic: return JurassicFactory.Create(new ScriptExecutionContext(window));  
                default:throw new Exception("Invalid engine specified: " + _jsEngine);
            }
        }
        
        private object Evaluate(IWindowEx window, string code)=>
            CreateExecutor(window).Evaluate(code);
        
        private void Execute(IWindowEx window, string code) =>
            CreateExecutor(window).Execute(code);
        
        public ScriptExecutorTests(JsEngines jsEngine) { _jsEngine = jsEngine;}
        
        [Test]
        public void EvaluateSimpleExpression()
        {
            var window = Mock.Of<IWindowEx>();
            Assert.AreEqual(10, Evaluate(window, "5+5"));
        }

        [Test]
        public void ConsoleLog()
        {
            var console = new TestingConsole();
            var window = Mock.Of<IWindowEx>(x => x.Console == console);
            Execute(window, "console.log('hello')");
            Assert.AreEqual(new[]{"hello"}, console.LogHistory);
        }

        [Test]
        public void CallAlert()
        {
            var window = Mock.Of<IWindowEx>();
            Mock.Get(window).Setup(x => x.Alert(It.IsAny<string>()));
            Evaluate(window,"alert('hi')");
            Mock.Get(window).Verify(x => x.Alert("hi"), Times.Once());
        }
        
        [TestCase("addEventListener")]
        [TestCase("removeEventListener")]
        [TestCase("setTimeout")]
        [TestCase("setInterval")]
        [TestCase("clearTimeout")]
        [TestCase("clearInterval")]
        public void GlobalFunctions(string func)
        {
            var window = Mock.Of<IWindowEx>();
            Assert.AreEqual(true, Evaluate(window, func+"!== null;"));
        }
        

        [Test]
        public void Window()
        {
            var window = Mock.Of<IWindowEx>();
            var xhr = new XmlHttpRequest(Mock.Of<IResourceProvider>(), null, null, null);
            Mock.Get(window).Setup(x => x.NewXmlHttpRequest()).Returns(xhr);
            Assert.AreEqual(true, Evaluate(window, "window != null"), "window != null");
            Assert.AreEqual(true, Evaluate(window, "window === self"), "window === self");
        }
        
        [Test]
        public void AccessArrayWithEmptyString()
        {
	        if(_jsEngine == JsEngines.Jint)
		        Assert.Ignore("Ignored due to bug in jint");
	        
            var window = Mock.Of<IWindowEx>();
            var res = Evaluate(window, "(function(){var arr=[];arr[\"\"]=8;return arr[\"\"];})()");
            Assert.AreEqual(8, res);
        }

        [Test]
        public void Globals()
        {
            var console = Mock.Of<IConsole>();
            var navigator = Mock.Of<INavigator>();
            var document = DomImplementation.Instance.CreateHtmlDocument();
            
            var window = Mock.Of<IWindowEx>(
                x => x.Console == console &&
                     x.Navigator == navigator &&
                     x.Document == document);
            
            Assert.AreEqual(console, Evaluate(window, "console"), "console");
            Assert.AreEqual(navigator, Evaluate(window, "navigator"), "navigator");
            Assert.AreEqual(document, Evaluate(window, "document"), "document");
        }
        
        [Test]
        public void AddEventListenerWithOptions()
        {
            Tuple<string, Action<Event>, EventListenerOptions> argsAdd = null;
            
            var window = Mock.Of<IWindowEx>();
            Mock.Get(window).Setup(x => x.AddEventListener(It.IsAny<string>(), It.IsAny<Action<Event>>(), It.IsAny<EventListenerOptions>()))
                .Callback<string, Action<Event>, EventListenerOptions>((a1, a2, a3) =>
                {
                    argsAdd = new Tuple<string, Action<Event>, EventListenerOptions>(a1,a2,a3);
                });
            
            Execute(window,  
                @"var listener = function(){console.log('ok');};
                addEventListener('click', listener, {capture:true, passive:true});
                ");

            argsAdd.Assert(add =>
                add.Item1 == "click" &&
                add.Item3.Capture == true &&
                add.Item3.Passive == true);
        }
        
        [Test]
        public void AddEventListenerWithString()
        {
            Tuple<string, Action<Event>, bool> argsAdd = null;
            
            var window = Mock.Of<IWindowEx>();
            Mock.Get(window).Setup(x => x.AddEventListener(It.IsAny<string>(), It.IsAny<Action<Event>>(), It.IsAny<bool>()))
                .Callback<string, Action<Event>, bool>((a1, a2, a3) =>
                {
                    argsAdd = new Tuple<string, Action<Event>, bool>(a1,a2,a3);
                });
            
            Execute(window,  
                @"var listener = function(){console.log('ok');};
                addEventListener('click', listener, 'false');
                ");

            argsAdd.Assert(add =>
                add.Item1 == "click" &&
                add.Item3== true);
        }

        [Test]
        public void SetTimeOutAccessedByIndexer()
        {
            var windowMock = new Mock<IWindowEx>();
            windowMock.Setup(x => x.SetTimeout(It.IsAny<Action<object[]>>(), It.IsAny<double?>(), It.IsAny<object[]>()));
            
            Execute(windowMock.Object, "window.setTimeout(function(){},1);");
            windowMock.Verify(x => x.SetTimeout(It.IsAny<Action<object[]>>(), It.IsAny<double?>(), It.IsAny<object[]>()), Times.Once());
            
            Execute(windowMock.Object, "window['setTimeout'](function(){},1);");
            windowMock.Verify(x => x.SetTimeout(It.IsAny<Action<object[]>>(), It.IsAny<double?>(), It.IsAny<object[]>()), Times.Exactly(2));
        }
        
        [Test]
        public void SetTimeoutWithData()
        {
            var windowMock = new Mock<IWindowEx>();
            
            Execute(windowMock.Object, "setTimeout(function(){}, 300, 'ok');");
            
            windowMock.Verify(x => x.SetTimeout(It.IsAny<Action<object[]>>(), 300, "ok"));
        }

        [Test]
        public void SetTimeoutWithNull()
        {
	        Tuple<Action<object[]>, double?, object[]> args = null; 
            
	        var windowMock = new Mock<IWindowEx>();
	        windowMock.Setup(x => x.SetTimeout(It.IsAny<Action<object[]>>(), It.IsAny<double?>(), It.IsAny<object[]>()))
		        .Callback<Action<object[]>, double?, object[]>( (a1,a2,a3) => args = new Tuple<Action<object[]>, double?, object[]>(a1,a2,a3));
            
	        Execute(windowMock.Object, "setTimeout(null, 300);");
            
	        Assert.IsNotNull(args);
	        Assert.IsNull(args.Item1);
        }

        [Test]
        public void ChildNodesAccessedByIndexer()
        {
            var document = DomImplementation.Instance.CreateHtmlDocument();
            document.Body.InnerHTML = "<div id=parent><div id=child>clickme</div></div>";
            var window = Mock.Of<IWindowEx>(x => x.Document == document);
            Assert.AreEqual(1, Evaluate(window, "document.getElementById('parent').childNodes.length"), "childNodes");
            Assert.AreEqual(1, Evaluate(window, "document.getElementById('parent')['childNodes'].length"), "['childNodes']");
        }
        

        [Test]
        public void AddEventListenerAndRemoveEventListener()
        {
            Tuple<string, Action<Event>, bool> argsAdd = null;
            Tuple<string, Action<Event>, bool> argsRemove = null;
            
            var window = Mock.Of<IWindowEx>();
            Mock.Get(window).Setup(x => x.AddEventListener(It.IsAny<string>(), It.IsAny<Action<Event>>(), It.IsAny<bool>()))
                .Callback<string, Action<Event>, bool>((a1, a2, a3) => { argsAdd = new Tuple<string, Action<Event>, bool>(a1,a2,a3);});
            Mock.Get(window).Setup(x => x.RemoveEventListener(It.IsAny<string>(), It.IsAny<Action<Event>>(), It.IsAny<bool>()))
                .Callback<string, Action<Event>, bool>((a1, a2, a3) => { argsRemove = new Tuple<string, Action<Event>, bool>(a1,a2,a3);});
            
            Execute(window,  
                @"var listener = function(){console.log('ok');};
                addEventListener('click', listener, true);
                removeEventListener('click', listener, true);");
            
            argsAdd.Assert(add => 
                add.Item1 == "click" &&
                add.Item2 != null &&
                add.Item3 == true);
            
            argsRemove.Assert(remove => 
                remove.Item1 == "click" &&
                remove.Item2 != null &&
                remove.Item3 == true);
            
            Assert.AreEqual(argsAdd.Item2, argsRemove.Item2);
        }
        
        [Test]
        public void AddAndRemoveItemEventListener()
        {
            var document = DomImplementation.Instance.CreateHtmlDocument();

            string msg = null;

            var window = Mock.Of<IWindowEx>(w => w.Document == document);
            Mock.Get(window).Setup(w => w.Open(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string>((m, _, __) => msg = m);

            var executor = CreateExecutor(window);
            
            executor.Execute("var msg='hi';" +
                                    "function handler(){window.open(msg);}" +
                                    "window.document.body.addEventListener('click', handler);");

            var evt = document.CreateEvent("event");
            evt.InitEvent("click", true, true);
            window.Document.Body.DispatchEvent(evt);

            Mock.Get(window).Verify(x => x.Open(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("hi", msg);
            
            executor.Execute("msg='by';" +
                                     "window.document.body.removeEventListener('click', handler);");
            
            var evt2 = document.CreateEvent("event");
            evt2.InitEvent("click", true, true);
            window.Document.Body.DispatchEvent(evt2);
            
            Assert.AreEqual("hi", msg);
        }

        [TestCase("document == {}", false)]
        [TestCase("window.document == {}", false)]
        public void CompareUncamparable(string expr, object expected)
        {
            var document = DomImplementation.Instance.CreateHtmlDocument();
            var window = Mock.Of<IWindowEx>(x => x.Document == document);
            Assert.AreEqual(expected, Evaluate(window, expr));
        }
        
        [Test]
        public void CallEventListener()
        {
            Tuple<string, Action<Event>, bool> args = null;
            var console = new TestingConsole();            
            var window = Mock.Of<IWindowEx>(x => x.Console == console);
            Mock.Get(window).Setup(x => x.AddEventListener(It.IsAny<string>(), It.IsAny<Action<Event>>(), It.IsAny<bool>()))
                .Callback<string, Action<Event>, bool>((a1, a2, a3) => { args = new Tuple<string, Action<Event>, bool>(a1,a2,a3);});
            
            Execute(window,  
                @"var listener = function(e){console.log(e);};
                addEventListener('click', listener, true);");

            var evt = new Event("click", new Document());

            Assert.IsNotNull(args);
            args.Item2(evt);
            Assert.AreEqual(new[]{evt}, console.LogHistory);
        }
        
       
        [Test]
        public void AccessDom()
        {
            var document = DomImplementation.Instance.CreateHtmlDocument();
            Assert.IsNotNull(document.Body);
            
            var window = Mock.Of<IWindowEx>(x => x.Document == document);

            Assert.AreEqual(document, Evaluate(window, "document"), "document");
            /*Assert.AreEqual(document, se.EvaluateJs("window['document']"), "window['document']'");
            Assert.AreEqual(document, se.EvaluateJs("self['document']"), "self['document']'");
            Assert.AreEqual(document.Body, se.EvaluateJs("document.body"), "document.body");
            Assert.AreEqual(document.Body, se.EvaluateJs("document['body']"), "document['body']");*/
        }

        [Test]
        public void SaveCustomProperties()
        {
            var document = DomImplementation.Instance.CreateHtmlDocument();
            var window = Mock.Of<IWindowEx>(x => x.Document == document);
            var executor = CreateExecutor(window);

            executor.Execute("document.$customProperty = 'hello';");
            Assert.AreEqual("hello", executor.Evaluate("document.$customProperty"), "document.$customProperty");
            
            executor.Execute("document.body.$customProperty = 'world'");
            Assert.AreEqual("world", executor.Evaluate("document.body.$customProperty"), "document.body.$customProperty");
        }
        
        [Test]
        public void ShareValueInHandlerArgument()
        {
            string msg = null;
            var document = DomImplementation.Instance.CreateHtmlDocument();
            document.Body.InnerHTML = "<div id=parent><div id=child></div></div>";
            var window = Mock.Of<IWindowEx>(x => x.Document == document);
            Mock.Get(window).Setup(x => x.Alert(It.IsAny<string>())).Callback<string>(x => msg = x);
            
            Execute(window, @"var parent = document.getElementById('parent');
var child = document.getElementById('child');
child.onclick=function(evt){evt.msg='hello';};
parent.onclick=function(evt){alert(evt.msg);};
var evt = document.createEvent('event');
evt.initEvent('click', true, true);
child.dispatchEvent(evt);");
           
            Assert.AreEqual("hello", msg);
        }
        
        [TestCase("var handler = function(){}; document.body.onclick = handler; return document.body.onclick == handler")]
        [TestCase("var handler = function(){}; document.body.custom = handler; return document.body.custom == handler")]
        public void GetSetFunction(string expr)
        {
            var document = DomImplementation.Instance.CreateHtmlDocument();
            var window = Mock.Of<IWindowEx>(x => x.Document == document);
            var result = Evaluate(window, "(function(){"+expr+"})()");
            Assert.AreEqual(true, result);
        }

        [TestCase("'alert' in {}", false)]
        [TestCase("Object.getPrototypeOf(document) != null", true)]
        [TestCase("document.write != null", true)]
        [TestCase("Object.getPrototypeOf(document).write != null", true)]
        [TestCase("Object.getPrototypeOf(document).write === document.write", true)]
        [TestCase("'body' in Object.getPrototypeOf(document)", true)]
        [TestCase("document.prototype === undefined", true)]
        
        [TestCase("HTMLElement.prototype != null", true)]
        [TestCase("Element.prototype != null", true)]
        [TestCase("HTMLBodyElement.prototype != null", true)]
        [TestCase("HTMLBodyElement.prototype.toString != null", true)]
        [TestCase("HTMLBodyElement.prototype.toString != null", true)]
        [TestCase("HTMLBodyElement.prototype.addEventListener != null", true)]
        [TestCase("HTMLBodyElement.prototype.prototype", null)]
        [TestCase("HTMLBodyElement.prototype.toString()", "[object HTMLBodyElementPrototype]")]
        [TestCase("Object.getPrototypeOf(document).toString()", "[object HTMLDocumentPrototype]")]
        [TestCase("Object.getPrototypeOf(document.body) != null", true)]
        [TestCase("Object.getPrototypeOf(HTMLBodyElement.prototype).toString()", "[object HTMLElementPrototype]")]
        [TestCase("HTMLBodyElement.prototype == Object.getPrototypeOf(document.body)", true)]
        public void Prototypes(string expression, object expected)
        {
            var document = DomImplementation.Instance.CreateHtmlDocument();
            var xhr = new XmlHttpRequest(Mock.Of<IResourceProvider>(), () => new object(), document, null);
            var window = Mock.Of<IWindowEx>(x => x.Document == document && x.NewXmlHttpRequest() == xhr);
            var result = Evaluate(window, expression);
            Assert.AreEqual(expected, result);
        }
        
        [TestCase("document == document", ExpectedResult = true)]
        [TestCase("document != null", ExpectedResult = true)]
        [TestCase("'ownerDocument' in document", ExpectedResult = true)]
        [TestCase("document.hasOwnProperty('ownerDocument')", ExpectedResult = false)]
        [TestCase("document.ownerDocument === null", ExpectedResult = true)]
        [TestCase("document.parentNode === null", ExpectedResult = true)]
        [TestCase("document.documentElement != null", ExpectedResult = true)]
        [TestCase("document.appendChild != null", ExpectedResult = true)]
        [TestCase("document.appendChild == document.appendChild", ExpectedResult = true)]
        [TestCase("document.body.appendChild == document.body.appendChild", ExpectedResult = true)]
        [TestCase("document.toString()", ExpectedResult = "[object HTMLDocument]")]
        public object Document(string expression)
        {
            var document = DomImplementation.Instance.CreateHtmlDocument();
            var window = Mock.Of<IWindowEx>(x => x.Document == document);
            return Evaluate(window, expression);
        }
        
        [TestCase("document.body instanceof String", ExpectedResult = false)]
        [TestCase("document.body instanceof Element", ExpectedResult = true)]
        [TestCase("document.body instanceof HTMLElement", ExpectedResult = true)]
        [TestCase("document.body instanceof HTMLBodyElement", ExpectedResult = true)]
        public object InstanceOfHtmlElement(string expression)
        {
            var document = DomImplementation.Instance.CreateHtmlDocument();
            var window = Mock.Of<IWindowEx>(x => x.Document == document);
            return Evaluate(window, expression);
        }

        [Test]
        public void Splice()
        {
            var window = Mock.Of<IWindowEx>();
            var result = Evaluate(window, "(function (){var x = [1,2,3]; x.splice(1,0,4);return x;})()");
            Assert.AreEqual(new[] {1, 4, 2, 3}, result);
        }

        [Test]
        public void Comment()
        {
            var result = Evaluate(Mock.Of<IWindowEx>(), @"<!--
    8+8
//-->");
            Assert.AreEqual(16, result);
        }
        
        [TestCase("window.setTimeout != null", ExpectedResult = true)]
        [TestCase("window.clearTimeout != null", ExpectedResult = true)]
        [TestCase("window.addEventListener != null", ExpectedResult = true)]
        [TestCase("window.removeEventListener != null", ExpectedResult = true)]
        [TestCase("window.dispatchEvent != null", ExpectedResult = true)]
        [TestCase("window.setInterval != null", ExpectedResult = true)]
        [TestCase("window.clearInterval != null", ExpectedResult = true)]
        public object WindowApi(string expr) => Evaluate(Mock.Of<IWindowEx>(), expr);
        
        [TestCase("({}===this)", ExpectedResult = false)]
        [TestCase("alert == alert", ExpectedResult = true)]
        [TestCase("setInterval == setInterval", ExpectedResult = true)]
        [TestCase("(function(){var data; return data !== undefined;})()", ExpectedResult = false)]
        public object Misc(string expr) => Evaluate(Mock.Of<IWindowEx>(), expr);

        [Test]
        public void Test()
        {
	        var document = DomImplementation.Instance.CreateHtmlDocument();
	        var window = Mock.Of<IWindowEx>(x => x.Document == document);

	        var result = (Event)Evaluate(window, @"(function(){
var ev = document.createEvent('Event');
ev.initEvent('click', false, false);
return ev;})()");
	        
	        Assert.AreEqual("click", result.Type);
        }

        [Test]
        public void Indexer()
        {
	        var document = DomImplementation.Instance.CreateHtmlDocument();
	        document.Body.Id = "bodyid";
	        var window = Mock.Of<IWindowEx>(x => x.Document == document);
	        var result = Evaluate(window, "document.body.attributes['id'].value");
	        Assert.AreEqual("bodyid", result);
        }
        
        
    }
}