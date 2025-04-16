using Microsoft.AspNetCore.Builder;
using RainCrab.Plugins.Base;

namespace RainCrab.Plugins.AspNet;

public class WebApplicationLoadContext(WebApplicationBuilder builder) : IPluginLoadContext<IWebPlugin>
{
    public IReadOnlyList<IWebPlugin> Loaded { get; }
}