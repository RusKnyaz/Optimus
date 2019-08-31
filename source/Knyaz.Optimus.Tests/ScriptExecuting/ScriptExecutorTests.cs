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

        private void Execute(IWindowEx window, string code) =>
            new ScriptExecutor(window, _ => null).Execute("text/javascript", code);
    }
}