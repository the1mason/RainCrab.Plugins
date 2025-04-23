namespace RainCrab.Plugins.AspNet;

public interface IWebPlugin
{
    Task ConfigureAsync(WebPluginLoadContext loadContext);
    Task ShutdownAsync(WebPluginLoadContext loadContext);
}