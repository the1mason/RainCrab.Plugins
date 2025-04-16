using RainCrab.Plugins.Base;

namespace RainCrab.Plugins.AspNet;

public interface IWebPlugin
{
    Task PreConfigureAsync(IPluginLoadContext<IWebPlugin> loaderContext);
    Task ConfigureAsync(IPluginLoadContext<IWebPlugin> loaderContext);
    Task PostConfigureAsync(IPluginLoadContext<IWebPlugin> loaderContext);
}