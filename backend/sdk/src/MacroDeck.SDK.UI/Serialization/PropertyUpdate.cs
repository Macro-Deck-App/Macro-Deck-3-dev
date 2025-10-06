namespace MacroDeck.SDK.UI.Serialization;

/// <summary>
/// Represents a property update for a specific node
/// </summary>
public class PropertyUpdate
{
    /// <summary>
    /// ID of the node to update
    /// </summary>
    public required string NodeId { get; set; }
    
    /// <summary>
    /// Properties that have changed
    /// </summary>
    public Dictionary<string, object?> Properties { get; set; } = new();
}

/// <summary>
/// Batch of property updates for multiple nodes
/// </summary>
public class PropertyUpdateBatch
{
    /// <summary>
    /// Session ID this batch belongs to
    /// </summary>
    public required string SessionId { get; set; }
    
    /// <summary>
    /// List of property updates
    /// </summary>
    public List<PropertyUpdate> Updates { get; set; } = new();
}
