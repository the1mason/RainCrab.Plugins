using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RainCrab.Plugins.Base;

namespace RainCrab.Plugins.AspNet;

public sealed class WebPluginLoader(string basePath, JsonSerializerOptions jsonSerializerOptions, ILogger logger) : IPluginLoader<IWebPlugin>
{
    private readonly List<IWebPlugin> _plugins = [];
    private List<Manifest> _manifests = [];
    
    public async Task<IPluginLoadContext<IWebPlugin>> Load(string basePath)
    {
        var manifestLoader = new ManifestLoader(basePath, jsonSerializerOptions, logger);
        var manifestLoadResults = await manifestLoader.LoadManifestsAsync();

        if (manifestLoadResults.Count == 0)
            return new WebPluginLoaderContext([]);

        foreach (var manifestLoadResult in manifestLoadResults)
        {
            try
            {
                var plugin = LoadPlugin(manifestLoadResult);
                _plugins.Add(plugin);
                _manifests.Add(manifestLoadResult.Manifest);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to load {PluginName}{Version}: {Message}", manifestLoadResult.Manifest.Id,manifestLoadResult.Manifest.Version, e.Message);
                _manifests.Remove(manifestLoadResult.Manifest);
            }
        }
        
        return new WebPluginLoaderContext(_plugins);
    }

    private static IWebPlugin LoadPlugin(ManifestLoadResult manifestLoadResult)
    {
        var path = Directory.GetParent(manifestLoadResult.Location) + "/" + manifestLoadResult.Manifest.Assembly;

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Plugin assembly file does not exist.", path);
        }

        var loadContext = new PluginAssemblyLoadContext(path);
        Assembly pluginAssembly =
            loadContext.LoadFromAssemblyName(
                new AssemblyName(Path.GetFileNameWithoutExtension(manifestLoadResult.Manifest.Assembly)));
        var plugin = CreatePlugin(pluginAssembly);

        if (plugin is null)
            throw new ApplicationException("Failed to create plugin instance.");
        
        return plugin;
    }
    
    private static IWebPlugin? CreatePlugin(Assembly assembly)
    {
        var types = assembly.GetTypes().Where(t => typeof(IWebPlugin).IsAssignableFrom(t)).ToArray();
        return types.Length switch
        {
            > 1 => throw new ApplicationException(
                $"Found more than 1 type, implementing IPlugin in assembly {assembly.FullName}"),
            < 1 => throw new ApplicationException(
                $"Found more than 1 type, implementing IPlugin in assembly {assembly.FullName}"),
            _ => Activator.CreateInstance(types.First()) as IWebPlugin
        };
    }
}