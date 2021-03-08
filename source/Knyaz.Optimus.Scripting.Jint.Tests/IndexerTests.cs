using Knyaz.Optimus.Scripting.Jint.Internal;
using NUnit.Framework;

namespace Knyaz.Optimus.Scripting.Jint.Tests
{
	[TestFixture]
	public class IndexerTests
	{
		public class ObjectIndex
		{
			[System.Runtime.CompilerServices.IndexerName("_Item")]
			public string this[object key]
			{
				get { return $"Hello, {key ?? "<null>"}"; }
			}
		}

		[TestCase("data[0]", ExpectedResult = "Hello, 0")]
		[TestCase("data['x']", ExpectedResult = "Hello, x")]
		public string AccessObjectIndex(string code)
		{
			var engine = new JintJsEngine(new {Data=new ObjectIndex()});
			return (string)engine.Evaluate(code);
		}
	}
}