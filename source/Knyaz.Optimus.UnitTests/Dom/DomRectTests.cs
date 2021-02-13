using System.Drawing;
using Knyaz.NUnit.AssertExpressions;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public static class DomRectTests
	{
		[Test]
		public static void InitByRectangle() => 
			DomRect.FromRectangleF(new RectangleF(1, 3, 5, 7))
				.Assert(domRect => domRect.X == 1 &&
				                   domRect.Y == 3 &&
				                   domRect.Width == 5 &&
				                   domRect.Height == 7 &&
				                   domRect.Left == 1 &&
				                   domRect.Top == 3 &&
				                   domRect.Right == 6 &&
				                   domRect.Bottom == 10);
		
		[Test]
		public static void InitByRectangleWithNegativeSize() =>
			DomRect.FromRectangleF(new RectangleF(10, 30, -5, -7))
				.Assert(domRect => domRect.X == 10 &&
				                   domRect.Y == 30 &&
				                   domRect.Width == -5 &&
				                   domRect.Height == -7 &&
				                   domRect.Left == 5 &&
				                   domRect.Top == 23 &&
				                   domRect.Right == 10 &&
				                   domRect.Bottom == 30);
	}
}