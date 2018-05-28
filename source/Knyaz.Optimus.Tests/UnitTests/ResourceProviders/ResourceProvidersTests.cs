using System;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.Tools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.UnitTests.ResourceProviders
{
	[TestFixture]
	public class ResourceProvidersTests
	{
		[Test, Ignore("For manual run")]
		public void HttpRequest() =>
			new ResourceProvider().GetResourceAsync(new Uri("http://google.com")).Wait();
	}
}