namespace MacroDeck.SDK.UI.Registry;

/// <summary>
/// Metadata about a UI view
/// </summary>
public class MdUiViewMetadata
{
    /// <summary>
    /// Unique view ID (e.g., "com.example.plugin.SettingsView")
    /// </summary>
    public required string ViewId { get; init; }
    
    /// <summary>
    /// The view type (null for proxied views where type is in plugin)
    /// </summary>
    public Type? ViewType { get; init; }
    
    /// <summary>
    /// Namespace of the view
    /// </summary>
    public required string Namespace { get; init; }
    
    /// <summary>
    /// Plugin ID that owns this view (if applicable)
    /// </summary>
    public string? PluginId { get; init; }
    
    /// <summary>
    /// Transport mode for this view
    /// </summary>
    public TransportMode TransportMode { get; init; }
    
    /// <summary>
    /// SignalR connection ID (for proxied views)
    /// </summary>
    public string? ConnectionId { get; init; }
}

/// <summary>
/// Transport mode for UI views
/// </summary>
public enum TransportMode
{
    /// <summary>
    /// View runs directly in server and sends to frontend
    /// </summary>
    Direct,
    
    /// <summary>
    /// View runs in plugin, proxied through server to frontend
    /// </summary>
    Proxied
}
