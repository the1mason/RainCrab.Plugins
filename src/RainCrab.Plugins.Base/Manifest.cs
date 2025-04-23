namespace RainCrab.Plugins.AspNet;

public class Manifest
{
    public Manifest(string id, string version, string assembly, string name, int priority = 0)
    {
        Id = id;
        Version = version;
        Assembly = assembly;
        Name = name;
        Priority = priority;
    }

    /// <summary>
    /// A unique identifier for the plugin.
    /// It is used to identify the plugin in the plugin loader or when other plugins depend on it.
    /// </summary>
    public string Id { get; }
    
    /// <summary>
    /// A version string of the plugin.
    /// </summary>
    public string Version { get; }
    
    /// <summary>
    /// A relative path from the manifest's directory to the plugin's main assembly.
    /// Dependent assemblies would be loaded automatically.
    /// </summary>
    public string Assembly { get; }
    
    /// <summary>
    /// A display name for the plugin. Not required to be unique. English is recommended.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Plugin's priority. Plugins with higher priority will be loaded first.
    /// It is not recommended to use this field.
    /// </summary>
    public int Priority { get; }
}