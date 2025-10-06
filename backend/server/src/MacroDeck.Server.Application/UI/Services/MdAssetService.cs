using System.Collections.Concurrent;
using System.Reflection;
using MacroDeck.SDK.UI.Assets;

namespace MacroDeck.Server.Application.UI.Services;

/// <summary>
/// Service for managing UI assets
/// </summary>
public class MdAssetService : IMdAssetProvider
{
    private readonly ConcurrentDictionary<string, AssetRegistration> _assets = new();

    public void RegisterAsset(string assetId, string resourceName, string contentType)
    {
        _assets[assetId] = new AssetRegistration
        {
            AssetId = assetId,
            ResourceName = resourceName,
            ContentType = contentType,
            Assembly = Assembly.GetCallingAssembly()
        };
    }

    public async Task<MdAsset?> GetAssetAsync(string assetId)
    {
        if (!_assets.TryGetValue(assetId, out var registration))
        {
            return null;
        }

        // Load from embedded resource
        if (registration.Assembly != null && registration.ResourceName != null)
        {
            var stream = registration.Assembly.GetManifestResourceStream(registration.ResourceName);
            if (stream == null)
            {
                return null;
            }

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            
            return new MdAsset
            {
                AssetId = assetId,
                ContentType = registration.ContentType,
                Data = memoryStream.ToArray(),
                PluginId = registration.PluginId
            };
        }
        
        // Return cached plugin asset
        if (registration.IsFromPlugin && registration.CachedData != null)
        {
            return new MdAsset
            {
                AssetId = assetId,
                ContentType = registration.ContentType,
                Data = registration.CachedData,
                PluginId = registration.PluginId
            };
        }
        
        return null;
    }

    public void UnregisterPluginAssets(string pluginId)
    {
        var pluginAssets = _assets.Values.Where(a => a.PluginId == pluginId).ToList();
        foreach (var asset in pluginAssets)
        {
            _assets.TryRemove(asset.AssetId, out _);
        }
    }

    /// <summary>
    /// Registers an asset from a plugin (proxied through server)
    /// </summary>
    public void RegisterPluginAsset(string assetId, string pluginId, string contentType)
    {
        _assets[assetId] = new AssetRegistration
        {
            AssetId = assetId,
            PluginId = pluginId,
            ContentType = contentType,
            IsFromPlugin = true
        };
    }

    /// <summary>
    /// Sets asset data received from a plugin
    /// </summary>
    public void SetPluginAssetData(string assetId, byte[] data)
    {
        if (_assets.TryGetValue(assetId, out var registration) && registration.IsFromPlugin)
        {
            registration.CachedData = data;
        }
    }

    private class AssetRegistration
    {
        public required string AssetId { get; init; }
        public string? ResourceName { get; init; }
        public required string ContentType { get; init; }
        public Assembly? Assembly { get; init; }
        public string? PluginId { get; init; }
        public bool IsFromPlugin { get; init; }
        public byte[]? CachedData { get; set; }
    }
}
