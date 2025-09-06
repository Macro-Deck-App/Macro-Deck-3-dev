using MacroDeck.SDK.PluginSDK.Actions;

namespace MacroDeck.SDK.PluginSDK.Extensions;

/// <summary>
/// Common interface for both internal integrations and external plugins
/// </summary>
public interface IMacroDeckExtension
{
	/// <summary>
	/// Unique identifier for this extension
	/// </summary>
	string Id { get; }

	/// <summary>
	/// Display name of the extension
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Version of the extension
	/// </summary>
	string Version { get; }

	/// <summary>
	/// Type of the extension (Internal or Plugin)
	/// </summary>
	ExtensionType ExtensionType { get; }

	/// <summary>
	/// Actions provided by this extension
	/// </summary>
	List<MacroDeckAction> GetActions();

	/// <summary>
	/// Start the extension
	/// </summary>
	Task Start(CancellationToken cancellationToken = default);

	/// <summary>
	/// Stop the extension
	/// </summary>
	Task Stop(CancellationToken cancellationToken = default);
}