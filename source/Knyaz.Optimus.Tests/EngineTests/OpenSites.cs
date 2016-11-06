using System;
using System.Linq;
using System.Threading;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture, Ignore]
	public class OpenSites
	{
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
			engine.OpenUrl("http://chromium.github.io/octane");
			engine.WaitDocumentLoad();
			var startButton = engine.WaitId("main-banner") /*as HtmlElement*/;
			Assert.IsNotNull(startButton);

			//startButton.Click();

			Thread.Sleep(10000);

			System.Console.WriteLine("Score: " + startButton.InnerHTML);
		}

		[Test]
		public void Css3test()
		{
			var engine = new Engine();
			engine.OpenUrl("http://css3test.com");
			engine.WaitDocumentLoad();
			var score = engine.WaitId("score");
			Assert.IsNotNull("score");
			System.Console.WriteLine("Score: " + score.InnerHTML);
		}

		[Test]
		public void Html5Score()
		{
			var engine = new Engine();
			engine.OpenUrl("https://html5test.com");
			engine.WaitDocumentLoad();

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
			engine.OpenUrl("http://okkamtech.com");
			Thread.Sleep(10000);
			var userName = engine.Document.GetElementById("UserName");
			Assert.IsNotNull(userName);
		}

		[Test]
		public void BrowseKwinto()
		{
			var engine = new Engine();
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception.ToString());
			engine.OpenUrl("http://192.168.1.36:8891");
			Thread.Sleep(10000);
			var userName = engine.Document.GetElementById("UserName");

			Assert.IsNotNull(userName);
		}

		[Test]
		public void LogonToOkkam()
		{
			var engine = new Engine();
			engine.AttachConsole();
			engine.OpenUrl("http://okkamtech.com");
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
			engine.OpenUrl("http://localhost:2930");

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
			engine.OpenUrl("http://chi.todosoft.org");

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
	}
}
