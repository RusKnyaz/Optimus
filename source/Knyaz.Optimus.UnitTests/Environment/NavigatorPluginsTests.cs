using Knyaz.Optimus.Configure;
using Knyaz.Optimus.Environment;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Environment
{
    [TestFixture]
    public class NavigatorPluginsTests
    {
        [Test]
        public void Init()
        {
            var plugins = new [] {
                new PluginInfo("Pdf reader", "Pdf document reader", "", "",
                    new PluginMimeTypeInfo[] {new PluginMimeTypeInfo("application/pdf", "", "pdf")}),
                new PluginInfo("Video plugin", "", "", "", new PluginMimeTypeInfo[]
                {
                    new PluginMimeTypeInfo("application/mpeg","","mpg"),
                    new PluginMimeTypeInfo("application/avi","","avi"),
                }), 
            };
            
            new NavigatorPlugins(plugins).Assert(navigatorPlugin => 
                navigatorPlugin.Plugins.Length == 2 &&
                navigatorPlugin.MimeTypes.Length == 3 &&
                navigatorPlugin.Plugins[0].Name == "Pdf reader" &&
                navigatorPlugin.Plugins[1].Name == "Video plugin" &&
                navigatorPlugin.Plugins["Pdf reader"].Name == "Pdf reader" &&
                navigatorPlugin.Plugins[1].Length == 2 &&
                navigatorPlugin.Plugins[1][0].Type == "application/mpeg" &&
                navigatorPlugin.Plugins[1]["application/avi"].Suffixes == "avi" &&
                navigatorPlugin.Plugins[3] == null);
        }
    }
}