using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Scripting.Jurassic.Tests
{
	/// <summary> Tests how the different argument types converted from js to clr and passed to methods. </summary>
	public static class CallMethodArgs
	{
		public interface IMethods
		{
			void ShortArg(short s);
		}

		public class Global
		{
			public IMethods Methods { get; set; }
		}

		[TestCase("methods.shortArg(5)", 5)]
		[TestCase("methods.shortArg('5')", 5)]
		[TestCase("methods.shortArg(true)", 1)]
		[TestCase("methods.shortArg(false)", 0)]
		public static void PassToShort(string code, short expected)
		{
			var methods = Mock.Of<IMethods>();
			var engine = new JurassicJsEngine(new Global(){ Methods = methods});
			engine.Execute(code);
			Mock.Get(methods).Verify(x => x.ShortArg(expected), Times.Once);
		}
	}
}