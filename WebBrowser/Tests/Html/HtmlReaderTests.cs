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
		[TestCase("<a/>", "TagStart:a, TagEnd:a")]
		[TestCase("<a />", "TagStart, TagEnd")]
		[TestCase("<option selected>A</option>", "TagStart, AttributeName, Text, TagEnd")]
		[TestCase("<a/><p/>", "TagStart:a, TagEnd:a, TagStart:p, TagEnd:p")]
		[TestCase("<a href=\"http://x.x\"/>", "TagStart:a, AttributeName:href, AttributeValue:http://x.x, TagEnd")]
		[TestCase("<a href=\'http://x.x\'/>", "TagStart:a, AttributeName:href, AttributeValue:http://x.x, TagEnd")]
		[TestCase("<span data-bind = '\"'/>", "TagStart:span, AttributeName:data-bind, AttributeValue:\", TagEnd")]
		[TestCase("<option value='1' selected>A</option>", "TagStart:option, AttributeName:value, AttributeValue:1, AttributeName:selected, Text:A, TagEnd")]
		[TestCase("<tr><td></td></tr>", "TagStart:tr, TagStart:td, TagEnd:td, TagEnd:tr", Description = "Nested tags")]
		[TestCase("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">", "DocType")]
		[TestCase("<!DOCTYPE html>", "DocType:html")]
		[TestCase("<h1>Header1</h1><h2>Header2</h2>", "TagStart:h1, Text:Header1, TagEnd:h1, TagStart:h2, Text:Header2, TagEnd:h2")]
		[TestCase("<p>Hello '<b>World</b>'!</p>", "TagStart:p, Text:Hello ', TagStart:b, Text:World, TagEnd:b, Text:'!, TagEnd:p")]
		[TestCase("<script>for (var i = 0; i < tokens.length - 1; i++) target = target[tokens[i]];</script>", "TagStart:script, Text:for (var i = 0; i < tokens.length - 1; i++) target = target[tokens[i]];, TagEnd:script")]
		[TestCase("<script>var a = x > 5;</script>", "TagStart:script, Text:var a = x > 5;, TagEnd:script")]
		[TestCase("<!-- [opa <i>aa</i>] -->", "Comment: [opa <i>aa</i>] ", Description = "Comment with tags inside")]
		[TestCase("<!-- ko foreach: Peoples --><div data-bind='template:\"itemTemplate\"'/><!-- /ko -->", "Comment, TagStart, AttributeName, AttributeValue, TagEnd, Comment")]
		[TestCase("<script><div></div></script>", "TagStart:script, Text:<div></div>, TagEnd:script")]
		[TestCase("<script><div></div></script><script>alert('a');</script>", "TagStart:script, Text:<div></div>, TagEnd:script, TagStart:script, Text:alert('a');, TagEnd:script")]
		[TestCase("<script type='text/html'><div></div></script>", "TagStart:script, AttributeName:type, AttributeValue:text/html, Text:<div></div>, TagEnd:script")]
		public void ReadString(string source, string expectedChunkTypesString)
		{
			var expectedChunkTypes =
				expectedChunkTypesString.Split(',').Select(x => (HtmlChunkTypes)Enum.Parse(typeof(HtmlChunkTypes), x.Split(':')[0].Trim())).ToArray();
			var result = Read(source).ToArray();
			CollectionAssert.AreEqual(expectedChunkTypes, result.Select(x => x.Type).ToArray());

			var expectedChunkValues =
				expectedChunkTypesString.Split(',').Select(x =>
					{
						var a = x.Split(':');
						return a.Length > 1 ? string.Join(":", a.Skip(1)) : null;
					}).ToArray();

			for (var i = 0; i < expectedChunkValues.Length; i++)
			{
				var expectedValue = expectedChunkValues[i];
				if (expectedValue != null)
				{
					Assert.AreEqual(expectedValue, result[i].Value, result[i].Type.ToString());
				}
			}
		}

		[TestCase("<a/ >")]
		public void InvalidFormat(string source)
		{
			Assert.Throws<HtmlInvalidFormatException>(() => Read(source).ToArray());
		}
	}
}

#endif
