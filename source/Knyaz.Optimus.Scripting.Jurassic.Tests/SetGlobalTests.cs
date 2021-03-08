using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Scripting.Jurassic;
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
		
		[TestCase("new Image(5.5, 3.3)", 5, 3)]
		[TestCase("new Image(5, 6)", 5, 6)]
		[TestCase("new Image(5)", 5, 0)]
		[TestCase("new Image()", 0, 0)]
		[TestCase("new Image(5.5, 3.3, 'hello')", 5, 3)]
		[TestCase("new Image(5.5, 'hello')", 5, 0)]
		[TestCase("new Image('5', '7')", 5, 7)]
		public static void DefineCtorFunction(string code, int expectedWidth, int expectedHeight)
		{
			var global = new TestingGlobal();
			var jsEngine = new JurassicJsEngine(global);
			var img = (Img)jsEngine.Evaluate(code);
			Assert.That(img.Width, Is.EqualTo(expectedWidth), "Width");
			Assert.That(img.Height, Is.EqualTo(expectedHeight), "Height");
		}
	}
}