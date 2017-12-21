#if NUNIT
using System.Linq;
using Knyaz.Optimus.Tools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests
{
	[TestFixture]
	public class IEnumerableExtensionTests
	{
		[Test]
		public void ToListOrNullReturnsNullForEmptyEnum()
		{
			var x = new string[0].ToListOrNull();
			Assert.IsNull(x);
		}

		[Test]
		public void ToListOrNullReturnsList()
		{
			var x = new string[] {"ABC"};
			Assert.AreEqual(1, x.ToListOrNull().Count);
		}
	}
}
#endif