using MacroDeck.SDK.UI.Serialization;

namespace MacroDeck.SDK.UI.Transport;

/// <summary>
/// Transport layer for sending UI updates
/// </summary>
public interface IMdUiTransport
{
    /// <summary>
    /// Sends a view tree to the frontend
    /// </summary>
    Task SendViewTreeAsync(string sessionId, ViewTreeNode viewTree);
    
    /// <summary>
    /// Sends a partial update to the frontend
    /// </summary>
    Task SendViewUpdateAsync(string sessionId, string viewId, ViewTreeNode updatedNode);
    
    /// <summary>
    /// Sends an error to the frontend
    /// </summary>
    Task SendErrorAsync(string sessionId, string message);
    
    /// <summary>
    /// Handles an event from the frontend
    /// </summary>
    Task HandleEventAsync(string sessionId, string viewId, string eventName, Dictionary<string, object>? parameters);
}
