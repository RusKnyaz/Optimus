
<div align="right">
  <details>
    <summary >🌐 Language</summary>
    <div>
      <div align="center">
        <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=en">English</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=zh-CN">简体中文</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=zh-TW">繁體中文</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=ja">日本語</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=ko">한국어</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=hi">हिन्दी</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=th">ไทย</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=fr">Français</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=de">Deutsch</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=es">Español</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=it">Italiano</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=ru">Русский</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=pt">Português</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=nl">Nederlands</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=pl">Polski</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=ar">العربية</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=fa">فارسی</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=tr">Türkçe</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=vi">Tiếng Việt</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=id">Bahasa Indonesia</a>
        | <a href="https://openaitx.github.io/view.html?user=RusKnyaz&project=Optimus&lang=as">অসমীয়া</
      </div>
    </div>
  </details>
</div>

# Optimus

Optimus is headless Web Browser fully implemented on .net.

[Release notes](CHANGELOG.md)

## Downloads

 - Public release versions are available on [NuGet](https://www.nuget.org/packages/Knyaz.Optimus).
 - Unstable versions are available on the appveyour nuget feed here: [https://ci.appveyor.com/nuget/optimus-private](https://ci.appveyor.com/nuget/optimus-private)

## Samples
 - Quick start
 
 Create console application and paste the code:
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
			var engine = EngineBuilder.New()
			    .UseJint()// Enable JavaScripts execution.
			    .Build(); // Builds the Optimus engine.
        
			//Request the web page.
			var page = await engine.OpenUrl("http://google.com");
			//Get the document
			var document = page.Document;
			//Get DOM items
			Console.WriteLine("The first document child node is: " + document.FirstChild);
			Console.WriteLine("The first document body child node is: " + document.Body.FirstChild);
			Console.WriteLine("The first element tag name is: " + document.ChildNodes.OfType<HtmlElement>().First().TagName);
			Console.WriteLine("Whole document innerHTML length is: " + document.DocumentElement.InnerHTML.Length);
			Console.ReadKey();
		}
	}
}
```


 - [More code snippets](docs/snippets.md)
 - [Prime](https://github.com/RusKnyaz/Prime) - the web browser based on Optimus and Optimus.Graphics.

## Projects structure

 - Knyaz.Optimus - The main assembly with implementation of web browser (without GUI and rendering).
 - Knyaz.Optimus.Tests - Acceptance tests library.
 - Knyaz.Optimus.JsTests - Simple Js tests. 
 - Knyaz.Optimus.UnitTests - Pure unit tests.
 - Knyaz.Optimus.WfApp - Simple test application with DOM explorer and Log window.
 
## License

Optimus is released under the [MIT license](https://raw.githubusercontent.com/RusKnyaz/Optimus/develop/LICENSE.txt).
