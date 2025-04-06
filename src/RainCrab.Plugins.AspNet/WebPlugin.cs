namespace RainCrab.Plugins.AspNet;

public interface IWebPlugin
{
    Manifest Manifest { get; }
    Task PreConfigureAsync(WebPluginLoaderContext loaderContext);
    Task ConfigureAsync(WebPluginLoaderContext loaderContext);
}