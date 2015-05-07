#if NUNIT
using System;
using System.Text;
using NUnit.Framework;
using WebBrowser.Dom.Elements;
using WebBrowser.Properties;
using WebBrowser.ScriptExecuting;
using WebBrowser.TestingTools;
using Text = WebBrowser.Dom.Text;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture]
	public class EngineWithKnockoutTests
	{
		[Test]
		public void KnockoutInclude()
		{
			var engine = new Engine();
			engine.Load("<html><head><script> " + Resources.knockout + " </script></head><body></body></html>");
		}

		[Test]
		public void KnockoutViewModel()
		{
			var vm =
@"function VM() {
	this.Greeting = ko.observable('Hello');
}
ko.applyBindings(new VM());
";

			var engine = new Engine();
			engine.Load("<html><head><script> " + Resources.knockout + " </script><script>" + vm + "</script></head><body><span id = 'c1' data-bind='text:Greeting'/></body></html>");

			var span = engine.Document.GetElementById("c1");
			Assert.IsNotNull(span);
			Console.Write(engine.Document.DocumentElement.InnerHtml);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Dom.Text)span.FirstChild).Data);
			Assert.AreEqual("Hello", span.InnerHtml);
		}

		[Test]
		public void KnockoutClick()
		{
			var vm =
@"function VM() {
	var _this = this;	
	this.Greeting = ko.observable('Hello');
	this.Click = function(){
		if(_this.Greeting() == 'Hello')
			_this.Greeting('World');
		else
			_this.Greeting('Hello');
	};
}
ko.applyBindings(new VM());
";

			var engine = new Engine();
			engine.Load("<html><head><script> " + Resources.knockout + " </script><script>" + vm + "</script></head><body><span id = 'c1' data-bind='text:Greeting, click: Click'/></body></html>");

			var span = (HtmlElement)engine.Document.GetElementById("c1");
			Assert.IsNotNull(span);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Dom.Text)span.FirstChild).Data);
			Assert.AreEqual("Hello", span.InnerHtml);

			var evt = engine.Document.CreateEvent("click");
			span.DispatchEvent(evt);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("World", ((Dom.Text)span.FirstChild).Data);
			Assert.AreEqual("World", span.InnerHtml);

			span.Click();
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Text)span.FirstChild).Data);
			Assert.AreEqual("Hello", span.InnerHtml);
		}

		[Test]
		public void KnockoutInputAndComputed()
		{
			var vm =
@"function VM() {
	var _this = this;	
	this.Name = ko.observable('World');
	this.Greeting = ko.computed(function(){return 'Hello, ' + _this.Name();});
}
ko.applyBindings(new VM());";

			var engine = new Engine();
			engine.Load("<html><head><script> " + Resources.knockout + " </script><script>" + vm + "</script></head>"+
				"<body>" +
				"<input type='text' data-bind='value:Name' id='in'/>" +
				"<span id = 'c1' data-bind='text:Greeting'/>" +
				"</body></html>");

			var span = (HtmlElement)engine.Document.GetElementById("c1");
			Assert.IsNotNull(span);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello, World", ((Dom.Text)span.FirstChild).Data);

			var input = (HtmlInputElement) engine.Document.GetElementById("in");

			input.EnterText("Lord");

			Assert.AreEqual("Hello, Lord", ((Dom.Text)span.FirstChild).Data);
		}
	}
}
#endif