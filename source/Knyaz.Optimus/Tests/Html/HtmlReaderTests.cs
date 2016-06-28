#if NUNIT
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Knyaz.Optimus.Html;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Html
{
	[TestFixture]
	public class HtmlReaderTests
	{
		//Doctype
		[TestCase("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">", "DocType:HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\"")]
		[TestCase("<!DOCTYPE html>", "DocType:html")]
		//Text
		[TestCase("Hello", "Text:Hello")]
		//Comments
		[TestCase("<!--Hello-->", "Comment:Hello")]
		[TestCase("<!-Hello-->", "Comment:-Hello--")]
		[TestCase("<?import a>", "Comment:?import a")]
		[TestCase("<?--import a-->", "Comment:?--import a--")]
		[TestCase("<!-- [opa <i>aa</i>] -->", "Comment: [opa <i>aa</i>] ", Description = "Comment with tags inside")]
		[TestCase("<!-- ko foreach: Peoples --><div data-bind='template:\"itemTemplate\"'/><!-- /ko -->", "Comment: ko foreach: Peoples , TagStart:div, AttributeName:data-bind, AttributeValue:template:\"itemTemplate\", TagEnd:div, Comment: /ko ")]
		[TestCase("<div><!--Com--></div>","TagStart:div, Comment:Com, TagEnd:div")]
		//Tags
		[TestCase("<a></a>", "TagStart:a, TagEnd:a")]
		[TestCase("<a>Hello</a>", "TagStart:a, Text:Hello, TagEnd:a")]
		[TestCase("<a ></a>", "TagStart:a, TagEnd:a")]
		[TestCase("<br/>", "TagStart:br, TagEnd:br")]
		[TestCase("<a />", "TagStart:a, TagEnd:a")]
		[TestCase("<a/><p/>", "TagStart:a, TagEnd:a, TagStart:p, TagEnd:p")]
		[TestCase("<tr><td></td></tr>", "TagStart:tr, TagStart:td, TagEnd:td, TagEnd:tr", Description = "Nested tags")]
		[TestCase("<h1>Header1</h1><h2>Header2</h2>", "TagStart:h1, Text:Header1, TagEnd:h1, TagStart:h2, Text:Header2, TagEnd:h2")]
		[TestCase("<p>Hello '<b>World</b>'!</p>", "TagStart:p, Text:Hello ', TagStart:b, Text:World, TagEnd:b, Text:'!, TagEnd:p")]
		[TestCase("<div></div><span></span>", "TagStart:div, TagEnd:div, TagStart:span, TagEnd:span")]
		[TestCase("<d\\iv></d\\iv>", "TagStart:d\\iv, TagEnd:d\\iv")]
		[TestCase("\\<span/>", "Text:\\, TagStart:span, TagEnd:span")]
		//Attributes
		[TestCase("<span /name='a'></span>", "TagStart:span, AttributeName:name, AttributeValue:a, TagEnd:span")]
		[TestCase("<a href=\"http://x.x\"/>", "TagStart:a, AttributeName:href, AttributeValue:http://x.x, TagEnd:a")]
		[TestCase("<a href=\'http://x.x\'/>", "TagStart:a, AttributeName:href, AttributeValue:http://x.x, TagEnd:a")]
		[TestCase("<span data-bind = '\"'/>", "TagStart:span, AttributeName:data-bind, AttributeValue:\", TagEnd:span")]
		[TestCase("<img src=\"\\\\\"></img>", "TagStart:img, AttributeName:src, AttributeValue:\\, TagEnd:img")]
		[TestCase("<div data-bind=\"template:\\\"itemTemplate\\\"\"></div>", "TagStart:div, AttributeName:data-bind, AttributeValue:template:\"itemTemplate\", TagEnd:div")]
		[TestCase("<option value='1' selected>A</option>", "TagStart:option, AttributeName:value, AttributeValue:1, AttributeName:selected, Text:A, TagEnd:option")]
		[TestCase("<option selected>A</option>", "TagStart:option, AttributeName:selected, Text:A, TagEnd:option")]
		[TestCase("<option selected id='dd'>A</option>", "TagStart:option, AttributeName:selected, AttributeName:id, AttributeValue:dd, Text:A, TagEnd:option")]
		[TestCase("<div att1></div>", "TagStart:div, AttributeName:att1, TagEnd:div", Description = "Attribute name can contains digits")]
		[TestCase("<div na\\me='a'></div>", "TagStart:div, AttributeName:na\\me, AttributeValue:a, TagEnd:div")]
		[TestCase("<div name=a\\ b></div>", "TagStart:div, AttributeName:name, AttributeValue:a\\, AttributeName:b, TagEnd:div")]
		[TestCase("<span name=a\\'b></span>", "TagStart:span, AttributeName:name, AttributeValue:a'b, TagEnd:span")]
        [TestCase("<span name=a\\b></span>", "TagStart:span, AttributeName:name, AttributeValue:a\\b, TagEnd:span")]
        [TestCase("<span name=a\\\\b></span>", "TagStart:span, AttributeName:name, AttributeValue:a\\\\b, TagEnd:span")]
		//Scripts
		[TestCase("<script>for (var i = 0; i < tokens.length - 1; i++) target = target[tokens[i]];</script>", "TagStart:script, Text:for (var i = 0; i < tokens.length - 1; i++) target = target[tokens[i]];, TagEnd:script")]
		[TestCase("<script>var a = x > 5;</script>", "TagStart:script, Text:var a = x > 5;, TagEnd:script")]
		[TestCase("<script><div></div></script>", "TagStart:script, Text:<div></div>, TagEnd:script")]
		[TestCase("<script><div></div></script><script>alert('a');</script>", "TagStart:script, Text:<div></div>, TagEnd:script, TagStart:script, Text:alert('a');, TagEnd:script")]
		[TestCase("<script type='text/html'><div></div></script>", "TagStart:script, AttributeName:type, AttributeValue:text/html, Text:<div></div>, TagEnd:script")]
		[TestCase("<script type='text/html'><span /></script>", "TagStart:script, AttributeName:type, AttributeValue:text/html, Text:<span />, TagEnd:script")]
		[TestCase("<script>console.log(\"\\\"</script>\\\"\");</script>", "TagStart:script, Text:console.log(\"\\\"</script>\\\"\");, TagEnd:script")]
		[TestCase("<script>var a = /\\/s*[\"']/g;</script><br/>", "TagStart:script, Text:var a = /\\/s*[\"']/g;, TagEnd:script, TagStart:br, TagEnd:br", Description = "Regex with special char inside script")]
		[TestCase("<script>//\"\r\n</script><br/>", "TagStart:script, Text://\"\r\n, TagEnd:script, TagStart:br, TagEnd:br", Description = "Regex with special char inside script")]
		[TestCase("<script defer/>", "TagStart:script, AttributeName:defer, TagEnd:script")]
		[TestCase("<script>var a = $('<input type=\"file\">')</script>", "TagStart:script, Text:var a = $('<input type=\"file\">'), TagEnd:script")]
		[TestCase("<script defer>var a = $('<input type=\"file\">')</script>", "TagStart:script, AttributeName:defer, Text:var a = $('<input type=\"file\">'), TagEnd:script")]
		[TestCase("<script>/*</script>*/hi();</script>", "TagStart:script, Text:/*</script>*/hi();, TagEnd:script")]
		[TestCase("<script>var a='\\\\';</script>", "TagStart:script, Text:var a='\\\\';, TagEnd:script")]
		//
		[TestCase("<meta><meta>", "TagStart:meta, TagEnd:meta, TagStart:meta, TagEnd:meta", Description = "Unclosed tag")]
		[TestCase("<meta name='viewport'><meta>", "TagStart:meta, AttributeName:name, AttributeValue:viewport, TagEnd:meta, TagStart:meta, TagEnd:meta", Description = "Unclosed tag")]
		[TestCase("<div at=val'></div>","TagStart:div, AttributeName:at, AttributeValue:val', TagEnd:div", Description = "Unquoted attributes values")]
		[TestCase("<textarea><!--</textarea>-->","TagStart:textarea, Text:<!--, TagEnd:textarea, Text:-->")]
		[TestCase("<textAREA><!--</TEXTarea>-->", "TagStart:textAREA, Text:<!--, TagEnd:textAREA, Text:-->")]
		[TestCase("<textarea>/*</textarea>*/", "TagStart:textarea, Text:/*, TagEnd:textarea, Text:*/")]
		[TestCase("<textarea>\"</textarea>\"", "TagStart:textarea, Text:\", TagEnd:textarea, Text:\"")]
		[TestCase("<head>\r\n\t<script>somecode</script></head>", 
			"TagStart:head, Text:\n\n\t, TagStart:script, Text:somecode, TagEnd:script, TagEnd:head")]
		[TestCase("\u000D", "Text:\u000A")]
		[TestCase("<div class=A>1</div>", "TagStart:div, AttributeName:class, AttributeValue:A, Text:1, TagEnd:div")]
		public void ReadString(string source, string expectedChunks)
		{
			Assert.AreEqual(expectedChunks, Read(source));
		}

		//http://www.w3schools.com/html/html_symbols.asp
		[TestCase("&lang;&rang;&amp;", "Text:〈〉&")]
		[TestCase("&euro;", "Text:€", Description = "Currency symbols")]
        [TestCase("<div data='&amp;'></div>", "TagStart:div, AttributeName:data, AttributeValue:&, TagEnd:div", Description = "Symbols in attribute should be translated")]
		[TestCase("<div>&amp;</div>", "TagStart:div, Text:&, TagEnd:div", Description = "Text inside tags should be translated.")]
		[TestCase("<div>\\&amp;</div>", "TagStart:div, Text:\\&, TagEnd:div", Description = "'&' can't be escaped by \\")]
		[TestCase("&amp;amp;", "Text:&amp;")]
		[TestCase("<s&rang;/>", "TagStart:s&rang;, TagEnd:s&rang;", Description = "Tags names should not be translated")]
		[TestCase("<span>&ra</span>", "TagStart:span, Text:&ra, TagEnd:span")]
		[TestCase("<span>&raduga;</span>", "TagStart:span, Text:&raduga;, TagEnd:span", Description = "Unregistered symbol")]
		[TestCase("<option name='&ra' selected/>", "TagStart:option, AttributeName:name, AttributeValue:&ra, AttributeName:selected, TagEnd:option", Description = "Part of symbol")]
		public void SpecialSymbolsTest(string source, string expectedChunks)
		{
			Assert.AreEqual(expectedChunks, Read(source));
		}


		private string Read(string html)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(html)))
			{
				var result = ((IEnumerable<HtmlChunk>) HtmlReader.Read(stream).ToList()).ToArray();
				return string.Join(", ", result.Select(x => x.Type + ":" + x.Value).ToArray());
			}
		}
	}
}

#endif
