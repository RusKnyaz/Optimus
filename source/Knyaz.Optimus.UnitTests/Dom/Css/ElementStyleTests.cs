﻿using System.IO;
using Knyaz.NUnit.AssertExpressions;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Interfaces;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom.Css
{
	[TestFixture]
	public class ElementStyleTests
	{
		private IWindow _window;
		private HtmlDocument _document;
		HtmlDivElement _div;

		[SetUp]
		public void SetUp()
		{
			_window = Mock.Of<IWindow>();
			_document = new HtmlDocument(_window);
			_div = (HtmlDivElement) _document.CreateElement("DIV");
		}

		[Test]
		public void CssTextFromStyle()
		{
			_div.SetAttribute("style", "background-color:Red");
			_div.Style.Assert(style => 
				style.CssText == "background-color:Red"
				&& style.Length == 1);
		}

		[Test]
		public void DefaultValues()
		{
			_div.Style.Assert(style => 
				style.CssText == "" 
				&& style.ParentRule == null
				&& style.Length == 0);
		}

		[Test]
		public void DefaultComputedStyle()
		{
			var defaultStyleSheet = StyleSheetBuilder.CreateStyleSheet(new StringReader("DIV {display:block;font-size:16px;}"), s => null);
			var styling = new DocumentStyling(_document, defaultStyleSheet, null);
			
			_document.Body.AppendChild(_div);

			styling.GetComputedStyle(_div).Assert(style =>
				style.GetPropertyValue("display") == "block" &&
				style.GetPropertyValue("font-size") == "16px");
		}

		[Test]
		public void SetStyleUsingSetAttribute()
		{
			_div.SetAttribute("style","width:100pt");

			_div.Style.Assert(style =>
				style.Length == 1 &&
				(string)style["width"] == "100pt" &&
				style.GetPropertyValue("width") == "100pt" &&
				style[0] == "width");
		}


		[Test]
		public void SetStyleUsingAttributeNode()
		{
			var attr = _document.CreateAttribute("style");
			attr.Value = "width:100pt";

			_div.SetAttributeNode(attr);

			_div.Style.Assert(style =>
				style.Length == 1 &&
                (string)style["width"] == "100pt" &&
				style.GetPropertyValue("width") == "100pt" &&
				style[0] == "width");
		}

		[Test]
		public void SetStyle()
		{
			_div.Style["width"] = "10pt";
			Assert.AreEqual("10pt", _div.Style["width"]);
			Assert.AreEqual("width:10pt",_div.GetAttribute("style"));
		}

		[Test]
		public void SetCssText()
		{
			_div.Style.CssText = "width:10pt";
			Assert.AreEqual("10pt", _div.Style["width"]);
			Assert.AreEqual("width:10pt", _div.GetAttribute("style"));
		}
	}
}