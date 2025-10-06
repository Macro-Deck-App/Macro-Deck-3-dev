namespace MacroDeck.SDK.UI.Registry;

/// <summary>
/// Attribute to mark a view for automatic registration
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class MdUiViewAttribute : Attribute
{
    /// <summary>
    /// Unique view ID. If not specified, uses the full type name.
    /// </summary>
    public string? ViewId { get; set; }
    
    /// <summary>
    /// View namespace. If not specified, uses the plugin ID.
    /// </summary>
    public string? Namespace { get; set; }
}
