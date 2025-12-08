using MacroDeck.SDK.UI.Registry;
using MacroDeck.SDK.UI.Serialization;

namespace MacroDeck.SDK.UI.Messages;

/// <summary>
///     Client interface for type-safe SignalR calls
/// </summary>
public interface IMdUiClient
{
	/// <summary>
	///     Sends a view tree to the client
	/// </summary>
	Task ReceiveViewTree(ViewTreeMessage message);

	/// <summary>
	///     Sends a view update to the client
	/// </summary>
	Task ReceiveViewUpdate(ViewUpdateMessage message);

	/// <summary>
	///     Sends property updates to the client
	/// </summary>
	Task ReceivePropertyUpdates(PropertyUpdateBatch updates);

	/// <summary>
	///     Sends an error to the client
	/// </summary>
	Task ReceiveError(UiErrorMessage message);

	/// <summary>
	///     Notifies client that a plugin disconnected
	/// </summary>
	Task PluginDisconnected(string pluginId);

	/// <summary>
	///     Notifies client that a view was registered
	/// </summary>
	Task ViewRegistered(MdUiViewMetadata metadata);

	/// <summary>
	///     Notifies client that a view was unregistered
	/// </summary>
	Task ViewUnregistered(string viewId);

	/// <summary>
	///     Requests user approval to open a link
	/// </summary>
	Task LinkRequest(LinkRequestMessage message);
}
