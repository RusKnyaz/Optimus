using Knyaz.Optimus.ScriptExecuting;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Scripting.Jurassic.Tests
{
	[TestFixture]
	public class JurassicJsEngineTests
	{
		private static JurassicJsEngine GetEngine() => new JurassicJsEngine(null);
		
		public interface IStringObjectOverload
		{
			void Log(object obj);
			void Log(string obj);
		}

		[Test]
		public static void ObjPreferableForInt()
		{
			var global = new Mock<IStringObjectOverload>();
			var engine = new JurassicJsEngine(global.Object);
			engine.Execute("log(1)");
			global.Verify(x => x.Log(1), Times.Once);
		}

		public interface IParamsMethods
		{
			void Method1(params object[] obj);
			void Method2(int val, params object[] obj);
			
			int Method3(params object[] data);

			void Method4(bool val, params object[] objs);
		}
		
		[Test]
		public static void ParamsMethodCallBoolAndArgs()
		{
			var global = new Mock<IParamsMethods>();
			var engine = new JurassicJsEngine(global.Object);
			engine.Execute("method4(true)");
			global.Verify(x => x.Method4(true));
		}
		
		[Test]
		public static void ParamsMethodCallNoArgs()
		{
			var global = new Mock<IParamsMethods>();
			var engine = new JurassicJsEngine(global.Object);
			engine.Execute("method1()");
			global.Verify(x => x.Method1());
		}

		[TestCase("1", 1)]
		[TestCase("'23'", "23")]
		public static void ParamsMethodCallOneArg(string arg, object expected)
		{
			var global = new Mock<IParamsMethods>();
			var engine = new JurassicJsEngine(global.Object);
			engine.Execute($"method1({arg})");
			global.Verify(x => x.Method1(expected));
		}
		
		[Test]
		public static void ParamsMethodCallTwoArg()
		{
			var global = new Mock<IParamsMethods>();
			var engine = new JurassicJsEngine(global.Object);
			engine.Execute("method1(1,'2')");
			global.Verify(x => x.Method1(1, "2"));
		}

		[Test]
		public static void ParamsAndFixedArgument()
		{
			var global = new Mock<IParamsMethods>();
			var engine = new JurassicJsEngine(global.Object);
			engine.Execute("method2(1,'23')");
			global.Verify(x => x.Method2(1, "23"));
		}
		
		[Test]
		public static void ParamsWithReturnValue()
		{
			var global = new Mock<IParamsMethods>();
			var engine = new JurassicJsEngine(global.Object);
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
			var engine = new JurassicJsEngine(global.Object);
			engine.Execute("method(true)");
			global.Verify(x => x.Method(true));
		}
		
		[Test]
		public void OverloadCallStringToBool()
		{
			var global = new Mock<IOverloads>();
			var engine = new JurassicJsEngine(global.Object);
			engine.Execute("method('false')");
			global.Verify(x => x.Method(true));
		}

		[Test]
		public void OverloadCallNoArgs()
		{
			var global = new Mock<IOverloads>();
			var engine = new JurassicJsEngine(global.Object);
			engine.Execute("method()");
			global.Verify(x => x.Method());
		}

		[Test]
		public void OverloadCallObj()
		{
			var global = new Mock<IOverloads>();
			var engine = new JurassicJsEngine(global.Object);
			engine.Execute("method({name:'x'})");
			global.Verify(x => x.Method(It.IsAny<C>()));
		}

		public interface IOverloadWithParams
		{
			void Warn(params object[] objs);
			void Warn(string format, params object[] objs);
		}

		[Test]
		public void CallWarn()
		{
			var global = new Mock<IOverloadWithParams>();
			var engine = new JurassicJsEngine(global.Object);
			engine.Execute("warn('hello')");
			global.Verify(x => x.Warn("hello"));
		}

		[Test]
		public void CallWarnObject()
		{
			var global = new Mock<IOverloadWithParams>();
			var engine = new JurassicJsEngine(global.Object);
			engine.Execute("warn(this)");
			global.Verify(x => x.Warn(global.Object));
		}

		public class MyType
		{
			public static readonly int BytesPerElement = 1;
		}
		
		[Test]
		public void StaticField()
		{
			var engine = GetEngine();
			engine.AddGlobalType(typeof(MyType));
			var result = engine.Evaluate("new MyType().BytesPerElement");
			Assert.AreEqual(1, result);
		}

		public class Ctors { public Ctors(int x){}}

		[Test]
		public void CallCtorInt()
		{
			var engine = GetEngine();
			engine.AddGlobalType(typeof(Ctors));
			var result = engine.Evaluate("new Ctors(5)");
			Assert.IsInstanceOf<Ctors>(result);
		}
		
		public class UlongCtor{public UlongCtor(ulong x){}}
		
		[Test]
		public void CallCtorUlong()
		{
			var engine = new JurassicJsEngine(null);
			engine.AddGlobalType(typeof(UlongCtor));
			var result = engine.Evaluate("new UlongCtor(5)");
			Assert.IsInstanceOf<UlongCtor>(result);
		}

		public class NullableCtor
		{
			public int CalledCtorNum = 0;
			public NullableCtor(int y) => CalledCtorNum = 1;
			public NullableCtor(byte[] x) => CalledCtorNum = 2;
		}

		[Test]
		public void CallCtorWithNull()
		{
			var engine = GetEngine();
			engine.AddGlobalType(typeof(NullableCtor));
			var result = engine.Evaluate("new NullableCtor(null)");
			Assert.IsInstanceOf<NullableCtor>(result);
			Assert.AreEqual(2, ((NullableCtor)result).CalledCtorNum);
		}

		

		class JsonStream : IRawJson
		{
			public string JsonString { get; set; }
		}
		
		class JsonProvider
		{
			public IRawJson GetJson() => new JsonStream(){JsonString = "{\"data\":\"hello\"}"};
		}
		
		[Test]
		public static void ParseJson() => 
			Assert.That(new JurassicJsEngine(new JsonProvider()).Evaluate("getJson().data"), Is.EqualTo("hello"));
			
		class OverrideToString
		{
			public override string ToString() => "Hi";
		}
		
		[Test]
		public static void ExposeToString()
		{
			var engine = GetEngine();
			engine.AddGlobalType(typeof(OverrideToString));
			var result = engine.Evaluate("new OverrideToString().toString()");
			Assert.AreEqual("Hi", result);
		}

		

		class ToStringContainer
		{
			public OverrideToString Prop => new OverrideToString();
		}

		
		[Test]
        public static void ExposeToStringFromProperty()
        {
        	var engine = GetEngine();
        	engine.AddGlobalType(typeof(ToStringContainer));
        	var result = engine.Evaluate("new ToStringContainer().prop.toString()");
        	Assert.AreEqual("Hi", result);
        }

        [Test]
        public static void ToStringOfPrototypeWithOverriden()
        {
	        var engine = GetEngine();
	        engine.AddGlobalType(typeof(OverrideToString));
	        var result = engine.Evaluate("Object.getPrototypeOf(new OverrideToString()).toString()");
	        Assert.AreEqual("[object OverrideToString]", result);
        }
        
        [Test]
        public static void ToStringOfPrototype()
        {
	        var engine = GetEngine();
	        engine.AddGlobalType(typeof(TypeWithConst));
	        var result = engine.Evaluate("Object.getPrototypeOf(new TypeWithConst()).toString()");
	        Assert.AreEqual("[object TypeWithConst]", result);
        }

        class TypeWithConst
        {
	        public const int VALUE = 8;
        }
        
        [Test]
        public static void ConstOfPrototype()
        {
	        var engine = GetEngine();
	        engine.AddGlobalType(typeof(TypeWithConst));
	        Assert.AreEqual(8, engine.Evaluate("TypeWithConst.VALUE"), "As static");
	        Assert.AreEqual(8, engine.Evaluate("(new TypeWithConst()).VALUE"), "As object's field");
			Assert.AreEqual(true, engine.Evaluate("Object.getPrototypeOf(new TypeWithConst()).hasOwnProperty('VALUE')"));
	        Assert.AreEqual(
		        8, 
		        engine.Evaluate("Object.getPrototypeOf(new TypeWithConst()).VALUE"),
		        "As prototype's field");
        }

        class TypeWithStatic
        {
	        public static int VALUE = 9;
        }
        
        [Test]
        public static void StaticOfPrototype()
        {
	        var engine = GetEngine();
	        engine.AddGlobalType(typeof(TypeWithStatic));
	        Assert.AreEqual(9, engine.Evaluate("Object.getPrototypeOf(new TypeWithStatic()).VALUE"));
        }

        class TypeWithMethod
        {
	        public int TryCallMe() => 8;
        }

        [Test]
        public static void CallMethodOfPrototype()
        {
	        var engine = GetEngine();
	        engine.AddGlobalType(typeof(TypeWithMethod));
	        Assert.AreEqual(8, engine.Evaluate("new TypeWithMethod().tryCallMe()"), "Call method of object");
	        Assert.IsTrue(
		        engine.Evaluate("Object.getPrototypeOf(new TypeWithMethod()).tryCallMe()")
			        .ToString()
			        .Contains("Illegal method invocation")
		        , "Call method of prototype");
        }

		[JsName("OverridenName")]
		class RenamedClass { }

		[Test]
		public static void DefaultToString()
		{
			var engine = GetEngine();
			engine.AddGlobalType(typeof(RenamedClass));
			var result = engine.Evaluate("new OverridenName().toString()");
			Assert.AreEqual("[object OverridenName]", result);
		}
	}
}