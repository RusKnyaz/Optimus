#if NUNIT
using System.Linq;
using NUnit.Framework;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;
using WebBrowser.Properties;
using WebBrowser.TestingTools;
using Text = WebBrowser.Dom.Text;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture]
	public class EngineWithKnockoutTests
	{
		
		Document Load(string html)
		{
			var engine = new Engine();
			engine.Console.OnLog += o => System.Console.WriteLine(o.ToString());
			engine.Load(html);
			return engine.Document;
		}

		[Test]
		public void KnockoutInclude()
		{
			Load("<html><head><script> " + Resources.knockout + " </script></head><body></body></html>");
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

			var document = Load("<html><head><script> " + Resources.knockout + " </script><script>" + vm + "</script></head><body><span id = 'c1' data-bind='text:Greeting'/></body></html>");

			var span = document.GetElementById("c1");
			Assert.IsNotNull(span);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Text)span.FirstChild).Data);
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

			var doc = Load("<html><head><script> " + Resources.knockout + " </script><script>" + vm + "</script></head><body><span id = 'c1' data-bind='text:Greeting, click: Click'/></body></html>");

			var span = (HtmlElement)doc.GetElementById("c1");
			Assert.IsNotNull(span);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Text)span.FirstChild).Data);
			Assert.AreEqual("Hello", span.InnerHtml);

			var evt = doc.CreateEvent("Event");
			evt.InitEvent("click", false, false);
			span.DispatchEvent(evt);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("World", ((Text)span.FirstChild).Data);
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

			var doc = Load("<html><head><script> " + Resources.knockout + " </script><script>" + vm + "</script></head>"+
				"<body>" +
				"<input type='text' data-bind='value:Name' id='in'/>" +
				"<span id = 'c1' data-bind='text:Greeting'/>" +
				"</body></html>");

			var span = (HtmlElement)doc.GetElementById("c1");
			Assert.IsNotNull(span);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello, World", ((Text)span.FirstChild).Data);

			var input = (HtmlInputElement) doc.GetElementById("in");

			input.EnterText("Lord");

			Assert.AreEqual("Hello, Lord", ((Text)span.FirstChild).Data);
		}

		[Test]
		public void KnockoutInputCheckbox()
		{
			var vm =
@"function VM() {
	var _this = this;	
	this.Checked = ko.observable(true);
	this.Click = function(){_this.Checked(!_this.Checked());};
}
ko.applyBindings(new VM());";

			var doc = Load("<html><head><script> " + Resources.knockout + " </script><script>" + vm + "</script></head>" +
				"<body>" +
				"<input type='checkbox' data-bind='checked:Checked' id='in'/>" +
				"<div id = 'button' data-bind='click:Click'>Click me</div>" +
				"</body></html>");

			var div = (HtmlElement)doc.Body.GetElementsByTagName("div").First();
			var checkbox = (HtmlInputElement) doc.Body.GetElementsByTagName("input").First();
			Assert.IsNotNull(checkbox);
			Assert.IsNotNull(div);
			Assert.IsTrue(checkbox.Checked);
			div.Click();
			Assert.IsFalse(checkbox.Checked);
		}

		[Test]
		public void ForeachBinding()
		{
			var doc = Load("<html><head><script> " + Resources.knockout + " </script>" +
@"<script>
function VM(peoples) {
	var _this = this;	
	this.Peoples = ko.observableArray(peoples);
	this.Click = function(){_this.Peoples.push({Name:'Neo'});};
}
ko.applyBindings(new VM([{Name:'Ivan'},{Name:'Vasil'}]));
</script>
</head>
<body>
	<!-- ko foreach: Peoples -->
		<span data-bind='text:Name'></span>
	<!-- /ko -->
	<input type='button' id = 'button' data-bind='click:Click' value='Click me'/>
</body>
</html>");

			var button = (HtmlInputElement)doc.Body.GetElementsByTagName("input").First();
			var spans = doc.Body.GetElementsByTagName("span").ToArray();
			Assert.AreEqual(2, spans.Length);
			Assert.AreEqual("Ivan", spans[0].InnerHtml);
			Assert.AreEqual("Vasil", spans[1].InnerHtml);

			button.Click();

			var newSpans = doc.Body.GetElementsByTagName("span").ToArray();
			Assert.AreEqual(3, newSpans.Length);
			Assert.AreEqual("Ivan", newSpans[0].InnerHtml);
			Assert.AreEqual("Vasil", newSpans[1].InnerHtml);
			Assert.AreEqual("Neo", newSpans[2].InnerHtml);
		}

		[Test]
		public void Template()
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

			var doc = Load("<html><head><script> " + Resources.knockout + " </script><script>" + vm + "</script>" +
@"<script type='text/html' id='tmpl'>
<span id = 'c1' data-bind='text:Greeting, click: Click'/>
</script>
</head><body><div data-bind='template:""tmpl""'></div></body></html>");

			var span = (HtmlElement)doc.GetElementById("c1");
			Assert.IsNotNull(span);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Text)span.FirstChild).Data);
			Assert.AreEqual("Hello", span.InnerHtml);

			var evt = doc.CreateEvent("Event");
			evt.InitEvent("click", false, false);
			span.DispatchEvent(evt);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("World", ((Text)span.FirstChild).Data);
			Assert.AreEqual("World", span.InnerHtml);

			span.Click();
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Text)span.FirstChild).Data);
			Assert.AreEqual("Hello", span.InnerHtml);
		}

		[Test]
		public void TemplateInsideForeachBinding()
		{
			var doc = Load("<html><head><script> " + Resources.knockout + " </script>" +
@"<script>
function VM(peoples) {
	var _this = this;	
	this.Peoples = ko.observableArray(peoples);
	this.Click = function(){_this.Peoples.push({Name:'Neo'});};
}
ko.applyBindings(new VM([{Name:'Ivan'},{Name:'Vasil'}]));
</script>
<script type='text/html' id='itemTemplate'>
	<span data-bind='text:Name'></span>
</script>
</head>
<body>
	<!-- ko foreach: Peoples -->
	<div data-bind=""template:'itemTemplate'""></div>
	<!-- /ko -->
	<input type='button' id = 'button' data-bind='click:Click' value='Click me'/>
</body>
</html>");

			var button = (HtmlInputElement)doc.Body.GetElementsByTagName("input").First();
			var divs = doc.Body.GetElementsByTagName("div").ToArray();
			Assert.AreEqual(2, divs.Length);

			button.Click();

			var newDivs = doc.Body.GetElementsByTagName("div").ToArray();
			Assert.AreEqual(3, newDivs.Length);

		}
	}
}
#endif