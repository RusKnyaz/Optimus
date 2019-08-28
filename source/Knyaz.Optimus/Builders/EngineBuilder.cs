using System;
using Knyaz.Optimus.Configure;
using Knyaz.Optimus.Environment;

namespace Knyaz.Optimus.ResourceProviders
{
    /// <summary>
    /// Configures and builds <see cref="Engine"/>;
    /// </summary>
    public class EngineBuilder
    {
        private IResourceProvider _resourceProvider;
        //private Window _window;
        private WindowConfig _windowConfig;

        public EngineBuilder()
        {
            
        }
        
        public EngineBuilder SetResourceProvider(IResourceProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
            return this;
        }

        public EngineBuilder Window(Action<WindowConfig> configure)
        {
            _windowConfig = new WindowConfig();
            configure(_windowConfig);
            return this;
        }

        private Window BuildWindow()
        {
            var navigatorPlugins = new NavigatorPlugins(_windowConfig?._plugins ?? new PluginInfo[0]);
            
            var navigator = new Navigator(navigatorPlugins)
            {
                UserAgent =
                    $"{System.Environment.OSVersion.VersionString} Optimus {GetType().Assembly.GetName().Version.Major}.{GetType().Assembly.GetName().Version.MajorRevision}"
            };
            
            var windowKeeper = new Window[1];
            
            return windowKeeper[0] = new Window(() => windowKeeper[0].Engine.Document, _windowConfig?._windowOpenHandler, navigator);
        }

        public Engine Build() => new Engine(_resourceProvider, BuildWindow());

        public class WindowConfig
        {
            internal PluginInfo[] _plugins;
            internal Action<string, string, string> _windowOpenHandler;
            
            public WindowConfig SetNavigatorPlugins(PluginInfo[] plugins)
            {
                _plugins = plugins;
                return this;
            }

            public WindowConfig SetWindowOpenHandler(Action<string, string, string> windowOpenHandler)
            {
                _windowOpenHandler = windowOpenHandler;
                return this;
            }
        }
    }
}