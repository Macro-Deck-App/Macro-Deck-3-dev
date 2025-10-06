namespace MacroDeck.SDK.UI.Assets;

/// <summary>
/// Provider for assets from embedded resources
/// </summary>
public interface IMdAssetProvider
{
    /// <summary>
    /// Registers an asset from an embedded resource
    /// </summary>
    void RegisterAsset(string assetId, string resourceName, string contentType);
    
    /// <summary>
    /// Gets an asset by ID
    /// </summary>
    Task<MdAsset?> GetAssetAsync(string assetId);
    
    /// <summary>
    /// Unregisters all assets for a plugin
    /// </summary>
    void UnregisterPluginAssets(string pluginId);
}
