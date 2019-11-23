using System.Collections.Generic;
using Knyaz.Optimus.Configure;

namespace Knyaz.Optimus.Environment
{
    public class MimeTypesArray : NamedItemsStore<MimeType>
    {
        public MimeTypesArray(IDictionary<string, MimeType> items) : base(items)
        {
        }
    }

    public class MimeType
    {
        private readonly PluginMimeTypeInfo _mimeTypeInfo;
        private readonly Plugin _plugin;

        public MimeType(PluginMimeTypeInfo  mimeTypeInfo, Plugin plugin)
        {
            _mimeTypeInfo = mimeTypeInfo;
            _plugin = plugin;
        }

        /// <summary>
        /// Returns the MIME type of the associated plugin.
        /// </summary>
        public string Type => _mimeTypeInfo.Type;

        /// <summary>
        /// Returns a description of the associated plugin or an empty string if there is none.
        /// </summary>
        public string Description => _mimeTypeInfo.Description;

        /// <summary>
        /// A string containing valid file extensions for the data displayed by the plugin, or an empty string if an extension is not valid for the particular module. For example, a browser's content decryption module may appear in the plugin list but support more file extensions than can be anticipated. It might therefore return an empty string.
        /// </summary>
        public string Suffixes => _mimeTypeInfo.Suffixes;

        /// <summary>
        /// Returns an instance of Plugin containing information about the plugin itself.
        /// </summary>
        public Plugin EnabledPlugin => _plugin;
    }
}