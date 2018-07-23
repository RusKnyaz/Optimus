using System.Threading;
using System.Threading.Tasks;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Tools;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlImageElementTests
	{
		private Document _document;

		[SetUp]
		public void SetUp()
		{
			_document = DomImplementation.Instance.CreateHtmlDocument();
		}
		
		[Test]
		public void ImageOnError()
		{
			_document.GetImage = s => Task.Run(() => (IImage)null);
			
			var img = (HtmlImageElement)_document.CreateElement(TagsNames.Img);
			var errorSignal = new ManualResetEvent(false);
			img.OnError += evt => { errorSignal.Set(); };

			img.Src = "http://noimage.jpg";

			Assert.IsTrue(errorSignal.WaitOne(1000));
		}
		
		[Test]
		public void ImageOnErrorTwice()
		{
			_document.GetImage = s => Task.Run(() => (IImage)null);
			var img = (HtmlImageElement)_document.CreateElement(TagsNames.Img);
			var errorSignal = new ManualResetEvent(false);
			img.OnError += evt => { errorSignal.Set(); };

			img.Src = "http://noimage.jpg";

			Assert.IsTrue(errorSignal.WaitOne(1000), "first error");

			errorSignal.Reset();
			img.Src = "http://noimage.jpg";
			
			Assert.IsTrue(errorSignal.WaitOne(1000), "second error");
		}

		[Test]
		public void ImageOnLoad()
		{
			var imageData = Mock.Of<IImage>(x => x.Width == 8 && x.Height == 4);
			_document.GetImage = s => Task.Run(() => imageData);

			var img = (HtmlImageElement)_document.CreateElement("img");
			var loadSignal = new ManualResetEvent(false);
			img.OnLoad += evt => { loadSignal.Set(); };

			img.Src = "http://image.jpg";

			Assert.IsTrue(loadSignal.WaitOne(1000));
			Assert.AreEqual(8, img.NaturalWidth);
			Assert.AreEqual(4, img.NaturalHeight);
		}
	}
}