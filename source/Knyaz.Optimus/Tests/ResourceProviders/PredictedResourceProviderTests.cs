#if NUNIT
using System.Threading.Tasks;
using Knyaz.Optimus.ResourceProviders;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.ResourceProviders
{
	[TestFixture]
	public class PredictedResourceProviderTests
	{
		[Test]
		public void PreloadedResource()
		{
			var request = Mock.Of<IRequest>(
				x=> x.Equals(It.IsAny<object>()) == true &&
				x.GetHashCode() == 1);

			var provider = Mock.Of<IResourceProvider>(x =>
				x.CreateRequest("some uri") == request &&
				x.SendRequestAsync(It.IsAny<IRequest>()) == new Task<IResource>(() => Mock.Of<IResource>()));

			var target = new PredictedResourceProvider(provider);
			target.Preload("some uri");

			Mock.Get(provider).Verify(x => x.SendRequestAsync(It.IsAny<IRequest>()), Times.Once);

			target.GetResourceAsync("some uri");

            Mock.Get(provider).Verify(x => x.SendRequestAsync(It.IsAny<IRequest>()), Times.Once);
		}

		[Test]
		public void GetNonPreloadedResource()
		{
			var request = Mock.Of<IRequest>(
            				x=> x.Equals(It.IsAny<object>()) == true &&
            				x.GetHashCode() == 1);

			var provider = Mock.Of<IResourceProvider>(x =>
					x.CreateRequest("some uri") == request &&
					x.SendRequestAsync(It.IsAny<IRequest>()) == new Task<IResource>(() => Mock.Of<IResource>()));

			var target = new PredictedResourceProvider(provider);
			target.GetResourceAsync("some uri");

			Mock.Get(provider).Verify(x => x.CreateRequest("some uri"), Times.Once);
			Mock.Get(provider).Verify(x => x.SendRequestAsync(It.IsAny<IRequest>()), Times.Once);
		}

		[Test]
		public void Clear()
		{
			var request = Mock.Of<IRequest>(
				x => x.Equals(It.IsAny<object>()) == true &&
				     x.GetHashCode() == 1);

			var provider = Mock.Of<IResourceProvider>(x =>
				x.CreateRequest("some uri") == request &&
				x.SendRequestAsync(It.IsAny<IRequest>()) == new Task<IResource>(() => Mock.Of<IResource>()));

			var target = new PredictedResourceProvider(provider);
			target.Preload("some uri");
			Mock.Get(provider).Verify(x => x.SendRequestAsync(It.IsAny<IRequest>()), Times.Once);

			target.Clear();

			target.GetResourceAsync("some uri");

			Mock.Get(provider).Verify(x => x.SendRequestAsync(It.IsAny<IRequest>()), Times.Exactly(2));
		}
	}
}
#endif