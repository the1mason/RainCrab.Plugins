namespace RainCrab.Plugins.Base;

public interface IPluginLoadContext<out TPlugin>
{
    IReadOnlyList<TPlugin> Loaded { get; }
    
}