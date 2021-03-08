using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Scripting.Jint.Internal;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Scripting.Jint.Tests
{
	[TestFixture]
	public class SetGlobalTests
	{
		class Img
		{
			public readonly int Width;
			public readonly int Height;
			
			public Img(int w, int h)
			{
				Width = w;
				Height = h;
			}
		}

		class TestingGlobal
		{
			[JsCtor("Image")]
			public Img NewImage(int? w = null, int? h = null) => new Img(w ?? 0, h ?? 0);
		}

		[Test]
		public static void DefineCtorFunction()
		{
			var jsEngine = new JintJsEngine(new TestingGlobal());
			Assert.AreEqual("function", jsEngine.Evaluate("typeof Image"));
		}

		[TestCase("new Image(5.5, 3.3)", 5, 3)]
		[TestCase("new Image(5, 6)", 5, 6)]
		[TestCase("new Image(5)", 5, 0)]
		[TestCase("new Image()", 0, 0)]
		[TestCase("new Image(5.5, 3.3, 'hello')", 5, 3)]
		[TestCase("new Image(5.5, 'hello')", 5, 0)]
		public static void CallCtorFunction(string code, int expectedWidth, int expectedHeight)
		{
			var global = new TestingGlobal();
			var jsEngine = new JintJsEngine(global);
			var img = (Img)jsEngine.Evaluate(code);
			Assert.AreEqual(img.Width, expectedWidth, "Width");
			Assert.AreEqual(img.Height, expectedHeight, "Height");
		}
	}
}