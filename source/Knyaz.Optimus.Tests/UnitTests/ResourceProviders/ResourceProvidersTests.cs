using System;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.Tools;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.UnitTests.ResourceProviders
{
	[TestFixture]
	public class ResourceProvidersTests
	{
		[Test]
		public void HttpRequest()
		{
			var request = new Request(new Uri("http://google.com"));
			
			var httpMock = new Mock<IResourceProvider>();
			var target = new ResourceProvider(httpMock.Object, null);

			target.SendRequestAsync(request).Wait();
			
			httpMock.Verify(x => x.SendRequestAsync(request), Times.Once());
		}
	}
}