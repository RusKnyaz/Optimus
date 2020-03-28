using System;
using System.IO;
using System.Linq;

namespace Knyaz.Optimus.Tools
{
	public interface IImage
	{
		int Width { get; }
		int Height { get; }
		
		Stream Data { get; }
	}

	internal class Image : IImage
	{
		private Lazy<Tuple<int, int>> _size;

		private byte[] _raw;
		
		public Image(string mimeType, Stream data)
		{
			var tmp = new MemoryStream();
			data.CopyTo(tmp);
			_raw = tmp.ToArray();
				
			_size = new Lazy<Tuple<int, int>>(() => GetSize(mimeType));
		}

		Tuple<int, int> GetSize(string mimeType)
		{
			if (mimeType == "image/bmp")
			{
				using (var reader = new BinaryReader(Data))
				{
					// Simplified Windows BMP Bitmap File Format Specification:
					// http://www.dragonwins.com/domains/GetTechEd/bmp/bmpfileformat.htm

					reader.ReadBytes(18);
					int pixelWidth = reader.ReadInt32();
					int pixelHeight = reader.ReadInt32();
					
					Data.Seek(0, SeekOrigin.Begin);
					return new Tuple<int, int>(pixelWidth, pixelHeight);
				}
			}
			
			if(mimeType =="image/png")
			{
				using (var reader = new BinaryReader(Data))
				{
					var ihdrLength = reader.ReadInt32();
					reader.ReadBytes(12); // chunk type ("IHDR")

					int pixelWidth = BitConverter.ToInt32(reader.ReadBytes(4).Reverse().ToArray(),0);
					int pixelHeight = BitConverter.ToInt32(reader.ReadBytes(4).Reverse().ToArray(),0);
					return new Tuple<int, int>(pixelWidth, pixelHeight);
				}
			}
			
			throw new NotImplementedException();	
		}

		public int Width => _size.Value.Item1;

		public int Height => _size.Value.Item2;
		public Stream Data => new MemoryStream(_raw);
	}
}