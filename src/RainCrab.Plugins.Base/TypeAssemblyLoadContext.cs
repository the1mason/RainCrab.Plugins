using System.Reflection;
using System.Runtime.Loader;

namespace RainCrab.Plugins.Base;

internal class PluginAssemblyLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    internal PluginAssemblyLoadContext(string pluginPath, bool isUnloadable = false) : base(isUnloadable)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath is null)
        {
            return null!;
        }
        return LoadFromAssemblyPath(assemblyPath);
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath is null)
        {
            return IntPtr.Zero;
        }
        return LoadUnmanagedDllFromPath(libraryPath);
    }
}