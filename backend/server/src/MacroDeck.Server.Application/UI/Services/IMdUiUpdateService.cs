using MacroDeck.SDK.UI.Serialization;

namespace MacroDeck.Server.Application.UI.Services;

/// <summary>
///     Interface for sending UI updates to clients
/// </summary>
public interface IMdUiUpdateService
{
	/// <summary>
	///     Send view tree to a specific session
	/// </summary>
	Task SendViewTreeAsync(string sessionId, ViewTreeNode viewTree);
	
	/// <summary>
	///     Send property updates to a specific session
	/// </summary>
	Task SendPropertyUpdatesAsync(PropertyUpdateBatch updates);

	/// <summary>
	///     Send error to a specific session
	/// </summary>
	Task SendErrorAsync(string sessionId, string message, string? viewId = null);
}
