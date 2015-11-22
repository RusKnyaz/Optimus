using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using WebBrowser.Dom.Elements;
using WebBrowser.Tools;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture]
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
		public void Html5Score()
		{
			var engine = new Engine();
			engine.OpenUrl("https://html5test.com");
			engine.WaitDocumentLoad();
			Thread.Sleep(5000);//wait calculation

			var score = engine.Document.GetElementById("score");
			Assert.IsNotNull(score, "score");
			var tagWithValue = score.GetElementsByTagName("strong").FirstOrDefault();
			Assert.IsNotNull(tagWithValue, "strong");
			System.Console.WriteLine("Score: " + tagWithValue.InnerHTML);

			foreach (var category in ("parsing elements form location output input communication webrtc interaction " +
			                         "performance security history offline storage files streams video audio responsive " +
			                         "canvas webgl animation").Split(' '))
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

			userName.Value = "admin";
			password.Value = "b";
			logonButton.Click();

			var error = engine.WaitId("validationError");
			Assert.IsNotNull(error, "error");
			System.Console.WriteLine(error.InnerHTML);
		}

		[Test]
		public void LogonToKwinto()
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

			userName.Value = "admin";
			password.Value = "admin";
			logonButton.Click();
			Thread.Sleep(1000);//wait while document changed

			var error = engine.WaitId("logout");
			Assert.IsNotNull(error, "logout");
		}
	}
}
