#if NUNIT
using System;
using System.IO;
using System.Text;
using Jint.Runtime;
using NUnit.Framework;
using WebBrowser.Dom.Elements;
using WebBrowser.Properties;
using WebBrowser.ScriptExecuting;
using Text = WebBrowser.Dom.Text;

namespace WebBrowser.Tests
{
	[TestFixture]
	public class EngineTests
	{
		[Test]
		public void EmptyHtml()
		{
			var engine = new Engine();
			engine.Load("<html></html>");
		}
		
		[Test]
		public void GenerateContent()
		{
			var engine = new Engine();
			engine.Load("<html><head><script>var elem = document.getElementById('content');elem.innerHTML = 'Hello';</script></head><body><div id='content'></div></body></html>");
			var contentDiv = engine.Document.GetElementById("content");
			Assert.AreEqual("Hello", contentDiv.InnerHtml);
			Assert.AreEqual(1, contentDiv.ChildNodes.Count);
			var text = contentDiv.ChildNodes[0] as Text;
			Assert.IsNotNull(text);
			Assert.AreEqual("Hello", text.Data);
		}

		[Test]
		public void DomManipulation()
		{
			var engine = new Engine();
			engine.Load("<html><head><script>var c2 = document.getElementById('content2');document.documentElement.getElementsByTagName('body')[0].insertBefore(document.createElement('<div id=\"c3\">'), c2);</script></head><body><div id='content1'></div><div id='content2'></div></body></html>");
			Assert.AreEqual(3, engine.Document.DocumentElement.GetElementsByTagName("body")[0].ChildNodes.Count);
			var elem = engine.Document.GetElementById("c3");
			Assert.IsNotNull(elem);
		}

		[Test]
		public void KnockoutInclude()
		{
			var engine = new Engine();
			engine.Load("<html><head><script> "+ Resources.knockout+" </script></head><body></body></html>");
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
			var console = new StringBuilder();
			ScriptExecutor.Log += o =>
			{
				if(o == null)
					return;
				;
				console.AppendLine(o.ToString()); Console.WriteLine(o);
			};
			
			engine.Load("<html><head><script> " + Resources.knockout+ " </script><script>"+vm+"</script></head><body><span id = 'c1' data-bind='text:Greeting'/></body></html>");
			
			var span = engine.Document.GetElementById("c1");
			Assert.IsNotNull(span);
			Console.Write(engine.Document.DocumentElement.InnerHtml);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Text)span.FirstChild).Data);
			Assert.AreEqual("Hello",  span.InnerHtml);
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
			var console = new StringBuilder();
			ScriptExecutor.Log += o =>
			{
				if (o == null)
					return;
				console.AppendLine(o.ToString()); Console.WriteLine(o);
			};

			engine.Load("<html><head><script> " + Resources.knockout + " </script><script>" + vm + "</script></head><body><span id = 'c1' data-bind='text:Greeting, click: Click'/></body></html>");

			var span = (HtmlElement)engine.Document.GetElementById("c1");
			Assert.IsNotNull(span);
			Assert.AreEqual(1, span.ChildNodes.Count);
			Assert.AreEqual("Hello", ((Text)span.FirstChild).Data);
			Assert.AreEqual("Hello", span.InnerHtml);

			var evt = engine.Document.CreateEvent("click");
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
		public void JQuery()
		{
			var engine = new Engine();
			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script></head><body></body></html>");
		}

		[Test]
		public void JQueryIdSelector()
		{
			var script = "$('#uca').html('zaza');";
			ScriptExecutor.Log = o => Console.WriteLine(o.ToString());
			var engine = new Engine();
			engine.Load("<html><head><script> " + Resources.jquery_2_1_3 + " </script><script>"+script+"</script></head><body><div id='uca'></div></body></html>");
			var ucaDiv = engine.Document.GetElementById("uca");
			Assert.AreEqual("zaza", ucaDiv.InnerHtml);
		}
	}

	static class ExtensionAttribute
	{
		public static void Load(this Engine engine, string html)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(html)))
			{
				engine.Load(stream);
			}
		}
	}
}
#endif