using System;
using System.Linq;
using System.Net;
using System.Threading;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	[Ignore("For manual run")]
	[Category("manual")]
	public class OpenSites
	{
		private int timeout = 20000;

		[TestCase("http://okkamtech.com")]
		[TestCase("http://ya.ru")]
		[TestCase("http://redmine.todosoft.org")]
		[TestCase("http://google.com")]
		[TestCase("https://html5test.com")]
		public void OpenUrl(string url)
		{
			var engine = new Engine();
			engine.OpenUrl(url);
		}

		[Test]
		public void Octane()
		{
			var engine = new Engine();
			engine.OpenUrl("http://chromium.github.io/octane").Wait(timeout);
			var startButton = engine.WaitSelector("h1#main-banner", 10000) as HtmlElement;
			Assert.IsNotNull(startButton, "Start button not found.");

			startButton.Click();

			Thread.Sleep(10000);

			System.Console.WriteLine("Score: " + startButton.InnerHTML);
		}

		[Test]
		public void Css3test()
		{
			var engine = new Engine();
			engine.OpenUrl("http://css3test.com").Wait(timeout);
			var score = engine.WaitId("score");
			Assert.IsNotNull("score");
			System.Console.WriteLine("Score: " + score.InnerHTML);
		}

		[Test]
		public void Html5Score()
		{
			var engine = new Engine();
			engine.OpenUrl("https://html5test.com").Wait(timeout);

			var tagWithValue = engine.WaitSelector("#score strong").FirstOrDefault();
			Assert.IsNotNull(tagWithValue, "strong");
			System.Console.WriteLine("Score: " + tagWithValue.InnerHTML);
			Thread.Sleep(500);

			foreach (var category in ("parsing elements form location output input communication webrtc interaction " +
			                         "performance security history offline storage files streams video audio responsive " +
			                         "canvas webgl animation components other").Split(' '))
			{
				try
				{
					System.Console.WriteLine(category + ": " +
					                         engine.Document.GetElementById("head-" + category).GetElementsByTagName("span")[0]
						                         .InnerHTML);
				}
				catch (Exception)
				{
					System.Console.WriteLine(category + " not found");
				}
			}
		}

		[Test]
		public void BrowseOkkam()
		{
			var engine = new Engine();
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception.ToString());
			engine.OpenUrl("http://okkamtech.com").Wait(timeout);
			var userName = engine.WaitId("UserName");
			Assert.IsNotNull(userName);
		}

		[Test]
		public void LogonToOkkam()
		{
			var engine = new Engine();
			engine.AttachConsole();
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
			var engine = new Engine();
			engine.AttachConsole();
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
			var engine = new Engine() {ComputedStylesEnabled = true};
			engine.AttachConsole();
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
			var engine = new Engine();
			Assert.Throws<AggregateException>(() => engine.OpenUrl("http://asd.okkamtech.com/").Wait());
		}

		[Test]
		public void Redmine()
		{
			var engine = new Engine();
			engine.OpenUrl("http://red.todosoft.ru").Wait();
			var login = engine.WaitSelector("a.login").FirstOrDefault() as HtmlElement;
			Assert.IsNotNull(login, "login button");
			engine.Window.Location.Href = login.GetAttribute("href");//login.Click();
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
			var engine = new Engine();
			engine.OpenUrl("http://whatsmyuseragent.org/").Wait();
			var uaDiv = (HtmlDivElement)engine.WaitSelector("div.user-agent").FirstOrDefault();
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

			var engine = new Engine(resourceProvider);

			engine.OpenUrl("https://rutracker.org").Wait();
			System.Console.WriteLine(engine.Document.DocumentElement.InnerHTML);
		}
	}
}
