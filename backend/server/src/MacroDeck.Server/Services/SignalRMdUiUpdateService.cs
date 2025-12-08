using MacroDeck.SDK.UI.Messages;
using MacroDeck.SDK.UI.Serialization;
using MacroDeck.Server.Application.UI.Services;
using MacroDeck.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MacroDeck.Server.Services;

/// <summary>
///     SignalR implementation of the UI update service
/// </summary>
public class SignalRMdUiUpdateService : MdUiUpdateService
{
	private readonly IHubContext<MdUiHub, IMdUiClient> _hubContext;

	public SignalRMdUiUpdateService(IHubContext<MdUiHub, IMdUiClient> hubContext)
	{
		_hubContext = hubContext;
	}

	/// <summary>
	///     Send view tree to a specific session using SignalR groups
	/// </summary>
	public override async Task SendViewTreeAsync(string sessionId, ViewTreeNode viewTree)
	{
		try
		{
			// Log some properties for debugging
			var text = FindTextInTree(viewTree);
			Log.Verbose("Sending view tree to session {SessionId}, sample text: {Text}", sessionId, text);
			
			// Use SignalR groups to send to the session
			await _hubContext.Clients.Group(sessionId).ReceiveViewTree(new ViewTreeMessage
			{
				SessionId = sessionId,
				ViewTree = viewTree,
				RootViewId = viewTree.NodeId
			});
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error sending view tree to session {SessionId}", sessionId);
		}
	}
	
	/// <summary>
	///     Send property updates to a specific session using SignalR groups
	/// </summary>
	public override async Task SendPropertyUpdatesAsync(PropertyUpdateBatch updates)
	{
		try
		{
			Log.Debug("Sending {Count} property updates to session {SessionId}", 
				updates.Updates.Count, updates.SessionId);
			
			// Use SignalR groups to send to the session
			await _hubContext.Clients.Group(updates.SessionId).ReceivePropertyUpdates(updates);
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error sending property updates to session {SessionId}", updates.SessionId);
		}
	}
	
	private string FindTextInTree(ViewTreeNode node)
	{
		if (node.ComponentType == "MdText" && node.Properties.TryGetValue("text", out var text))
		{
			return text?.ToString() ?? "";
		}
		
		foreach (var child in node.Children)
		{
			var childText = FindTextInTree(child);
			if (!string.IsNullOrEmpty(childText))
			{
				return childText;
			}
		}
		
		return "";
	}

	/// <summary>
	///     Send error to a specific session using SignalR groups
	/// </summary>
	public override async Task SendErrorAsync(string sessionId, string message, string? viewId = null)
	{
		try
		{
			Log.Verbose("Sending error to session {SessionId}: {Message}", sessionId, message);
			
			// Use SignalR groups to send to the session
			await _hubContext.Clients.Group(sessionId).ReceiveError(new UiErrorMessage
			{
				SessionId = sessionId,
				Message = message,
				ViewId = viewId
			});
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error sending error message to session {SessionId}", sessionId);
		}
	}
}
