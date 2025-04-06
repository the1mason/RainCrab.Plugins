namespace RainCrab.Plugins.Base;

public interface IUnloadablePluginLoader
{
    bool Unloadable { get; }
    PluginUnloadResult TryUnload();
}