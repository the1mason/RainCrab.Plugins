using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RainCrab.Plugins.Base;

namespace RainCrab.Plugins.AspNet.Extensions;

public static class WebPluginExtensions
{
    public static async Task<WebPluginLoadContext> AddWebPlugins(this WebApplicationBuilder appBuilder, ILogger logger, string? path = null, JsonSerializerOptions? jsonOptions = null)
    {
        jsonOptions ??= JsonSerializerOptions.Default;
        var loader = new TypeLoader<IWebPlugin>(path ?? AppContext.BaseDirectory, jsonOptions, logger, unloadable:false);
        var plugins = await loader.LoadAsync();
        appBuilder.Services.AddSingleton(plugins);
        var loadContext = new WebPluginLoadContext
        {
            ApplicationBuilder = appBuilder,
            LoadedPlugins = plugins
        };
        
        foreach (var plugin in plugins)
            await plugin.ConfigureAsync(loadContext);
        
        appBuilder.Services.TryAddSingleton(appBuilder);
        appBuilder.Services.TryAddSingleton(loader);
        
        return loadContext;
    }
}