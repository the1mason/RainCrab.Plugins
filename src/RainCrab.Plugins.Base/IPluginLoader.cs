namespace RainCrab.Plugins.Base;

public interface IPluginLoader<TPlugin>
{
    Task<IReadOnlyList<TPlugin>> LoadAsync();
}