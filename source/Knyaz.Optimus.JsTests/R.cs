using System;
using System.IO;

namespace Knyaz.Optimus.JsTests
{
	class R
	{
		public static string GetString(string file)
		{
			using (var reader = new StreamReader(typeof(R).Assembly.GetManifestResourceStream(file)
			                                     ?? throw new ArgumentException($"Resource not found {file}")))
			{
				return reader.ReadToEnd();
			}
		}
	}
}