using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	partial class HtmlInputElementTests
	{
		private HtmlInputElement GetInput()
		{
			var input = (HtmlInputElement)_document.CreateElement("input");
			input.Type = "number";
			return input;
		}

		[Test]
		public void ValueSanitization()
		{
			var input = GetInput();
			input.SetAttribute("value", "123");
			Assert.AreEqual("123", input.Value);
			input.Value = "ABC";
			Assert.AreEqual(string.Empty, input.Value);
			Assert.AreEqual("123", input.GetAttribute("value"));
		}

		[Test]
		public void ValueFromAttributeSanitization()
		{
			var input = GetInput();
			input.SetAttribute("value", "abc");
			Assert.AreEqual(string.Empty, input.Value);
			Assert.AreEqual("abc", input.GetAttribute("value"));
		}

		[Test]
		public void SetFromAttributeAndStepUp()
		{
			var input = GetInput();
			input.SetAttribute("value", "123");
			input.StepUp();
			Assert.AreEqual("124", input.Value);
		}

		[Test]
		public void StepUp()
		{
			var input = GetInput();
			input.Value = "56";
			input.StepUp();
			Assert.AreEqual("57", input.Value);
		}

		[Test]
		public void StepDown()
		{
			var input = GetInput();
			input.Value = "20";
			input.StepDown();
			Assert.AreEqual("19", input.Value);
		}

		[Test]
		public void StepDownWithArgument()
		{
			var input = GetInput();
			input.Value = "20";
			input.StepDown(3);
			Assert.AreEqual("17", input.Value);
		}

		[Test]
		public void StepDownWithSpecifiedStep()
		{
			var input = GetInput();
			input.Value = "20";
			input.Step = "5";
			input.StepDown();
			Assert.AreEqual("15", input.Value);
		}

		[Test]
		public void StepDownWithMin()
		{
			var input = GetInput();
			input.Value = "5";
			input.Min = "5";
			input.Step = "5";
			input.StepDown();
			Assert.AreEqual("5", input.Value);
		}

		[Test]
		public void StepDownFromMin()
		{
			var input = GetInput();
			input.Value = "21";
			input.Min = "5";
			input.Step = "5";
			input.StepDown();
			Assert.AreEqual("20", input.Value);
		}

		[Test]
		public void StepUpWithArgument()
		{
			var input = (HtmlInputElement)_document.CreateElement("input");
			input.Type = "number";
			input.Value = "56";
			input.StepUp(10);
			Assert.AreEqual("66", input.Value);
		}

		[Test]
		public void StepWithSpecifiedStep()
		{
			var input = (HtmlInputElement)_document.CreateElement("input");
			input.Type = "number";
			input.Step = "5";
			input.StepUp();
			Assert.AreEqual("5", input.Value);
		}

		[Test]
		public void StepUpWithMin()
		{
			var input = (HtmlInputElement)_document.CreateElement("input");
			input.Type = "number";
			input.Min = "8";
			input.Step = "5";
			input.Value = "20";
			input.StepUp();
			Assert.AreEqual("23", input.Value);
		}

		[Test]
		public void StepUpWithMinMax()
		{
			var input = (HtmlInputElement)_document.CreateElement("input");
			input.Type = "number";
			input.Min = "8";
			input.Max = "22";
			input.Step = "5";
			input.Value = "20";
			input.StepUp();
			Assert.AreEqual("18", input.Value);
		}
	}
}