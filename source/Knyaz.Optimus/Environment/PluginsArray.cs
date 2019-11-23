using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Configure;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Environment
{
    public class PluginsArray : NamedItemsStore<Plugin>
    {
        public PluginsArray(IDictionary<string, Plugin> items) : base(items)
        {
        }
        
        /// <summary>
        /// Updates the lists of supported plugins and MIME types for this page, and reloads the page if the lists have changed.
        /// </summary>
        public void Refresh(){}
    }

    public class Plugin: INamedItemsReadonlyStore<MimeType>
    {
        private readonly PluginInfo _pluginInfo;
        private readonly INavigatorPlugins _plugins;

        public Plugin(PluginInfo pluginInfo, INavigatorPlugins plugins) 
        {
            _pluginInfo = pluginInfo;
            _plugins = plugins;
        }

        /// <summary>
        /// A human readable description of the plugin.
        /// </summary>
        public string Description => _pluginInfo.Description;

        /// <summary>
        /// The filename of the plugin file.
        /// </summary>
        public string Filename => _pluginInfo.Filename;

        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public string Name => _pluginInfo.Name;

        /// <summary>
        /// The plugin's version number string.
        /// </summary>
        public string Version => _pluginInfo.Version;

        public MimeType this[int idx] =>
            idx < 0 || idx >= _pluginInfo.MimeTypes.Length ? null : _plugins.MimeTypes[_pluginInfo.MimeTypes[idx].Type];

        public MimeType this[string name] =>
            int.TryParse(name, out var index) ? this[index] :
            _pluginInfo.MimeTypes.Any(x => x.Type == name) ? _plugins.MimeTypes[name] : null;
        
        public int Length => _pluginInfo.MimeTypes.Length;
    }
}