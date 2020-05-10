using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.Tests.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.EngineTests
{

#if NETCOREAPP3_1
[Ignore("Rider runs 'Explicits' under Netcore")]
#endif
	[TestFixture, Explicit]
	public class OpenSites
	{
		private int timeout = 20000;

		[TestCase("http://okkamtech.com")]
		[TestCase("http://ya.ru")]
		[TestCase("http://redmine.todosoft.ru")]
		[TestCase("http://todosoft.ru")]
		[TestCase("http://google.com")]
		[TestCase("https://html5test.com")]
		public void OpenUrl(string url)
		{
			var engine = TestingEngine.BuildJint();
			engine.OpenUrl(url);
		}

		private Engine Engine() =>
			EngineBuilder.New().UseJint().Window(w => w.SetConsole(new SystemConsole())).Build();

		[Test]
		public async Task Octane()
		{
			var engine = Engine();
			engine.LogEvents();
			
			var page = await engine.OpenUrl("http://chromium.github.io/octane");
			var startButton = page.Document.WaitSelector("h1#main-banner", 10000).FirstOrDefault() as HtmlElement;
			Assert.IsNotNull(startButton, "Start button not found.");

			startButton.Click();

			Thread.Sleep(10000);
			
			var richard = (HtmlElement)page.Document.WaitId("Result-Richards");
			var deltaBlue = (HtmlElement) page.Document.WaitId("Result-DeltaBlue");
			var regex = (HtmlElement) page.Document.WaitId("Result-RegExp");
			var zlib = (HtmlElement) page.Document.WaitId("Result-zlib");
			
			System.Console.WriteLine($"Richard: {richard.TextContent}");
			System.Console.WriteLine($"DeltaBlue: {deltaBlue.TextContent}");
			System.Console.WriteLine($"Regex: {regex.TextContent}");
			System.Console.WriteLine($"Zlib: {zlib.TextContent}");

			System.Console.WriteLine("Score: " + startButton.InnerHTML);
		}

		[Test]
		public void Css3test()
		{
			var engine = TestingEngine.BuildJint();
			engine.LogEvents();
			engine.OpenUrl("http://css3test.com").Wait(timeout);
			
			Thread.Sleep(10000);
			var score = engine.WaitId("score");
			
			Assert.IsNotNull("score");
			System.Console.WriteLine("Score: " + score.InnerHTML);
		}

		[Test]
		public void Html5Score()
		{
			var engine = EngineBuilder.New().UseJint().Build();
			engine.OpenUrl("https://html5test.com").Wait(timeout);

			var tagWithValue = engine.WaitSelector("#score strong").FirstOrDefault();
			Assert.IsNotNull(tagWithValue, "strong");
			System.Console.WriteLine("Score: " + tagWithValue.InnerHTML);
			Thread.Sleep(500);

			foreach (var category in ("parsing elements form location output input communication " +
			                          "interaction performance security offline " +
			                          "storage files streams video audio responsive " +
			                          "canvas animation components scripting other").Split(' '))
			{
				var headerRow = engine.Document.WaitId($"head-{category}");
				if (headerRow == null)
				{
					System.Console.WriteLine($"Header of {category} not found");
					continue;
				}

				var scoreElement = engine.WaitSelector($"#head-{category} span").FirstOrDefault();
				if (scoreElement == null)
				{
					System.Console.WriteLine($"Score of {category} category not found.");
					continue;
				}

				System.Console.WriteLine(category + ": " + scoreElement.InnerHTML);
			}
		}

		[Test]
		public void BrowseOkkam()
		{
			var engine = TestingEngine.BuildJint();
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception.ToString());
			engine.OpenUrl("http://okkamtech.com").Wait(timeout);
			var userName = engine.WaitId("UserName");
			Assert.IsNotNull(userName);
		}

		[Test]
		public void LogonToOkkam()
		{
			var engine = TestingEngine.BuildJint();
			engine.LogEvents();
			engine.OpenUrl("http://okkamtech.com").Wait(timeout);
			//engine.OpenUrl("http://localhost:2930");

			var logonButton = engine.WaitId("logon") as HtmlElement;
			Thread.Sleep(5000);

			var userName = engine.Document.GetElementById("UserName") as HtmlInputElement;
			var password = engine.Document.GetElementById("Password") as HtmlInputElement;
			Assert.IsNotNull(logonButton, "LogonButton");
			Assert.IsNotNull(userName, "UserName");
			Assert.IsNotNull(password, "Password");

			userName.Value = "a";
			password.Value = "b";
			logonButton.Click();

			var error = engine.WaitId("validationError");
			Assert.IsNotNull(error, "error");
		}

		[Test]
		public void LogonToKwintoError()
		{
			var engine = TestingEngine.BuildJint();
			engine.LogEvents();
			engine.OpenUrl("http://localhost:2930").Wait(timeout);

			var logonButton = engine.WaitId("logon") as HtmlElement;
			Thread.Sleep(5000);

			var userName = engine.Document.GetElementById("UserName") as HtmlInputElement;
			var password = engine.Document.GetElementById("Password") as HtmlInputElement;
			Assert.IsNotNull(logonButton, "LogonButton");
			Assert.IsNotNull(userName, "UserName");
			Assert.IsNotNull(password, "Password");

			userName.EnterText("admin");
			password.EnterText("b");
			logonButton.Click();

			var error = engine.WaitId("validationError");
			Assert.IsNotNull(error, "error");
			System.Console.WriteLine(error.InnerHTML);
		}

		[Test]
		public void LogonToKwinto()
		{
			var engine = TestingEngine.BuildJintCss();
			engine.LogEvents();
			engine.OpenUrl("http://chi.todosoft.org").Wait(timeout);

			var logonButton = engine.WaitId("logon") as HtmlElement;

			var userName = engine.WaitId("UserName") as HtmlInputElement;
			var password = engine.WaitId("Password") as HtmlInputElement;
			Assert.IsNotNull(logonButton, "LogonButton");
			Assert.IsNotNull(userName, "UserName");
			Assert.IsNotNull(password, "Password");

			userName.EnterText("admin");
			password.EnterText("admin");
			logonButton.Click();

			var error = engine.WaitId("logout");
			Assert.IsNotNull(error, "logout");
		}

		[Test]
		public void Open404()
		{
			var engine = TestingEngine.BuildJint();
			Assert.Throws<AggregateException>(() => engine.OpenUrl("http://asd.okkamtech.com/").Wait());
		}

		[Test]
		public void Redmine()
		{
			var engine = TestingEngine.BuildJint();
			engine.OpenUrl("http://red.todosoft.ru").Wait();
			var login = engine.WaitSelector("a.login").FirstOrDefault() as HtmlElement;
			Assert.IsNotNull(login, "login button");
			engine.Window.Location.Href = login.GetAttribute("href"); //login.Click();
			var userNameInput = engine.WaitId("username") as HtmlInputElement;
			Assert.IsNotNull(userNameInput, "user name inpit");
			var passwordInput = engine.WaitId("password") as HtmlInputElement;

			userNameInput.Value = "";
			passwordInput.Value = "";
			engine.Document.Get<HtmlInputElement>("[name=login]").First().Click();

			var loggedUser = engine.WaitSelector("a.user.active").FirstOrDefault();
			Assert.IsNotNull(loggedUser);
		}

		[Test]
		public void MyUserAgent()
		{
			var engine = TestingEngine.BuildJint();
			engine.OpenUrl("http://whatsmyuseragent.org/").Wait();
			var uaDiv = (HtmlDivElement) engine.WaitSelector("div.user-agent").FirstOrDefault();
			var ua = uaDiv?.TextContent;
			Assert.IsNotNull(ua);
			System.Console.Write(ua);
			Assert.IsTrue(ua.Contains("Optimus"));
		}

		[Test]
		public void ProxyTest()
		{
			var resourceProvider = new ResourceProviderBuilder()
				.Http(x => x.Proxy(new WebProxy("88.146.227.247", 8080)))
				.Build();

			var engine = TestingEngine.BuildJint(resourceProvider);

			engine.OpenUrl("https://rutracker.org").Wait();
			System.Console.WriteLine(engine.Document.DocumentElement.InnerHTML);
		}

		[Test]
		public async Task OpenHabr()
		{
			var engine = TestingEngine.BuildJint();
			engine.LogEvents();
			var page = await engine.OpenUrl("https://habr.com");
			System.Console.WriteLine(page.Document.InnerHTML);
			//var logo = page.Document.WaitSelector(".logo-wrapper").First();
		}
	}
}