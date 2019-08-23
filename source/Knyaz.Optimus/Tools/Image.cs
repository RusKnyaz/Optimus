using System;
using System.IO;
using System.Linq;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Tools
{
	interface IImage
	{
		int Width { get; }
		int Height { get; }
	}

	[DomItem]
	internal class Image : IImage
	{
		private Lazy<Tuple<int, int>> _size; 
		
		public Image(string mimeType, Stream data)
		{
			_size = new Lazy<Tuple<int, int>>(() => GetSize(mimeType, data));
		}

		Tuple<int, int> GetSize(string mimeType, Stream data)
		{
			if (mimeType == "image/bmp")
			{
				using (var reader = new BinaryReader(data))
				{
					// Simplified Windows BMP Bitmap File Format Specification:
					// http://www.dragonwins.com/domains/GetTechEd/bmp/bmpfileformat.htm

					reader.ReadBytes(18);
					int pixelWidth = reader.ReadInt32();
					int pixelHeight = reader.ReadInt32();
					return new Tuple<int, int>(pixelWidth, pixelHeight);
				}
			}
			
			if(mimeType =="image/png")
			{
				using (var reader = new BinaryReader(data))
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
	}
}