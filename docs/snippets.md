## Walkthrough the DOM
All methods and properties of accessing the DOM that you have in the browser's JavaScript are available.
```c#
using System.Linq;
using Knyaz.Optimus;
using Knyaz.Optimus.Dom.Elements;
using Console = System.Console;

namespace ConsoleApplication1
{
	class Program
	{
		static async void Main(string[] args)
		{
			var engine = EngineBuilder.New().UseJint().Build();
			var page = await engine.OpenUrl("http://google.com");

			Console.WriteLine("The first document child node is: " + engine.Document.FirstChild);
			Console.WriteLine("The first document body child node is: " + engine.Document.Body.FirstChild);
			Console.WriteLine("The first element tag name is: " + engine.Document.ChildNodes.OfType<HtmlElement>().First().TagName);
			Console.WriteLine("Whole document innerHTML length is: " + engine.Document.DocumentElement.InnerHTML.Length);

			Console.ReadKey();
		}
	}
}
```


## Wait for dynamic elements.
There are methods that allow waiting for the appearance of DOM elements, which are created dynamically in scripts.
```c#
using System.Linq;
using System.Threading.Tasks;
using Knyaz.Optimus;
using Knyaz.Optimus.TestingTools;

namespace Sample
{
	class Program
	{
		public static async Task GetHtml5ScoreTest()
		{
			var engine = EngineBuilder.New().UseJint().Build()

			//Open Html5Test site
			var page = await engine.OpenUrl("https://html5test.com");

			//Wait until it finishes the test of browser and get DOM element with score value.
			var tagWithValue = page.Document.WaitSelector("#score strong").FirstOrDefault();

			//Show result
			System.Console.WriteLine("Score: " + tagWithValue.InnerHTML);
		}
	}
}
```

## Using of proxy
```c#
//create web proxy object
var webProxy = new WebProxy(yor_proxy_address, true);

var engine = EngineBuilder.New()
    //initialize resource provider with the proxy
    .ConfigureResourceProvider(r => r.Http(http => http.Proxy(webProxy))) 
    .Build();
```

## Open sites with basic authorization
```c#
var engine = EngineBuilder.New()
    .ConfigureResourceProvider(r => r.Http(http.Basic(login, password))) 
    .Build();

```

## Get the status code of the page
```c#
var page = await EngineBuilder.New().UseJint().Build().OpenUrl(value);
if (page is HttpPage httpPage && httpPage.HttpStatusCode == HttpStatusCode.Unauthorized)
{
//do something
}
```
## Form submit

Suppose that you have a form, for example this:

```html
<form>
  <input placeholder="Username" id="username" type="text">
  <input placeholder="Password" id="password" type="password">
  <input value="Logon" type="submit" id="clickme">
</form>
```

Easy way to submit this form is in the code below:


```c#
var page = await EngineBuilder.New().UseJint().Build().OpenUrl("http://yoursite.here");
((HtmlInputElement)page.Document.GetElementById("Username")).Value = "John";
((HtmlInputElement)page.Document.GetElementById("Password")).Value = "123456";
((HtmlInputElement)page.Document.GetElementById("clickme")).Click();

```

### Entering of text

The previous example shows how to send a simple form. It will work if your site uses standard send mechanisms. But, if the form is burdened with javascript this method may not work. You may need to emulate the manual input of text with the excitation of all the necessary events in the DOM. For this, there is a special extension method  -
 'EnterText'.


```c#
var page = await EngineBuilder.New().UseJint().Build().OpenUrl("http://yoursite.here");
((HtmlInputElement)page.Document.GetElementById("Username")).EnterText("John");
((HtmlInputElement)page.Document.GetElementById("Password")).EnterText("123456");
((HtmlInputElement)page.Document.GetElementById("clickme")).Click();

```

## Go to another page

You can call the OpenUrl method as many times as you like. Each time a call is made, a new Document instance will be created, but all cookies will be saved. In this way, you can emulate the user's transition through links within a single site.


```c#
var engine = EngineBuilder.New().UseJint().Build();
var mainPage = await engine.OpenUrl("http://yoursite.here");
//... Do something, for example, submit the login form on the main page.
var subPage = await engine.OpenUrl("http://yoursite.here/someresource");
// or if you have some 'a' element on the site
engine.OpenUrl(engine.Document.GetElementsByTagName("a").First().GetAttribute("href"));
```

## Files downloading

Here are a few ways to download a file.

#### Download the file by url


This method is suitable if the page from which you want to download the file contains direct links to these files, for example: 

```html
 <a id=download href="file.txt" download>Download</a>
```


You just need to get the link from the document and load the file in any accessible way, for example:


```c#
var engine = EngineBuilder.New().UseJint().Build();
var page = engine.OpenUrl("http://testpage.net/").Wait();
var download = page.Document.GetElementById("download");
var href = download.GetAttribute("href");
var fileLink = new Uri(engine.Uri, href);
//you can get cookies using engine.CookieContainer
using (var client = new WebClient())
    System.Console.Write(client.DownloadString(fileLink));
```

### Download the file initiated by window.open call
Some pages may contain the buttons or other elements like this:

```html
<button type="submit" onclick="window.open('file.doc')">Download!</button>
```

In browser click on such a button leads to downloading the file from the url specified by first argument of function window.open. There is Engine.OnWindowOpen event to handle such calls:


```c#
var engine = EngineBuilder.New().UseJint().Build();
engine.OnWindowOpen += (url, name, options) => {
   // download the file, from url using code from samples above.
};

var page = await engine.OpenUrl("http://site.net");

var button = page.Document.GetElementById("download") as HtmlElement;
button.Click();
```

## Pre-define JavaScript functions
Some sites may use the JS functions that does not supported by Optimus. Most often, this is an obsolete API parts. You can execute your script that defines missing functions.

```c#
var engine = EngineBuilder.New().UseJint().Build();
engine.ScriptExecutor.Execute("text/javascript", "function escape(str){return encodeURI(str)};");
var page = await engine.OpenUrl("http://localhost", false);//second parameter should be 'false'.
```

## Handle request and response
If you want to modify the request before sent (add cookies, headers) you should use Notify method of ResourceProviderBuilder

```c#
var engine = EngineBuilder.New().ConfigureResourceProvider(x => x.Notify(
    request => { /*modify request here */},
    response => { /*handle response */}
)).Build();
```