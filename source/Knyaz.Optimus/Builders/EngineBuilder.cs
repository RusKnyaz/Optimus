using System;
using Knyaz.Optimus.Configure;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus
{
    /// <summary>
    /// Configures and builds <see cref="Engine"/>;
    /// </summary>
    public class EngineBuilder
    {
        private IResourceProvider _resourceProvider;
        private DocumentStylesConfiguration _stylesConfig;
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
            _stylesConfig = new DocumentStylesConfiguration();
            return this;
        }

        public EngineBuilder EnableCss(Action<DocumentStylesConfiguration> configure)
        {
            _stylesConfig = new DocumentStylesConfiguration();
            configure?.Invoke(_stylesConfig);
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

        private Window BuildWindow(Func<Engine> getEngine)
        {
            var navigatorPlugins = new NavigatorPlugins(_windowConfig?._plugins ?? new PluginInfo[0]);
            
            var navigator = new Navigator(navigatorPlugins)
            {
                UserAgent =
                    $"{System.Environment.OSVersion.VersionString} Optimus {GetType().Assembly.GetName().Version.Major}.{GetType().Assembly.GetName().Version.MajorRevision}"
            };
            
            var windowKeeper = new Window[1];
            
            return windowKeeper[0] = new Window(
                () => windowKeeper[0].Engine.Document, 
                _windowConfig?._windowOpenHandler, 
                navigator, 
                _windowConfig?._console ?? NullConsole.Instance, 
                getEngine);
        }

        public Engine Build()
        {
            var resourceProvider = _resourceProvider ?? new ResourceProviderBuilder().UsePrediction().Http()
                .Build();
            
            var engineKeeper = new Engine[1];
            
            var window = BuildWindow(() => engineKeeper[0]);
            
            var exeCtx = new ScriptExecutionContext(window);
            
            var scriptExecutor = _getScriptExecutor?.Invoke(exeCtx);

            var docStyling = 
                _stylesConfig == null ? null 
                    : (Func<Document, DocumentStyling>)(doc => new DocumentStyling(
                        doc, 
                        _stylesConfig?.UserAgentStyleSheet, 
                        s => resourceProvider.SendRequestAsync(engineKeeper[0].CreateRequest(s))));
            
            return engineKeeper[0] = new Engine(resourceProvider, window , scriptExecutor, docStyling); 
        }


        public class WindowConfig
        {
            internal PluginInfo[] _plugins;
            internal Action<string, string, string> _windowOpenHandler;
            internal IConsole _console;
            
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

            public WindowConfig SetConsole(IConsole console)
            {
                _console = console;
                return this;
            }
        }
    }
}

namespace Knyaz.Optimus.ResourceProviders
{
}