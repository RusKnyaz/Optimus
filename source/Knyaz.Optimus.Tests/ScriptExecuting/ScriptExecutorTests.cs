using System;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ScriptExecuting;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.ScriptExecuting
{
    [TestFixture]
    public class ScriptExecutorTests
    {
        [TestCase("XMLHttpRequest")]
        [TestCase("Image")]
        [TestCase("Event")]
        public void TypeofGlobalTypes(string type)
        {
            var window = Mock.Of<IWindowEx>();
            var se = new ScriptExecutor(window, null );
            Assert.AreEqual("function", se.Evaluate("text/javascript", "typeof " + type));
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
        public void NewXmlHttpRequest()
        {
            var window = Mock.Of<IWindowEx>();
            var xmlhttp = new XmlHttpRequest(null, () => new object(), null, null);
            var se = new ScriptExecutor(window, func => xmlhttp );
            Assert.AreEqual(true, se.Evaluate("text/javascript", "XMLHttpRequest != null"));
            Assert.AreEqual(xmlhttp, se.Evaluate("text/javascript", "new XMLHttpRequest()"));
        }
        
        [Test]
        public void XmlHttpRequestStaticFields()
        {
            var window = Mock.Of<IWindowEx>();
            var se = new ScriptExecutor(window, func => new XmlHttpRequest(null, null, null, null) );

            Assert.AreEqual(0, se.Evaluate("text/javascript","XMLHttpRequest.UNSENT"));
            Assert.AreEqual(1, se.Evaluate("text/javascript","XMLHttpRequest.OPENED"));
        }

        [Test]
        public void Window()
        {
            var window = Mock.Of<IWindowEx>();
            var se = new ScriptExecutor(window, func => new XmlHttpRequest(null, null, null, null) );
            Assert.AreEqual(true, se.Evaluate("text/javascript","window != null"), "window != null");
            Assert.AreEqual(true, se.Evaluate("text/javascript","window === self"), "window === self");
        }
        
        [Test, Ignore("Bug in Jint")]
        public void AccessArrayWithEmptyString()
        {
            var window = Mock.Of<IWindowEx>();
            var se = new ScriptExecutor(window, func => null);
            var res = se.Evaluate("text/javascript", "(function(){var arr=[];arr[\"\"]=8;return arr[\"\"];})()");
            Assert.AreEqual(8, res);
        }

        [Test]
        public void Globals()
        {
            var console = new Console();
            var navigator = Mock.Of<INavigator>();
            var window = Mock.Of<IWindowEx>(
                x => x.Console == console &&
                     x.Navigator == navigator);
            
            var se = new ScriptExecutor(window, func => new XmlHttpRequest(null, null, null, null) );
            Assert.AreEqual(console, se.Evaluate("text/javascript","console"), "console");
            Assert.AreEqual(navigator, se.Evaluate("text/javascript","navigator"), "navigator");
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
        public void ChildNodesAccessedByIndexer()
        {
            var document = DomImplementation.Instance.CreateHtmlDocument();
            document.Body.InnerHTML = "<div id=parent><div id=child>clickme</div></div>";
            var window = Mock.Of<IWindowEx>(x => x.Document == document);
            Assert.AreEqual(1, Evaluate(window, "document.getElementById('parent').childNodes.length"));
            Assert.AreEqual(1, Evaluate(window, "document.getElementById('parent')['childNodes'].length"));
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

            var se = new ScriptExecutor(window, null);
            
            se.ExecuteJs("var msg='hi';" +
                         "function handler(){window.open(msg);}" +
                         "window.document.body.addEventListener('click', handler);");

            var evt = document.CreateEvent("event");
            evt.InitEvent("click", true, true);
            window.Document.Body.DispatchEvent(evt);
            
            Assert.AreEqual("hi", msg);
            
            se.ExecuteJs("msg='by';" +
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
            var window = Mock.Of<IWindowEx>();
            var se = new ScriptExecutor(window, null);
            
            Assert.AreEqual(expected, se.EvaluateJs(expr));
        }
        
        [Test]
        public void CallEventListener()
        {
            Tuple<string, Action<Event>, bool> args = null;
            object logObject = null;

            var console = new Console();
            console.OnLog += x => logObject = x;
            
            var window = Mock.Of<IWindowEx>(x => x.Console == console);
            Mock.Get(window).Setup(x => x.AddEventListener(It.IsAny<string>(), It.IsAny<Action<Event>>(), It.IsAny<bool>()))
                .Callback<string, Action<Event>, bool>((a1, a2, a3) => { args = new Tuple<string, Action<Event>, bool>(a1,a2,a3);});
            
            Execute(window,  
                @"var listener = function(e){console.log(e);};
                addEventListener('click', listener, true);");

            var evt = new Event("click", new Document());

            Assert.IsNotNull(args);
            args.Item2(evt);
            Assert.AreEqual(evt, logObject);
        }

        [Test]
        public void AccessDom()
        {
            var document = DomImplementation.Instance.CreateHtmlDocument();
            Assert.IsNotNull(document.Body);
            
            var window = Mock.Of<IWindowEx>(x => x.Document == document);
            var se = new ScriptExecutor(window, null);

            Assert.AreEqual(document, se.EvaluateJs("document"), "document");
            Assert.AreEqual(document, se.EvaluateJs("window['document']"), "window['document']'");
            Assert.AreEqual(document, se.EvaluateJs("self['document']"), "self['document']'");
            Assert.AreEqual(document.Body, se.EvaluateJs("document.body"), "document.body");
            Assert.AreEqual(document.Body, se.EvaluateJs("document['body']"), "document['body']");
        }

        [Test]
        public void SaveCustomProperties()
        {
            var document = DomImplementation.Instance.CreateHtmlDocument();
            var window = Mock.Of<IWindowEx>(x => x.Document == document);
            var se = new ScriptExecutor(window, null);

            se.ExecuteJs("document.$customProperty = 'hello';");
            Assert.AreEqual("hello", se.EvaluateJs("document.$customProperty"), "document.$customProperty");
            
            se.ExecuteJs("document.body.$customProperty = 'world'");
            Assert.AreEqual("world", se.EvaluateJs("document.body.$customProperty"), "document.body.$customProperty");
        }
        
        [Test]
        public void ShareValueInHandlerArgument()
        {
            string msg = null;
            var document = DomImplementation.Instance.CreateHtmlDocument();
            document.Body.InnerHTML = "<div id=parent><div id=child></div></div>";
            var window = Mock.Of<IWindowEx>(x => x.Document == document);
            Mock.Get(window).Setup(x => x.Alert(It.IsAny<string>())).Callback<string>(x => msg = x);
            var se = new ScriptExecutor(window, null);
            se.ExecuteJs(@"var parent = document.getElementById('parent');
var child = document.getElementById('child');
child.onclick=function(evt){evt.msg='hello';};
parent.onclick=function(evt){alert(evt.msg);};
var evt = document.createEvent('event');
evt.initEvent('click', true, true);
child.dispatchEvent(evt);");
           
            Assert.AreEqual("hello", msg);
        }

        private object Evaluate(IWindowEx window, string code)=>
            new ScriptExecutor(window, _ => null).Evaluate("text/javascript", code);
        
        private void Execute(IWindowEx window, string code) =>
            new ScriptExecutor(window, _ => null).Execute("text/javascript", code);
    }
    
    static class ScriptExecutorExtensions
    {
        public static object EvaluateJs(this ScriptExecutor se, string code) =>
            se.Evaluate("text/javascript", code);
        
        public static void ExecuteJs(this ScriptExecutor se, string code) =>
            se.Execute("text/javascript", code);
    }
}