using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Knyaz.Optimus.Tests.Performance
{
	public class TestData
	{
		private static byte[] DeepHtmlBytes;
		private static byte[] LongHtmlBytes;
		private static byte[] BalancedHtmlBytes;
		private static byte[] AttributesHtmlBytes;

		static TestData()
		{
			DeepHtmlBytes = Encoding.UTF8.GetBytes(GenerateHtml(1000, 0, 1));
			LongHtmlBytes = Encoding.UTF8.GetBytes(GenerateHtml(1, 0, 1000));
			BalancedHtmlBytes = Encoding.UTF8.GetBytes(GenerateHtml(10, 10, 10));
			AttributesHtmlBytes = Encoding.UTF8.GetBytes(GenerateHtml(1, 1000, 1));
		}
		
		public static Stream LongHtml => new MemoryStream(LongHtmlBytes);
		public static Stream DeepHtml => new MemoryStream(DeepHtmlBytes);
		public static Stream BalancedHtml => new MemoryStream(BalancedHtmlBytes);
		public static Stream AttributesHtml => new MemoryStream(AttributesHtmlBytes);
		
		public static Stream LargeHtml = typeof(HtmlReaderPerfTests).Assembly.GetManifestResourceStream(
			"Knyaz.Optimus.Tests.Resources.Large_Html.txt");
		
		private static string GenerateHtml(int deep, int attrs, int count)
		{
			var tag =
				"<div " +
				String.Join(" ", Enumerable.Range(0, attrs).Select(x => "attr" + x + "=" + "val" + x))
				+">";

			var deepTag = string.Join("", Enumerable.Range(0, deep).Select(x => tag))
			              + string.Join("", Enumerable.Range(0, deep).Select(x => "/<div>"));

			return string.Join("", Enumerable.Range(0, count).Select(x => deepTag));
		}
	}
}