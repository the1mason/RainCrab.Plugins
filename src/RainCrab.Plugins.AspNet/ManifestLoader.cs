using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace RainCrab.Plugins.AspNet;

internal record ManifestLoadResult(Manifest Manifest, string Location);

/// <summary>
/// Loads and deserializes manifests from the specified directory.
/// </summary>
internal class ManifestLoader(
    string lookupDirectory,
    JsonSerializerOptions jsonSerializerOptions,
    ILogger logger)
{
   
    internal async Task<IReadOnlyList<ManifestLoadResult>> LoadManifestsAsync()
    {
        var manifestsPath = Directory.GetFiles(lookupDirectory, "*.manifest.json", SearchOption.AllDirectories);

        List<ManifestLoadResult> manifests = [];

        foreach (var location in manifestsPath)
        {
            Manifest? manifest = await LoadManifest(location);
            if (manifest is null)
                continue;
            
            manifests.Add(new (manifest, location));
        }
        return manifests;
    }

    private async Task<Manifest?> LoadManifest(string path)
    {
        try
        {
            await using var stream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<Manifest>(stream, jsonSerializerOptions);
        }
        catch (JsonException ex)
        {
            logger.LogCritical("Failed to deserialize manifest for {path}", path);
            throw;
        }
    }

}