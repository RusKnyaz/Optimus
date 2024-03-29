﻿using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlFormElementTests
	{
		private HtmlDocument _document;
		private HtmlFormElement _form;

		[SetUp]
		public void SetUp()
		{
			_document = new HtmlDocument();
			_form = (HtmlFormElement)_document.CreateElement("form");
		}

		[TestCase("GeT", "get")]
		[TestCase("get", "get")]
		[TestCase("", "get")]
		[TestCase("update", "get")]
		[TestCase("PoSt", "post")]
		public void MethodTest(string setValue, string getValue)
		{
			_form.Method = setValue;
			Assert.AreEqual(setValue, _form.GetAttribute("method"));
			Assert.AreEqual(getValue, _form.Method);
		}
	}
}