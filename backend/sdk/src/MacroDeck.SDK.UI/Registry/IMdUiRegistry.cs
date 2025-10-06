using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Registry;

/// <summary>
/// Registry for UI views
/// </summary>
public interface IMdUiRegistry
{
    /// <summary>
    /// Registers a UI view type
    /// </summary>
    void RegisterView(MdUiViewMetadata metadata);
    
    /// <summary>
    /// Unregisters a UI view
    /// </summary>
    void UnregisterView(string viewId);
    
    /// <summary>
    /// Unregisters all views from a plugin
    /// </summary>
    void UnregisterPluginViews(string pluginId);
    
    /// <summary>
    /// Gets view metadata by ID
    /// </summary>
    MdUiViewMetadata? GetViewMetadata(string viewId);
    
    /// <summary>
    /// Gets all registered views
    /// </summary>
    IEnumerable<MdUiViewMetadata> GetAllViews();
    
    /// <summary>
    /// Creates an instance of a view
    /// </summary>
    MdUiView CreateViewInstance(string viewId);
}
