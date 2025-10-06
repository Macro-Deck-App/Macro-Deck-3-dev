using MacroDeck.SDK.UI.Serialization;

namespace MacroDeck.SDK.UI.Messages;

/// <summary>
/// Message to register a UI view from a plugin
/// </summary>
public class RegisterViewMessage
{
    public required string ViewId { get; init; }
    public required string ViewTypeName { get; init; }
    public required string Namespace { get; init; }
    public required string PluginId { get; init; }
}

/// <summary>
/// Message to unregister views
/// </summary>
public class UnregisterViewsMessage
{
    public required string PluginId { get; init; }
}

/// <summary>
/// Message to send a view tree to the frontend
/// </summary>
public class ViewTreeMessage
{
    public required string SessionId { get; init; }
    public required ViewTreeNode ViewTree { get; init; }
    public required string RootViewId { get; init; }
}

/// <summary>
/// Message for view updates
/// </summary>
public class ViewUpdateMessage
{
    public required string SessionId { get; init; }
    public required string ViewId { get; init; }
    public required ViewTreeNode UpdatedNode { get; init; }
}

/// <summary>
/// Message for UI events from frontend
/// </summary>
public class UiEventMessage
{
    public required string SessionId { get; init; }
    public required string ViewId { get; init; }
    public required string EventName { get; init; }
    public Dictionary<string, object>? Parameters { get; init; }
}

/// <summary>
/// Message for navigation changes
/// </summary>
public class NavigationMessage
{
    public required string SessionId { get; init; }
    public required string ViewId { get; init; }
    public required NavigationType Type { get; init; }
}

public enum NavigationType
{
    Set,
    Push,
    Pop
}

/// <summary>
/// Message for errors
/// </summary>
public class UiErrorMessage
{
    public required string SessionId { get; init; }
    public required string Message { get; init; }
    public string? ViewId { get; init; }
}

/// <summary>
/// Message to request asset data from plugin
/// </summary>
public class AssetRequestMessage
{
    public required string AssetId { get; init; }
    public required string PluginId { get; init; }
}

/// <summary>
/// Message with asset data response
/// </summary>
public class AssetResponseMessage
{
    public required string AssetId { get; init; }
    public required byte[] Data { get; init; }
    public required string ContentType { get; init; }
}
