using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Scripting.Jint.Internal;
using NUnit.Framework;

namespace Knyaz.Optimus.Scripting.Jint.Tests
{
	[TestFixture]
	public class CollectionsTests
	{
		public class MyCollection : IReadOnlyList<string>
		{
			private readonly string[] _original;

			public MyCollection(string[] original) => _original = original;

			#region IReadOnlyList implementation 
			public IEnumerator<string> GetEnumerator() => _original.Cast<string>().GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			[JsName("length")]
			public int Count => _original.Length;

			public string this[int index] => _original[index];
			#endregion
			
			//Some additional method.

			public string GetHello() => "Hello";
			
			//Additional indexer
			public string this[string s] => new string(s.Reverse().ToArray());

		}
		
		public class Global
		{
			public MyCollection Data => new MyCollection(new[]{"1","2","3","4"});
		}
		
		
		[TestCase("[].slice.call(data).length", ExpectedResult = 4)]
		[TestCase("data.length = 8;data.length", ExpectedResult = 4)]
		[TestCase("data.getHello()", ExpectedResult = "Hello")]
		[TestCase("data instanceof MyCollection", ExpectedResult = true)]
		[TestCase("data['Hello']", ExpectedResult = "olleH")]
		public static object ReadonlyCollectionTests(string code)
		{
			var engine = new JintJsEngine(new Global());
			engine.AddGlobalType(typeof(MyCollection));
			return engine.Evaluate(code);
		}
	}
}