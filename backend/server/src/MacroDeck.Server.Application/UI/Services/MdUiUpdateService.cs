using MacroDeck.SDK.UI.Serialization;
using Serilog;

namespace MacroDeck.Server.Application.UI.Services;

/// <summary>
///     Base service for sending UI updates to clients
/// </summary>
public abstract class MdUiUpdateService : IMdUiUpdateService
{
	protected static readonly ILogger Log = Serilog.Log.ForContext<MdUiUpdateService>();

	/// <summary>
	///     Send view tree to a specific session
	/// </summary>
	public abstract Task SendViewTreeAsync(string sessionId, ViewTreeNode viewTree);
	
	/// <summary>
	///     Send property updates to a specific session
	/// </summary>
	public abstract Task SendPropertyUpdatesAsync(PropertyUpdateBatch updates);

	/// <summary>
	///     Send error to a specific session
	/// </summary>
	public abstract Task SendErrorAsync(string sessionId, string message, string? viewId = null);
}
