namespace RainCrab.Plugins.Base;

public interface IUnloadableTypeLoader
{
    bool Unloadable { get; }
    PluginUnloadResult TryUnload();
}