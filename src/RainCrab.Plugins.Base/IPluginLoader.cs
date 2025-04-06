namespace RainCrab.Plugins.Base;

public interface IPluginLoader<TPlugin>
{
    Task<IPluginLoadContext<TPlugin>> LoadAsync();
}