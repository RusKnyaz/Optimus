using Knyaz.Optimus.Environment;

namespace Knyaz.Optimus.Dom.Interfaces
{
    public interface INavigatorPlugins
    {
        MimeTypesArray MimeTypes { get; }
        PluginsArray Plugins { get; }
    }
}