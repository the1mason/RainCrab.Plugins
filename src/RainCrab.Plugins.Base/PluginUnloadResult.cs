namespace RainCrab.Plugins.Base;

public sealed record PluginUnloadResult(bool Success, IReadOnlyList<PluginUnloadError> Errors);

public sealed record PluginUnloadError(string PluginName, string Message);