using System.Collections.Generic;
using NUnit.Framework;

namespace Knyaz.Optimus.Scripting.Jurassic.Tests
{
	[TestFixture]
	public class IndexerTests
	{
		public class ObjectIndexGet
		{
			[System.Runtime.CompilerServices.IndexerName("_Item")]
			public string this[object key] => $"Hello, {key ?? "<null>"}";
		}
		
		[TestCase("data[0]", ExpectedResult = "Hello, 0")]
		[TestCase("data['x']", ExpectedResult = "Hello, x")]
		public string AccessObjectIndex(string code)
		{
			var engine = new JurassicJsEngine(new {Data=new ObjectIndexGet()});
			return (string)engine.Evaluate(code);
		}

		public class ObjectIndexGetSet
		{
			private Dictionary<object, string> _data = new Dictionary<object, string>();

			public string this[object key]
			{
				get => _data[key];
				set => _data[key] = value;
			}
		}
		
		[Test, Ignore("#2. To be fixed.")]
		public static void SetIndexer()
		{
			var obj = new ObjectIndexGetSet();
			var engine = new JurassicJsEngine(new {Data=obj});
			engine.Execute("data[0] = 'hello'");
			Assert.AreEqual("hello", obj[0]);
		}
	}
}