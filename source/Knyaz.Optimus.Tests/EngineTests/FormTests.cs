using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knyaz.NUnit.AssertExpressions;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.Tests.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public class FormTests
	{
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
		
		[TestCase("application/x-www-form-urlencoded", ExpectedResult = "username=John%40mail.m&password=1%21+%25%26&Text1=hello%0D%0Aworld%3Dpip")]
		[TestCase("text/plain", ExpectedResult = "username=John@mail.m\r\npassword=1! %&\r\nText1=hello\r\nworld=pip\r\n")]
		[TestCase("multipart/form-data", ExpectedResult = 
			"-----------------------------421941713216853671061224586803\r\nContent-Disposition: form-data; name=\"username\"\r\n\r\nJohn@mail.m\r\n" +
			"-----------------------------421941713216853671061224586803\r\nContent-Disposition: form-data; name=\"password\"\r\n\r\n1! %&\r\n" +
			"-----------------------------421941713216853671061224586803\r\nContent-Disposition: form-data; name=\"Text1\"\r\nhello\r\nworld=pip\r\n" +
			"-----------------------------421941713216853671061224586803--\r\n", Ignore = "Not implemented")]
		public static string SubmitPostFormEncodingType(string enctype)
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/",
					$"<form method=post action='login' enctype='{enctype}'>" +
					$"<input name=username type=text>" +
					$"<input name=password type=password>" +
					"<textarea name=\"Text1\" cols=\"40\" rows=\"5\"></textarea>" +
					$"</form>")
				.Resource("http://site.net/login", "<div id=d></div>");

			var engine = TestingEngine.Build(new ResourceProvider(httpResources, null));
			engine.OpenUrl("http://site.net").Wait();

			var doc = engine.Document;

			doc.Get<HtmlInputElement>("[name=username]").First().Value = "John@mail.m";
			doc.Get<HtmlInputElement>("[name=password]").First().Value = "1! %&";
			doc.Get<HtmlTextAreaElement>("[name=Text1]").First().Value = "hello\r\nworld=pip";
			doc.Get<HtmlFormElement>("form").First().Submit();
			Assert.IsNotNull(engine.WaitId("d"));

			return Encoding.UTF8.GetString(httpResources.History[1].Data);
		}

		[TestCase(true, ExpectedResult = "rememberme=true")]
		[TestCase(false, ExpectedResult = "")]
		public static string SubmitPostFormCheckbox(bool check)
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/",
					"<form method=post action='login'>" +
					"<input name=rememberme type=cheCkbox id=checkbox>" +
					"</form>")
				.Resource("http://site.net/login", "<div id=d></div>");

			var engine = TestingEngine.Build(new ResourceProvider(httpResources, null));
			engine.OpenUrl("http://site.net").Wait();

			var doc = engine.Document;

			var form = doc.Get<HtmlFormElement>("form").First();

			if (check)
			{
				var cb = (HtmlInputElement)form.Elements.Single();
				cb.Checked = true;
			}

			form.Submit();
			
			return Encoding.UTF8.GetString(httpResources.History[1].Data);
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
		public static async Task SubmitFormInSubWithParams(string method, string action, string expected)
		{
			//1. initial query should be removed from request on form submit
			//2. Form action query should be ignored.
			var resources = Mocks.ResourceProvider("http://site.net/sub/?var1=x",
					"<form method=" + method + " action='" + action + "'><input name=username type=text/><input name=password type=password/></form>")
				.Resource(expected, "<div id=d></div>");

			var engine = TestingEngine.BuildJint(resources);
			var page = await engine.OpenUrl("http://site.net/sub/?var1=x");
			
			var doc = page.Document;
			
			doc.Get<HtmlInputElement>("[name=username]").First().Value = "John";
			doc.Get<HtmlInputElement>("[name=password]").First().Value = "123456";
			doc.Get<HtmlFormElement>("form").First().Submit();
			Assert.IsNotNull(engine.WaitId("d"));
			doc.Assert(document => document.Location.Href == expected);
		}

		[Test]
		public static async Task SubmitNonHtmlResponse()
		{
			var resources = Mocks.ResourceProvider("http://site.net/sub",
					"<form method=get action='download'></form>")
				.Resource("http://site.net/sub/download", "<div id=d></div>", "image/png");
			
			var engine = TestingEngine.BuildJint(resources);
			var page = await engine.OpenUrl("http://site.net/sub");
			
			page.Document.Get<HtmlFormElement>("form").First().Submit();
			page.Document.Assert(doc => doc.Location.Href == "http://site.net/sub");
		}

		[Test]
		public static async Task OverrideSubmitActionInButton()
		{
			var resources = Mocks.ResourceProvider(
				"http://site.net/",
					"<form action='/login'><button id=b formAction='/logout'></button></form>")
				.Resource("http://site.net/login", "<div id=d>login</div>")
				.Resource("http://site.net/logout", "<div id=d>logout</div>");
			
			var engine = TestingEngine.BuildJint(resources);
			var page = await engine.OpenUrl("http://site.net");
			
			page.Document.Get<HtmlButtonElement>("button").First().Click();
			
			engine.Document.Assert(document =>
				document.WaitId("d").InnerHTML == "logout" &&
				document.Location.Href == "http://site.net/logout"
			);
		}

		[Test]
		public static async Task SubmitByClickOnButton()
		{
			var resources = Mocks.ResourceProvider("http://site.net/",
					"<form action='/login'><button id=b type='submit'></button></form>")
				.Resource("http://site.net/login", "<div id=d>login</div>");
			
			var engine = TestingEngine.BuildJint(resources);
			var page = await engine.OpenUrl("http://site.net");
			
			page.Document.Get<HtmlButtonElement>("button").First().Click();
			
			engine.Document.Assert(doc => doc.Location.Href == "http://site.net/login");
		}

		[Test]
		public static async Task OmitDisabledInputs()
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/",
					"<form method='post'>" +
					"<input name=t1 value=A/>" +
					"<input name=t2 value=B disabled/>" +
					"<input name=t3 value=C disabled=1/>" +
					"<input name=t4 value=D disabled=0/>" +
					"<button id=b type='submit'></button></form>");
			
			var engine = TestingEngine.BuildJint(new ResourceProvider(httpResources, null));
			var page = await engine.OpenUrl("http://site.net");
			
			page.Document.Get<HtmlButtonElement>("button").First().Click();
			
			var data = 	Encoding.UTF8.GetString(httpResources.History[1].Data);
			Assert.AreEqual("t1=A", data);
			
		}

		[Test]
		public static async Task OnSubmitEventCalled()
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/",
					"<form method=post action='login'><input name=username type=text><input type=submit id=sub></form>")
				.Resource("http://site.net/login", "<div id=d></div>");

			var engine = TestingEngine.Build(new ResourceProvider(httpResources, null));
			engine.OpenUrl("http://site.net").Wait();

			var doc = engine.Document;

			var submitButton = doc.Get<HtmlInputElement>("#sub").First();
			var form = doc.Get<HtmlFormElement>("form").First();;

			bool formsOnSubmitCalled = false;
			form.OnSubmit += _ => formsOnSubmitCalled = true;
			bool windowOnSubmitCalled = false;
			engine.Window.OnSubmit += _ => windowOnSubmitCalled = true; 
			
			submitButton.Click();
			
			Assert.IsTrue(formsOnSubmitCalled);
			Assert.IsTrue(windowOnSubmitCalled);
		}
		
		[Test]
		public static async Task OnSubmitEventNotCalled()
		{
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/",
					"<form method=post action='login'><input name=username type=text><input type=submit id=sub></form>")
				.Resource("http://site.net/login", "<div id=d></div>");

			var engine = TestingEngine.Build(new ResourceProvider(httpResources, null));
			engine.OpenUrl("http://site.net").Wait();

			var doc = engine.Document;

			var form = doc.Get<HtmlFormElement>("form").First();;

			bool formsOnSubmitCalled = false;
			form.OnSubmit += _ => formsOnSubmitCalled = true;
			bool windowOnSubmitCalled = false;
			engine.Window.OnSubmit += _ => windowOnSubmitCalled = true; 
			
			form.Submit();
			
			Assert.IsFalse(formsOnSubmitCalled);
			Assert.IsFalse(windowOnSubmitCalled);
		}

		[Test]
		public static async Task EventsOrder()
		{
			var html = @"<form><button id=b>clickme</button></form>";
			
			var js = @"
			window.onsubmit = function(){console.log('window onsubmit');};
			window.onclick = function(){console.log('window onclick')};
			var f = document.getElementsByTagName('form')[0];
			f.onsubmit = function(){console.log('form onsubmit')};
			f.onclick = function(){console.log('form onclick')};
			var b = document.getElementById('b');
			b.onclick=function(){console.log('button onclick')};";
			
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/", $"{html}<script>{js}</script>");

			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(httpResources, console);
			
			engine.OpenUrl("http://site.net").Wait();

			var doc = engine.Document;

			var button = doc.Get<HtmlButtonElement>("#b").First();
			
			button.Click();
			
			Assert.AreEqual(new[]
			{
				"button onclick",
				"form onclick",
				"window onclick",
				"form onsubmit",
				"window onsubmit"
			}, console.LogHistory);
		}
		
		[Test]
		public static async Task EventsOrder2()
		{
			var html = @"<form id=f><button id=b>clickme</button></form>";
			
			var js = @"var b = document.getElementById('b');

b.addEventListener( 'click', function(){console.log('b.click')}, false );
b.addEventListener( 'click', function(){console.log('b.click bubble')}, true );
b.addEventListener( 'subscribe', function(){console.log('b.subscribe')}, false );

var f = document.getElementById('f');
f.addEventListener( 'click', function(){console.log('f.click')}, false );
f.addEventListener( 'click', function(){console.log('f.click bubble')}, true );
f.addEventListener( 'subscribe', function(){console.log('f.subscribe')}, false );

document.addEventListener( 'click', function(){console.log('d.click')}, false );
document.addEventListener( 'click', function(){console.log('d.click bubble')}, true );
document.addEventListener( 'subscribe', function(){console.log('d.subscribe')}, false );";
			
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/", $"{html}<script>{js}</script>");

			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(httpResources, console);
			var page = await engine.OpenUrl("http://site.net");
			
			engine.OnUriChanged += () => console.Log("reloaded");

			var doc = page.Document;

			var button = doc.Get<HtmlButtonElement>("#b").First();
			
			button.Click();

			await Task.Delay(1000);
			
			Assert.AreEqual(new[]
			{
				"d.click bubble",
				"f.click bubble",
				"b.click",
				"b.click bubble",
				"f.click",
				"d.click",
				"reloaded"
			}, console.LogHistory);
		}
		
		

		[Test]
		public static async Task CancelSubmit()
		{
			var html = @"<form><button id=b>clickme</button></form>";
			
			var js = @"
			window.onsubmit = function(){console.log('window onsubmit');};
			window.onclick = function(){console.log('window onclick')};
			var f = document.getElementsByTagName('form')[0];
			f.onsubmit = function(){console.log('form onsubmit')};
			f.onclick = function(){console.log('form onclick')};
			var b = document.getElementById('b');
			b.onclick=function(){console.log('button onclick'); return false;};";
			
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/", $"{html}<script>{js}</script>");

			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(httpResources, console);
			
			engine.OpenUrl("http://site.net").Wait();

			var doc = engine.Document;

			var button = doc.Get<HtmlButtonElement>("#b").First();
			
			button.Click();
			
			Assert.AreEqual(new[]
			{
				"button onclick",
				"form onclick",
				"window onclick"
			}, console.LogHistory);
		}
		
		[TestCase("true", "click,submit,reload")]
		[TestCase("false", "click")]
		public static async Task CancelSubmitFromWindow(string returnValue, string log)
		{
			var html = @"<form><button id=b>clickme</button></form>";
			
			var js = "window.onsubmit=function(){console.log('submit')};"+
			         $"window.onclick=function(){{console.log('click'); return {returnValue};}};";
			
			var httpResources = Mocks.HttpResourceProvider()
				.Resource("http://site.net/", $"{html}<script>{js}</script>");

			var console = new TestingConsole();
			var engine = TestingEngine.BuildJint(httpResources, console);
			
			var page = await engine.OpenUrl("http://site.net");

			engine.OnUriChanged += () => console.Log("reload");

			var button = page.Document.Get<HtmlButtonElement>("#b").First();
			
			button.Click();

			await Task.Delay(1000);

			Assert.AreEqual(log, string.Join(",", console.LogHistory));
		}
	}
}