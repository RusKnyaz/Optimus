using System;
using System.IO;
using System.Threading.Tasks;
using Knyaz.Optimus;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
//using Knyaz.Optimus.Scripting.Jurassic;
using Knyaz.Optimus.TestingTools;

namespace MySiteScraper
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var url = "https://localhost:5001";
			
			Console.WriteLine($"Going to scrap the {url}, (do not forget to run the MySite)");

			// ---------------------------------
			// 1. Configuring the web engine.
			// ---------------------------------
			var engine = EngineBuilder.New()
				.ConfigureResourceProvider(res => res.Http(h => h.ConfigureClientHandler(c =>
					//avoid "System.Net.Http.HttpRequestException: The SSL connection could not be established,..."
					c.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true)))
				.UseJint()
				//.UseJurassic()
				//redirect browser's console to the application console.
				.Window(w => w.SetConsole(new SystemConsole()))
				.Build();
			
			// ---------------------------------
			// 2. Load the sample's site web page
			// ---------------------------------
			Console.Write("Loading the root page...");
			var page = await engine.OpenUrl(url);
			Console.WriteLine(page is HttpPage httpPage ? httpPage.HttpStatusCode: "Error");
			
			// ---------------------------------
			// 3. Go to login form
			// ---------------------------------
			Console.Write("Go to login form...");
			var loginAnchor = page.Document.WaitId("login") as HtmlAnchorElement;
			if (loginAnchor == null)
			{
				Console.WriteLine("Error: Login button not found.");
				return;
			}

			var loginPage = await engine.OpenUrl(loginAnchor.Href);
			Console.WriteLine(page is HttpPage httpLoginPage ? httpLoginPage.HttpStatusCode: "Error");

			// ---------------------------------
			// 4. Go to login form
			// ---------------------------------
			Console.Write("Login...");
			var userName = loginPage.Document.WaitId("Input_Email") as HtmlInputElement;
			var password = loginPage.Document.WaitId("Input_Password") as HtmlInputElement;
			var loginButton = loginPage.Document.WaitId("login-submit") as HtmlButtonElement;
			if (userName == null || password == null || loginButton == null)
			{
				Console.WriteLine("Error: Login form is not loaded correctly.");
				return;
			}
			userName.EnterText("user@mycompany.com");
			password.EnterText("QWEqwe123!");
			loginButton.Click(); //or loginButton.Form.Submit();

			//The appearance of the 'logout' button is a sign that we are logged in.
			Console.WriteLine(engine.WaitId("logout", 2000) is HtmlButtonElement ? "OK" : "ERROR");

			// -----------------------------------------
			// 5. Download secret file which can not be
			// downloaded without authorization.
			// -----------------------------------------
			var anchor = (HtmlAnchorElement)engine.WaitId("secret_link");
			if (anchor == null)
			{
				Console.WriteLine("Download link not found.");
			}
			else
			{
				Console.WriteLine("Downloading: " + anchor.Href);
				using var s = engine.DownloadAsync(url + anchor.Href).Result;
				var data = s.ReadBytesToEnd();
				Console.WriteLine($"Downloaded {data.Length} bytes.");
			}
		}
	}

	static class Extensions
	{
		public static byte[] ReadBytesToEnd(this Stream input)
		{
			byte[] buffer = new byte[16*1024];
			using (var ms = new MemoryStream())
			{
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}

		public static async Task<Stream> DownloadAsync(this Engine engine, string href) =>
			(await engine.ResourceProvider.SendRequestAsync(new Request("GET", new Uri(href)) {
				Cookies = engine.CookieContainer
			})).Stream;
	}
}