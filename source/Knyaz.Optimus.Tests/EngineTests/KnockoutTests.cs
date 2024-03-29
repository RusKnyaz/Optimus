﻿using System.Linq;
using System.Threading.Tasks;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using NUnit.Framework;
using Text = Knyaz.Optimus.Dom.Elements.Text;
using Knyaz.Optimus.Tests.Resources;
using Knyaz.Optimus.Tests.TestingTools;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public class KnockoutTests
	{
		static async Task<HtmlDocument> Load(string html)
		{
			var rp = Mocks.ResourceProvider("http://localhost", html);
			var engine = TestingEngine.BuildJint(rp, SystemConsole.Instance);
			return (await engine.OpenUrl("http://localhost")).Document;
		}

		static Task<HtmlDocument> Load(string script, string body) =>
			Load("<html><head><script>" + R.KnockoutJs + "</script></head><body>" + body + "</body><script>" +
			     script + "</script></html>");

		[Test]
		public async Task KnockoutInclude()
		{
			await Load("<html><head><script> " + R.KnockoutJs + " </script></head><body></body></html>");
		}

		[Test]
		public async Task KnockoutViewModel()
		{
			var document = await Load(@"function VM() {
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
		public async Task KnockoutClick()
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

			var doc = await Load("<html><head><script> " + R.KnockoutJs + " </script></head><body><span id = 'c1' data-bind='text:Greeting, click: Click'/></body>" +
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
		public async Task KnockoutInputAndComputed()
		{
			var vm =
@"function VM() {
	var _this = this;	
	this.Name = ko.observable('World');
	this.Greeting = ko.computed(function(){return 'Hello, ' + _this.Name();});
}
ko.applyBindings(new VM());";

			var doc = await Load("<html><head><script> " + R.KnockoutJs + " </script></head>"+
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
		public async Task KnockoutInputCheckbox()
		{
			var doc = await Load(@"function VM() {
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
		public async Task ForeachBinding()
		{
			var doc = await Load(
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
		public async Task ArrayPushAll()
		{
			var doc = await Load(
@"function VM() {
	var _this = this;	
	this.Peoples = ko.observableArray();
	this.Click = function(){_this.Peoples.push({Name:'Neo'});};
}
var vm = new VM();
ko.applyBindings(vm);
ko.utils.arrayPushAll(vm.Peoples, [{Name:'Ivan'},{Name:'Vasil'}]);peoples",
@"<!-- ko foreach: Peoples -->
		<span data-bind='text:Name'></span>
<!-- /ko -->");

			var spans = doc.Body.GetElementsByTagName("span").ToArray();
			Assert.AreEqual(2, spans.Length);
			Assert.AreEqual("Ivan", spans[0].InnerHTML);
			Assert.AreEqual("Vasil", spans[1].InnerHTML);
		}

		[Test]
		public async Task Template()
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

			var doc = await Load("<html><head><script> " + R.KnockoutJs + " </script>" +
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
		public async Task TemplateInsideForeachBinding()
		{
			var doc = await Load("<html><head><script> " + R.KnockoutJs + " </script>" +
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
		public async Task BindToNode()
		{
			var doc = await Load("<html><head><script> " + R.KnockoutJs + " </script>" +
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
		public static async Task ComponentBinding()
		{
			var doc = await Load("<html><head><script> " + R.KnockoutJs + " </script></head>" +
@"<body> <div id='view' data-bind=""component:{name:'myco'}""></div> </body>
<script>
	ko.components.register('myco', { 
		viewModel: {instance: { name:'Kos' }}, 
		template: '<div id=inner><div><span data-bind=""text:name""></span></div></div>'	
	});
	ko.applyBindings({});
</script>
</html>");
			
			var inner = doc.WaitId("inner");
			Assert.NotNull(inner, "First component's div");
			Assert.AreEqual(1, inner.ChildNodes.Count, "First component's div children count");
			Assert.AreEqual(1, inner.ChildNodes[0].ChildNodes.Count);
			Assert.AreEqual("Kos", ((HtmlElement)inner.ChildNodes[0].ChildNodes[0]).InnerHTML);
		}

		[Test]
		public async Task HtmlWithScriptBinding()
		{
			var doc = await Load("<html><head><script> " + R.KnockoutJs + " </script></head>" +
@"<body> <div id='view' data-bind=""html:html""></div> </body>
<script>
	ko.applyBindings({ html:'<script type=""text/javascript"">var d = document.createElement(""div"");d.id=""d""; document.body.appendChild(d);</script>' });
</script>
</html>");
			Assert.IsNotNull(doc.GetElementById("d"));
		}

		[Test]
		public async Task HtmlWithScriptBindingWithJquery()
		{
			var document = await Load("<html><head><script>" + R.JQueryJs+"</script><script> " + R.KnockoutJs + " </script></head>" +
			                           @"<body> <div id='view' data-bind=""html:html""></div> </body>
<script>
	ko.applyBindings({ html:'<script type=""text/javascript"">var d = document.createElement(""div"");d.id=""d""; document.body.appendChild(d);</script>' });
</script>
</html>");
			Assert.IsNotNull(document.GetElementById("d"));
		}

		[Test]
		public async Task SelectOptionsTest()
		{
			var document = await Load("<html><head><script>" + R.JQueryJs + "</script><script> " + R.KnockoutJs + " </script></head>" +
						@"<body> <select id='s' data-bind=""options:options""></select> </body>
<script>
	ko.applyBindings({ options: ['A','B']});
</script>
</html>");
			var select = document.GetElementById("s") as HtmlSelectElement;
			Assert.IsNotNull(select);
			Assert.AreEqual(2, select.Options.Length);
		}
	}
}