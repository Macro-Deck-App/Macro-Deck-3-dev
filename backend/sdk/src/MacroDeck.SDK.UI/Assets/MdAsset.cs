namespace MacroDeck.SDK.UI.Assets;

/// <summary>
/// Represents an asset (image, icon, etc.)
/// </summary>
public class MdAsset
{
    /// <summary>
    /// Unique asset ID
    /// </summary>
    public required string AssetId { get; init; }
    
    /// <summary>
    /// Asset content type (e.g., "image/png")
    /// </summary>
    public required string ContentType { get; init; }
    
    /// <summary>
    /// Asset data
    /// </summary>
    public required byte[] Data { get; init; }
    
    /// <summary>
    /// Plugin ID that owns this asset (if applicable)
    /// </summary>
    public string? PluginId { get; init; }
}
