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
			var request = Mock.Of<IRequest>(x => x.Url == new Uri("http://google.com"));
			var httpMock = new Mock<ISpecResourceProvider>();
			httpMock.Setup(x => x.CreateRequest(It.IsAny<Uri>())).Returns(request);
			var target = new ResourceProvider(httpMock.Object, null);

			target.GetResourceAsync(new Uri("http://google.com")).Wait();
			
			httpMock.Verify(x => x.CreateRequest(new Uri("http://google.com")), Times.Once());
			httpMock.Verify(x => x.SendRequestAsync(request), Times.Once());
		}
	}
}