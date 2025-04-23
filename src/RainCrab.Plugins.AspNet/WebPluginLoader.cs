using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RainCrab.Plugins.Base;

namespace RainCrab.Plugins.AspNet;


public sealed class WebPluginLoader(
    string basePath,
    JsonSerializerOptions jsonSerializerOptions,
    ILogger logger,
    bool unloadable)
{
    public bool Unloadable => unloadable;

    private readonly List<(Manifest manifest, IWebPlugin plugin)> _manifestPlugins = [];

    private readonly List<(string manifestId, PluginAssemblyLoadContext context)> _loadedContexts = [];

    public async Task<IReadOnlyList<IWebPlugin>> LoadAsync()
    {
        var manifestLoader = new ManifestLoader(basePath, jsonSerializerOptions, logger);
        var manifestLoadResults = await manifestLoader.LoadManifestsAsync();

        if (manifestLoadResults.Count == 0)
            return [];

        foreach (var manifestLoadResult in manifestLoadResults)
        {
            try
            {
                var (plugin, pluginAssemblyLoadContext) = LoadPlugin(manifestLoadResult);
                _loadedContexts.Add((manifestLoadResult.Manifest.Id, pluginAssemblyLoadContext));
                _manifestPlugins.Add((manifestLoadResult.Manifest, plugin));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to load {PluginName}{Version}: {Message}", manifestLoadResult.Manifest.Id,
                    manifestLoadResult.Manifest.Version, e.Message);
            }
        }

        return _manifestPlugins.OrderByDescending(x => x.manifest.Priority).Select(x => x.plugin).ToArray();
    }

    public PluginUnloadResult TryUnload()
    {
        if (!Unloadable)
            throw new InvalidOperationException("This plugin loader is not unloadable!");

        List<WeakReference> assemblies = [];
        foreach (var idContextTuple in _loadedContexts)
        {
            assemblies.AddRange(idContextTuple.context.Assemblies.Select(x => new WeakReference(x)));
            idContextTuple.context.Unload();
        }

        List<PluginUnloadError> errors = [];

        foreach (var assembly in assemblies)
        {
            if (TryWaitUntilUnloaded(assembly))
                continue;

            errors.Add(new PluginUnloadError("Unknown", "Failed to unload assembly"));
            logger.LogWarning("Failed to unload plugin context!");
        }

        return new PluginUnloadResult(errors.Count > 0, errors);
    }

    private static (IWebPlugin, PluginAssemblyLoadContext) LoadPlugin(ManifestLoadResult manifestLoadResult)
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

        return (plugin, loadContext);
    }

    private static IWebPlugin? CreatePlugin(Assembly assembly)
    {
        var types = assembly.GetTypes().Where(t => typeof(IWebPlugin).IsAssignableFrom(t)).ToArray();

        if (types.Length != 1)
        {
            throw new ApplicationException("There should be exactly 1 member implementing IWebPlugin");
        }
        
        IWebPlugin? result = Activator.CreateInstance(types[0]) as IWebPlugin;
        return result;
    }

    private static bool TryWaitUntilUnloaded(WeakReference reference)
    {
        const int infiniteLoopGuard = 10;

        for (var i = 0; reference.IsAlive && i < infiniteLoopGuard; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        return !reference.IsAlive;
    }
}