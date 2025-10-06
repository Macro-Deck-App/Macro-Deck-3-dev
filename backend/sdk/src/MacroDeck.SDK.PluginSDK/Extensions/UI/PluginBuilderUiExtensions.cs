using MacroDeck.SDK.UI.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MacroDeck.SDK.PluginSDK.Extensions.UI;

/// <summary>
/// Extension methods for PluginBuilder to register UI views
/// </summary>
public static class PluginBuilderUiExtensions
{
    /// <summary>
    /// Register UI views from the plugin
    /// </summary>
    public static PluginBuilder RegisterViews(this PluginBuilder builder, Action<UiViewCollection> configure)
    {
        var collection = new UiViewCollection();
        configure(collection);
        
        // Store the collection in services so the plugin can access it later
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(collection);
        });
        
        return builder;
    }
}
