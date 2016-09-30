#if NUNIT
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Tools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Tools
{
	[TestFixture]
	public class CachedEnumerableTests
	{
		[SetUp]
		public void SetUp()
		{
			i = 0;
		}

		int i;

		IEnumerable<int> GetNumbers(int cnt)
		{
			for(var x = 0; x < cnt;x++)
			{
				yield return i;
				i++;
			}
		}

		[Test]
		public void EnumerateOnceTest()
		{
			var target = new CachedEnumerable<int>(GetNumbers(3));
			Assert.AreEqual(new[] {0,1,2}, target.ToArray());
		}

		[Test]
		public void EnumerateTwiceTest()
		{
			var target = new CachedEnumerable<int>(GetNumbers(3));
			Assert.AreEqual(new[] { 0, 1, 2 }, target.ToArray());
			Assert.AreEqual(new[] { 0, 1, 2 }, target.ToArray());
		}

		[Test]
		public void EnumerateTwiceNotAllTest()
		{
			var target = new CachedEnumerable<int>(GetNumbers(3));
			Assert.AreEqual(new[] { 0, 1 }, target.Take(2).ToArray());
			Assert.AreEqual(new[] { 0, 1 }, target.Take(2).ToArray());
		}
	}
}
#endif