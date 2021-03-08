using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Scripting.Jurassic.Tests
{
	public class CallMethodResult
	{
		public interface IAnotherType
		{
			void Method();
		}
		
		public class AnotherType : IAnotherType
		{
			public void Method() {}
		}
		
		public interface IMethods
		{
			string String();
			string[] StringArray();

			AnotherType[] TypeArray();
		}
		
		public class Global
		{
			public IMethods Methods { get; set; }
		}

		[Test]
		public static void StringArray()
		{
			var methods = Mock.Of<IMethods>(x => x.StringArray() == new[]{"Hi","You"});
			var engine = new JurassicJsEngine(new Global() {Methods = methods});
			var result = engine.Evaluate("methods.stringArray()");
			Assert.AreEqual(new[]{"Hi", "You"}, result);
		}
		
		[Test]
		public static void TypeArray()
		{
			var resultArray = new[] {new AnotherType(), new AnotherType()};
			var methods = Mock.Of<IMethods>(x => x.TypeArray() == resultArray);
			var engine = new JurassicJsEngine(new Global() {Methods = methods});
			var result = engine.Evaluate("methods.typeArray()");
			Assert.AreEqual(resultArray, result);
		}
	}
}