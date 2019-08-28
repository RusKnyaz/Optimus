using System.Collections.Generic;
using Knyaz.Optimus.Configure;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Environment
{
    public class NavigatorPlugins : INavigatorPlugins
    {
        public NavigatorPlugins(PluginInfo[] pluginsInfos)
        {
            var mimeTypes = new Dictionary<string, MimeType>();
            var plugins = new Dictionary<string, Plugin>();
            
            foreach (var pluginInfo in pluginsInfos)
            {
                var plugin = new Plugin(pluginInfo, this);
                foreach (var mimeTypeInfo in pluginInfo.MimeTypes)
                {
                    var mimeType = new MimeType(mimeTypeInfo, plugin);
                    mimeTypes.Add(mimeTypeInfo.Type, mimeType);
                }

                plugins.Add(plugin.Name, plugin);
            }
            
            MimeTypes = new MimeTypesArray(mimeTypes);
            Plugins = new PluginsArray(plugins);
        }
        
        public MimeTypesArray MimeTypes { get; }
        public PluginsArray Plugins { get; }
    }
}