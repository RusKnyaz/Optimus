using System;
using Knyaz.NUnit.AssertExpressions;
using Knyaz.Optimus.Tools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Tools
{
	[TestFixture]
	public class CookieParserTests
	{
		[Test]
		public void ExpirationParse() =>
			CookieParser.FromString("username=op; Expires=Wed, 03 Jan 2018 14:54:50 GMT")
				.Assert(cookie => 
					cookie.Name == "username" &&
					cookie.Value == "op" &&
					cookie.Expires == new DateTime(2018, 01, 3, 14,54,50, DateTimeKind.Utc));
			
	}
}