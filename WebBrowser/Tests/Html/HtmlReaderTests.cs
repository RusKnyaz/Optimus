#if NUNIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using WebBrowser.Html;

namespace WebBrowser.Tests.Html
{
	[TestFixture]
	public class HtmlReaderTests
	{
		private IEnumerable<HtmlChunk> Read(string str)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
			{
				return HtmlReader.Read(stream).ToList();
			}
		}

		[TestCase("Hello", "Text:Hello")]
		[TestCase("<!--Hello-->", "Comment:Hello")]
		[TestCase("<a></a>", "TagStart:a, TagEnd:a")]
		[TestCase("<a>Hello</a>", "TagStart:a, Text:Hello, TagEnd:a")]
		[TestCase("<a ></a>", "TagStart:a, TagEnd:a")]
		[TestCase("<br/>", "TagStart:br, TagEnd:br")]
		[TestCase("<a />", "TagStart:a, TagEnd:a")]
		[TestCase("<a/><p/>", "TagStart:a, TagEnd:a, TagStart:p, TagEnd:p")]
		[TestCase("<a href=\"http://x.x\"/>", "TagStart:a, AttributeName:href, AttributeValue:http://x.x, TagEnd:a")]
		[TestCase("<a href=\'http://x.x\'/>", "TagStart:a, AttributeName:href, AttributeValue:http://x.x, TagEnd:a")]
		[TestCase("<span data-bind = '\"'/>", "TagStart:span, AttributeName:data-bind, AttributeValue:\", TagEnd:span")]
		[TestCase("<img src=\"\\\\\"></img>", "TagStart:img, AttributeName:src, AttributeValue:\\, TagEnd:img")]
		[TestCase("<div data-bind=\"template:\\\"itemTemplate\\\"\"></div>", "TagStart:div, AttributeName:data-bind, AttributeValue:template:\"itemTemplate\", TagEnd:div")]
		[TestCase("<option value='1' selected>A</option>", "TagStart:option, AttributeName:value, AttributeValue:1, AttributeName:selected, Text:A, TagEnd:option")]
		[TestCase("<option selected>A</option>", "TagStart:option, AttributeName:selected, Text:A, TagEnd:option")]
		[TestCase("<tr><td></td></tr>", "TagStart:tr, TagStart:td, TagEnd:td, TagEnd:tr", Description = "Nested tags")]
		[TestCase("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">", "DocType:HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\"")]
		[TestCase("<!DOCTYPE html>", "DocType:html")]
		[TestCase("<h1>Header1</h1><h2>Header2</h2>", "TagStart:h1, Text:Header1, TagEnd:h1, TagStart:h2, Text:Header2, TagEnd:h2")]
		[TestCase("<p>Hello '<b>World</b>'!</p>", "TagStart:p, Text:Hello ', TagStart:b, Text:World, TagEnd:b, Text:'!, TagEnd:p")]
		[TestCase("<script>for (var i = 0; i < tokens.length - 1; i++) target = target[tokens[i]];</script>", "TagStart:script, Text:for (var i = 0; i < tokens.length - 1; i++) target = target[tokens[i]];, TagEnd:script")]
		[TestCase("<script>var a = x > 5;</script>", "TagStart:script, Text:var a = x > 5;, TagEnd:script")]
		[TestCase("<!-- [opa <i>aa</i>] -->", "Comment: [opa <i>aa</i>] ", Description = "Comment with tags inside")]
		[TestCase("<!-- ko foreach: Peoples --><div data-bind='template:\"itemTemplate\"'/><!-- /ko -->", "Comment: ko foreach: Peoples , TagStart:div, AttributeName:data-bind, AttributeValue:template:\"itemTemplate\", TagEnd:div, Comment: /ko ")]
		[TestCase("<script><div></div></script>", "TagStart:script, Text:<div></div>, TagEnd:script")]
		[TestCase("<script><div></div></script><script>alert('a');</script>", "TagStart:script, Text:<div></div>, TagEnd:script, TagStart:script, Text:alert('a');, TagEnd:script")]
		[TestCase("<script type='text/html'><div></div></script>", "TagStart:script, AttributeName:type, AttributeValue:text/html, Text:<div></div>, TagEnd:script")]
		[TestCase("<script>console.log(\"\\\"</script>\\\"\");</script>", "TagStart:script, Text:console.log(\"\\\"</script>\\\"\");, TagEnd:script")]
		public void ReadString(string source, string expectedChunkTypesString)
		{
			var result = Read(source).ToArray();
			
			Assert.AreEqual(expectedChunkTypesString, string.Join(", ", result.Select(x => x.Type+":"+x.Value).ToArray()));
		}
	}
}

#endif
