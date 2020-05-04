using System;
using System.Threading.Tasks;
using Knyaz.Optimus.ResourceProviders;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.ResourceProviders
{
	[TestFixture]
	public class PredictedResourceProviderTests
	{
		string SomeUri = "http://some.some";
		
		[Test]
		public void PreloadedResource()
		{
			var provider = Mock.Of<IResourceProvider>(x =>
				x.SendRequestAsync(It.IsAny<Request>()) == new Task<IResource>(() => Mock.Of<IResource>()));

			var target = new PredictedResourceProvider(provider);
			target.Preload(new Request(new Uri(SomeUri)));

			Mock.Get(provider).Verify(x => x.SendRequestAsync(It.IsAny<Request>()), Times.Once);

			target.SendRequestAsync(new Request(new Uri(SomeUri)));

            Mock.Get(provider).Verify(x => x.SendRequestAsync(It.IsAny<Request>()), Times.Once);
		}

		[Test]
		public void GetNonPreloadedResource()
		{
			var provider = Mock.Of<IResourceProvider>(x =>
					x.SendRequestAsync(It.IsAny<Request>()) == new Task<IResource>(() => Mock.Of<IResource>()));

			var target = new PredictedResourceProvider(provider);
			target.SendRequestAsync(new Request(new Uri(SomeUri)));

			Mock.Get(provider).Verify(x => x.SendRequestAsync(It.IsAny<Request>()), Times.Once);
		}

		[Test]
		public void Clear()
		{
			var provider = Mock.Of<IResourceProvider>(x =>
				x.SendRequestAsync(It.IsAny<Request>()) == new Task<IResource>(() => Mock.Of<IResource>()));

			var target = new PredictedResourceProvider(provider);
			target.Preload(new Request(new Uri(SomeUri)));
			Mock.Get(provider).Verify(x => x.SendRequestAsync(It.IsAny<Request>()), Times.Once);

			target.Clear();

			target.SendRequestAsync(new Request(new Uri(SomeUri)));

			Mock.Get(provider).Verify(x => x.SendRequestAsync(It.IsAny<Request>()), Times.Exactly(2));
		}
	}
}