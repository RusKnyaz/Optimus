using System.IO;

namespace Knyaz.Optimus.Tools
{
    /// <summary>
    /// Helper class to work with Stream.
    /// </summary>
    public static class StreamExtension
    {
        /// <summary>
        /// Reads entire Stream to the string.
        /// </summary>
        public static string ReadToEnd(this Stream stream)
        {
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}