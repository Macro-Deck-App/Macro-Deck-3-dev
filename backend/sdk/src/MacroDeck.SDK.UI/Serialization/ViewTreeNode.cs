namespace MacroDeck.SDK.UI.Serialization;

/// <summary>
///     Represents a node in the UI view tree
/// </summary>
public class ViewTreeNode
{
    /// <summary>
    ///     Unique ID for this node instance
    /// </summary>
    public string NodeId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    ///     Type of the component (e.g., "MdText", "MdButton")
    /// </summary>
    public required string ComponentType { get; set; }

    /// <summary>
    ///     Properties for this component
    /// </summary>
    public Dictionary<string, object?> Properties { get; set; } = new();

    /// <summary>
    ///     Child nodes
    /// </summary>
    public List<ViewTreeNode> Children { get; set; } = [];

    /// <summary>
    ///     Event handlers available on this node
    /// </summary>
    public List<string> Events { get; set; } = [];
}
