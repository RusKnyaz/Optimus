using System.Linq;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.Tests.Properties;
using NUnit.Framework;
using Text = Knyaz.Optimus.Dom.Elements.Text;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public class KnockoutTests
	{
		static Document Load(string html)
		{
			var engine = new Engine();
			engine.Console.OnLog += o => System.Console.WriteLine(o.ToString());
			engine.Load(html);
			return engine.Document;
		}

		Document Load(string script, string body)
		{
			var engine = new Engine();
			engine.Console.OnLog += o => System.Console.WriteLine(o.ToString());
			engine.Load("<html><head><script>" + Resources.knockout + "</script></head><body>" + body + "</body><script>" + script + "</script></html>");
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
			var document = Load(@"function VM() {
	this.Greeting = ko.observable('Hello');
}
ko.applyBindings(new VM());
", "<span id = 'c1' data-bind='text:Greeting'/>");

			var span = document.GetElementById("c1");
			Assert.IsNotNull(span);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Text)span.FirstChild).Data);
			Assert.AreEqual("Hello", span.InnerHTML);
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

			var doc = Load("<html><head><script> " + Resources.knockout + " </script></head><body><span id = 'c1' data-bind='text:Greeting, click: Click'/></body>" +
						   "<script>" + vm + "</script></html>");

			var span = (HtmlElement)doc.GetElementById("c1");
			Assert.IsNotNull(span);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Text)span.FirstChild).Data);
			Assert.AreEqual("Hello", span.InnerHTML);

			var evt = doc.CreateEvent("Event");
			evt.InitEvent("click", false, false);
			span.DispatchEvent(evt);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("World", ((Text)span.FirstChild).Data);
			Assert.AreEqual("World", span.InnerHTML);

			span.Click();
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Text)span.FirstChild).Data);
			Assert.AreEqual("Hello", span.InnerHTML);
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

			var doc = Load("<html><head><script> " + Resources.knockout + " </script></head>"+
				"<body>" +
				"<input type='text' data-bind='value:Name' id='in'/>" +
				"<span id = 'c1' data-bind='text:Greeting'/>" +
				"</body><script>" + vm + "</script></html>");

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
			var doc = Load(@"function VM() {
	var _this = this;	
	this.Checked = ko.observable(true);
	this.Click = function(){_this.Checked(!_this.Checked());};
}
ko.applyBindings(new VM());",
				"<input type='checkbox' data-bind='checked:Checked' id='in'/>" +
				"<div id = 'button' data-bind='click:Click'>Click me</div>");

			var div = (HtmlElement)doc.GetElementById("button");
			var checkbox = (HtmlInputElement) doc.GetElementById("in");
			Assert.IsNotNull(checkbox);
			Assert.IsNotNull(div);
			Assert.IsTrue(checkbox.Checked);
			div.Click();
			Assert.IsFalse(checkbox.Checked);
		}

		[Test]
		public void ForeachBinding()
		{
			var doc = Load(
@"function VM(peoples) {
	var _this = this;	
	this.Peoples = ko.observableArray(peoples);
	this.Click = function(){_this.Peoples.push({Name:'Neo'});};
}
ko.applyBindings(new VM([{Name:'Ivan'},{Name:'Vasil'}]));",
@"<!-- ko foreach: Peoples -->
		<span data-bind='text:Name'></span>
	<!-- /ko -->
	<input type='button' id = 'button' data-bind='click:Click' value='Click me'/>");

			var button = (HtmlInputElement)doc.Body.GetElementsByTagName("input").First();
			var spans = doc.Body.GetElementsByTagName("span").ToArray();
			Assert.AreEqual(2, spans.Length);
			Assert.AreEqual("Ivan", spans[0].InnerHTML);
			Assert.AreEqual("Vasil", spans[1].InnerHTML);

			button.Click();

			var newSpans = doc.Body.GetElementsByTagName("span").ToArray();
			Assert.AreEqual(3, newSpans.Length);
			Assert.AreEqual("Ivan", newSpans[0].InnerHTML);
			Assert.AreEqual("Vasil", newSpans[1].InnerHTML);
			Assert.AreEqual("Neo", newSpans[2].InnerHTML);
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

			var doc = Load("<html><head><script> " + Resources.knockout + " </script>" +
@"<script type='text/html' id='tmpl'>
<span id = 'c1' data-bind='text:Greeting, click: Click'/>
</script>
</head><body><div data-bind='template:""tmpl""'></div></body><script>" + vm + "</script></html>");

			var span = (HtmlElement)doc.GetElementById("c1");
			Assert.IsNotNull(span);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Text)span.FirstChild).Data);
			Assert.AreEqual("Hello", span.InnerHTML);

			var evt = doc.CreateEvent("Event");
			evt.InitEvent("click", false, false);
			span.DispatchEvent(evt);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("World", ((Text)span.FirstChild).Data);
			Assert.AreEqual("World", span.InnerHTML);

			span.Click();
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Text)span.FirstChild).Data);
			Assert.AreEqual("Hello", span.InnerHTML);
		}

		[Test]
		public void TemplateInsideForeachBinding()
		{
			var doc = Load("<html><head><script> " + Resources.knockout + " </script>" +
@"
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
<script>
function VM(peoples) {
	var _this = this;	
	this.Peoples = ko.observableArray(peoples);
	this.Click = function(){_this.Peoples.push({Name:'Neo'});};
}
ko.applyBindings(new VM([{Name:'Ivan'},{Name:'Vasil'}]));
</script>
</html>");

			var button = (HtmlInputElement)doc.Body.GetElementsByTagName("input").First();
			var divs = doc.Body.GetElementsByTagName("div").ToArray();
			Assert.AreEqual(2, divs.Length);

			button.Click();

			var newDivs = doc.Body.GetElementsByTagName("div").ToArray();
			Assert.AreEqual(3, newDivs.Length);

		}

		[Test]
		public void BindToNode()
		{
			var doc = Load("<html><head><script> " + Resources.knockout + " </script>" +
@"<script type='text/html' id='itemTemplate'>
	<span data-bind='text:Name'></span>
</script>
</head>
<body>
<div id='view'>
	<!-- ko foreach: Peoples -->
	<div data-bind=""template:'itemTemplate'""></div>
	<!-- /ko -->
	<input type='button' id = 'button' data-bind='click:Click' value='Click me'/>
</div>
</body>
<script>
function VM(peoples) {
	var _this = this;	
	this.Peoples = ko.observableArray(peoples);
	this.Click = function(){_this.Peoples.push({Name:'Neo'});};
}
ko.applyBindings(new VM([{Name:'Ivan'},{Name:'Vasil'}]), document.getElementById('view'));
</script>
</html>");

			var button = (HtmlInputElement)doc.Body.GetElementsByTagName("input").First();
			var divs = doc.Body.GetElementsByTagName("div").ToArray();
			Assert.AreEqual(3, divs.Length);

			button.Click();

			var newDivs = doc.Body.GetElementsByTagName("div").ToArray();
			Assert.AreEqual(4, newDivs.Length);

		}

		[Test]
		public void ComponentBinding()
		{
			var doc = Load("<html><head><script> " + Resources.knockout + " </script></head>" +
@"<body> <div id='view' data-bind=""component:{name:'myco'}""></div> </body>
<script>
	ko.components.register('myco', { 
		viewModel: {instance: { name:'Kos' }}, 
		template: '<div id=inner><div><span data-bind=""text:name""></span></div></div>'	
	});
	ko.applyBindings({});
</script>
</html>");
			
			var inner = doc.GetElementById("inner");
			Assert.NotNull(inner, "First component's div");
			Assert.AreEqual(1, inner.ChildNodes.Count, "First component's div children count");
			Assert.AreEqual(1, inner.ChildNodes[0].ChildNodes.Count);
			Assert.AreEqual("Kos", ((HtmlElement)inner.ChildNodes[0].ChildNodes[0]).InnerHTML);
		}
	}
}