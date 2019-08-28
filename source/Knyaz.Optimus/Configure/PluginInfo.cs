namespace Knyaz.Optimus.Configure
{
    public class PluginInfo
    {
        public PluginInfo(string name, string description, string filename, string version, PluginMimeTypeInfo[] mimeTypes)
        {
            Name = name;
            Description = description;
            Filename = filename;
            Version = version;
            MimeTypes = mimeTypes;
        }
        
        /// <summary>
        /// A human readable description of the plugin.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// The filename of the plugin file.
        /// </summary>
        public readonly string Filename;

        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The plugin's version number string.
        /// </summary>
        public readonly string Version;
        
        public readonly PluginMimeTypeInfo[] MimeTypes;
    }

    public class PluginMimeTypeInfo
    {
        public PluginMimeTypeInfo(string type, string description, string suffixes)
        {
            Type = type;
            Description = description;
            Suffixes = suffixes;
        }
        
        /// <summary>
        /// Returns the MIME type of the associated plugin.
        /// </summary>
        public string Type { get; }
        
        /// <summary>
        /// Returns a description of the associated plugin or an empty string if there is none.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// A string containing valid file extensions for the data displayed by the plugin, or an empty string if an extension is not valid for the particular module. For example, a browser's content decryption module may appear in the plugin list but support more file extensions than can be anticipated. It might therefore return an empty string.
        /// </summary>
        public string Suffixes { get; }
    }
}