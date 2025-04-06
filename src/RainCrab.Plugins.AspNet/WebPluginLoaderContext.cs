using System.Collections.Immutable;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using RainCrab.Plugins.Base;

namespace RainCrab.Plugins.AspNet;

public class WebPluginLoaderContext(IReadOnlyList<IWebPlugin> loadedPlugins) : IPluginLoadContext<IWebPlugin>
{
    // do not cast it back to List
    // Or do
    // I'm not your daddy
    public IReadOnlyList<IWebPlugin> Loaded => loadedPlugins;
    public IHostApplicationBuilder? ApplicationBuilder { get; }

    public void SetBuilder(IHostApplicationBuilder builder)
    {
        
    }
}