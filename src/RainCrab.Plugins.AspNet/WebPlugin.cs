using RainCrab.Plugins.Base;

namespace RainCrab.Plugins.AspNet;

public interface IWebPlugin
{
    public Manifest Manifest { get; }
}