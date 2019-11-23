using System;
using System.IO;
using Knyaz.Optimus.Configure;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.ResourceProviders
{
    /// <summary>
    /// Configures and builds <see cref="Engine"/>;
    /// </summary>
    public class EngineBuilder
    {
        private IResourceProvider _resourceProvider;
        private bool _computedStylesEnabled = false;
        private WindowConfig _windowConfig;

        private Func<ScriptExecutionContext, IScriptExecutor> _getScriptExecutor;
        
        public static EngineBuilder New() => new EngineBuilder();
        
        public EngineBuilder SetResourceProvider(IResourceProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
            return this;
        }

        /// <summary>
        /// Enables computed styles evaluation. Styles evaluation is disabled by default.
        /// </summary>
        public EngineBuilder EnableCss()
        {
	        _computedStylesEnabled = true;
	        return this;
        }

        public EngineBuilder ConfigureResourceProvider(Action<ResourceProviderBuilder> configure)
        {
	        var builder = new ResourceProviderBuilder();
	        configure(builder);
	        _resourceProvider = builder.Build();
	        return this;
        }

        public EngineBuilder Window(Action<WindowConfig> configure)
        {
            _windowConfig = new WindowConfig();
            configure(_windowConfig);
            return this;
        }

        public EngineBuilder JsScriptExecutor(Func<ScriptExecutionContext, IJsScriptExecutor> getScriptExecutor)
        {
	        _getScriptExecutor = context => new ScriptExecutor(() => getScriptExecutor(context));
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

        public Engine Build()
        {
            var window = BuildWindow();
            var engineKeeper = new Engine[1];
            
            Request CreateRequest(string uri, string method = "GET")
            {
                var req = new Request(method, engineKeeper[0].LinkProvider.MakeUri(uri));
                req.Headers["User-Agent"] = window.Navigator.UserAgent;
                req.Cookies = engineKeeper[0].CookieContainer;
                return req;
            }
            
            var exeCtx = new ScriptExecutionContext(
                window,
                parseJson => new XmlHttpRequest(_resourceProvider, () => window.Document, window.Document, CreateRequest, parseJson));
            
            var scriptExecutor = _getScriptExecutor?.Invoke(exeCtx);
            return engineKeeper[0] = new Engine(_resourceProvider, window , scriptExecutor) {
	            ComputedStylesEnabled = _computedStylesEnabled
            }; 
        }
        
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

    public class ScriptExecutionContext
    {
        public ScriptExecutionContext(IWindowEx window, Func<Func<Stream, object>, XmlHttpRequest> createXhr)
        {
            Window = window;
            CreateXhr = createXhr;
        }

        public IWindowEx Window { get; }

        public Func<Func<Stream, object>, XmlHttpRequest> CreateXhr { get; }
        
    }
}