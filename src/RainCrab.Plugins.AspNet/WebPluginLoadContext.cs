using Microsoft.AspNetCore.Builder;

namespace RainCrab.Plugins.AspNet;

public class WebPluginLoadContext
{
    public required IReadOnlyList<IWebPlugin> LoadedPlugins { get; init; }
    public required WebApplicationBuilder ApplicationBuilder { get; init; }
    public WebApplication? Application
    {
        get => _application;
        set
        {
            if (_application is not null)
            {
                throw new InvalidOperationException("Application already initialized");
            }
            _application = value;
        }
    }

    private WebApplication? _application;
}