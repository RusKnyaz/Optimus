using Knyaz.Optimus.ScriptExecuting.Jint;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.ScriptExecuting
{
	[TestFixture]
	public class JintJsEngineTests
	{
		public interface IStringObjectOverload
		{
			void Log(object obj);
			void Log(string obj);
		}

		[Test]
		public static void ObjPreferableForInt()
		{
			var global = new Mock<IStringObjectOverload>();
			var engine = new JintJsEngine(global.Object);
			engine.Execute("log(1)");
			global.Verify(x => x.Log(1d), Times.Once);
		}

		public interface IParamsMethods
		{
			void Method1(params object[] obj);
			void Method2(int val, params object[] obj);
			
			int Method3(params object[] data);
		}
		
		[Test]
		public static void ParamsMethodCallNoArgs()
		{
			var global = new Mock<IParamsMethods>();
			var engine = new JintJsEngine(global.Object);
			engine.Execute("method1()");
			global.Verify(x => x.Method1());
		}

		[TestCase("1", 1d)]
		[TestCase("'23'", "23")]
		public static void ParamsMethodCallOneArg(string arg, object expected)
		{
			var global = new Mock<IParamsMethods>();
			var engine = new JintJsEngine(global.Object);
			engine.Execute($"method1({arg})");
			global.Verify(x => x.Method1(expected));
		}
		
		[Test]
		public static void ParamsMethodCallTwoArg()
		{
			var global = new Mock<IParamsMethods>();
			var engine = new JintJsEngine(global.Object);
			engine.Execute("method1(1,'2')");
			global.Verify(x => x.Method1(1d, "2"));
		}

		[Test]
		public static void ParamsAndFixedArgument()
		{
			var global = new Mock<IParamsMethods>();
			var engine = new JintJsEngine(global.Object);
			engine.Execute("method2(1,'23')");
			global.Verify(x => x.Method2(1, "23"));
		}
		
		[Test]
		public static void ParamsWithReturnValue()
		{
			var global = new Mock<IParamsMethods>();
			var engine = new JintJsEngine(global.Object);
			engine.Execute("method3('23')");
			global.Verify(x => x.Method3("23"));
		}
		
		public class C
		{
			public C()
			{
				
			}
			
			public string Name;
		}
		
		public interface IOverloads
		{
			void Method(C arg1);

			void Method();

			void Method(bool arg1);
		}

		[Test]
		public void OverloadCallBool()
		{
			var global = new Mock<IOverloads>();
			var engine = new JintJsEngine(global.Object);
			engine.Execute("method(true)");
			global.Verify(x => x.Method(true));
		}
		
		[Test]
		public void OverloadCallStringToBool()
		{
			var global = new Mock<IOverloads>();
			var engine = new JintJsEngine(global.Object);
			engine.Execute("method('false')");
			global.Verify(x => x.Method(true));
		}

		[Test]
		public void OverloadCallNoArgs()
		{
			var global = new Mock<IOverloads>();
			var engine = new JintJsEngine(global.Object);
			engine.Execute("method()");
			global.Verify(x => x.Method());
		}

		[Test]
		public void OverloadCallObj()
		{
			var global = new Mock<IOverloads>();
			var engine = new JintJsEngine(global.Object);
			engine.Execute("method({name:'x'})");
			global.Verify(x => x.Method(It.IsAny<C>()));
		}
	}
}