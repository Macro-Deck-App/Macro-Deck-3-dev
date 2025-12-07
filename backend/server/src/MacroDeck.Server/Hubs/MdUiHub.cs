using MacroDeck.SDK.UI.Messages;
using MacroDeck.SDK.UI.Registry;
using MacroDeck.Server.Application.UI.Registry;
using MacroDeck.Server.Application.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using ILogger = Serilog.ILogger;

namespace MacroDeck.Server.Hubs;

/// <summary>
///     SignalR Hub for UI communication with the frontend
/// </summary>
public class MdUiHub : Hub<IMdUiClient>
{

	// Track sessions per connection for cleanup
	private static readonly Dictionary<string, HashSet<string>> _connectionSessions = new();

	// Track which connection owns which session (sessionId -> connectionId)
	private static readonly Dictionary<string, string> _sessionToConnection = new();
	private static readonly object _connectionSessionsLock = new();
	private readonly IHubContext<MdUiHub, IMdUiClient> _hubContext;
	private readonly ILogger _logger = Log.ForContext<MdUiHub>();
	private readonly MdUiRegistry _registry;
	private readonly MdUiStateManager _stateManager;

	public MdUiHub(MdUiStateManager stateManager, MdUiRegistry registry, IHubContext<MdUiHub, IMdUiClient> hubContext)
	{
		_stateManager = stateManager;
		_registry = registry;
		_hubContext = hubContext;
	}

	public override async Task OnConnectedAsync()
	{
		_logger.Debug("UI client connected: {ConnectionId}", Context.ConnectionId);

		lock (_connectionSessionsLock)
		{
			_connectionSessions[Context.ConnectionId] = new HashSet<string>();
		}

		// Subscribe to view registry events
		_registry.ViewRegistered += OnViewRegistered;
		_registry.ViewUnregistered += OnViewUnregistered;

		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		_logger.Debug("UI client disconnected: {ConnectionId}", Context.ConnectionId);

		// Clean up all sessions for this connection
		lock (_connectionSessionsLock)
		{
			if (_connectionSessions.TryGetValue(Context.ConnectionId, out var sessions))
			{
				foreach (var sessionId in sessions)
				{
					_logger.Debug("Removing session {SessionId} for disconnected connection {ConnectionId}",
						sessionId,
						Context.ConnectionId);
					_stateManager.RemoveSession(sessionId);
					_sessionToConnection.Remove(sessionId);
				}

				_connectionSessions.Remove(Context.ConnectionId);
			}
		}

		// Unsubscribe from view registry events
		_registry.ViewRegistered -= OnViewRegistered;
		_registry.ViewUnregistered -= OnViewUnregistered;

		await base.OnDisconnectedAsync(exception);
	}

	private void OnViewRegistered(MdUiViewMetadata metadata)
	{
		// Notify all clients about the new view
		_hubContext.Clients.All.ViewRegistered(metadata);
	}

	private void OnViewUnregistered(string viewId)
	{
		// Notify all clients about the removed view
		_hubContext.Clients.All.ViewUnregistered(viewId);
	}

	/// <summary>
	///     Navigate to a view - creates a new session and returns the session ID
	/// </summary>
	public async Task<string> NavigateToView(string viewId, string? sessionId = null)
	{
		try
		{
			// Use provided session ID or generate a new one
			var actualSessionId = sessionId ?? Guid.NewGuid().ToString("N");

			_logger.Debug(
				"Client {ConnectionId} navigating to view {ViewId} - Session: {SessionId} (provided: {ProvidedSessionId})",
				Context.ConnectionId,
				viewId,
				actualSessionId,
				sessionId ?? "null");

			// Track this session for this connection
			lock (_connectionSessionsLock)
			{
				if (!_connectionSessions.TryGetValue(Context.ConnectionId, out var sessions))
				{
					sessions = new HashSet<string>();
					_connectionSessions[Context.ConnectionId] = sessions;
				}

				sessions.Add(actualSessionId);

				// Map sessionId -> connectionId for routing updates
				_sessionToConnection[actualSessionId] = Context.ConnectionId;

				_logger.Debug("Mapped session {SessionId} to connection {ConnectionId}. Total sessions: {Count}",
					actualSessionId,
					Context.ConnectionId,
					_sessionToConnection.Count);
			}

			var viewTree = _stateManager.SetRootView(actualSessionId, viewId);

			await Clients.Caller.ReceiveViewTree(new ViewTreeMessage
			{
				SessionId = actualSessionId,
				ViewTree = viewTree,
				RootViewId = viewId
			});

			return actualSessionId;
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error navigating to view {ViewId}", viewId);
			var errorSessionId = sessionId ?? Guid.NewGuid().ToString("N");
			await Clients.Caller.ReceiveError(new UiErrorMessage
			{
				SessionId = errorSessionId,
				Message = $"Failed to navigate to view: {ex.Message}"
			});
			throw;
		}
	}

	/// <summary>
	///     Handle an event from the frontend
	/// </summary>
	public async Task SendEvent(UiEventMessage eventMessage)
	{
		try
		{
			_logger.Debug(
				"Received event {EventName} for view {ViewId} from session {SessionId}. Total sessions: {Count}",
				eventMessage.EventName,
				eventMessage.ViewId,
				eventMessage.SessionId,
				_sessionToConnection.Count);

			// Check if the session is mapped to this connection
			string? mappedConnectionId;
			lock (_connectionSessionsLock)
			{
				if (!_sessionToConnection.TryGetValue(eventMessage.SessionId, out mappedConnectionId))
				{
					_logger.Warning(
						"Session {SessionId} not found in session-to-connection mapping. Available sessions: {Sessions}",
						eventMessage.SessionId,
						string.Join(", ", _sessionToConnection.Keys));
				}
				else if (mappedConnectionId != Context.ConnectionId)
				{
					_logger.Warning(
						"Session {SessionId} is mapped to connection {MappedConnectionId}, but event came from {ConnectionId}",
						eventMessage.SessionId,
						mappedConnectionId,
						Context.ConnectionId);
				}
				else
				{
					_logger.Debug("Session {SessionId} correctly mapped to connection {ConnectionId}",
						eventMessage.SessionId,
						Context.ConnectionId);
				}
			}

			_stateManager.HandleEvent(eventMessage.SessionId,
				eventMessage.ViewId,
				eventMessage.EventName,
				eventMessage.Parameters);

			// Note: ViewChanged event will automatically send the updated tree
			// So we don't need to manually send it here
		}
		catch (Exception ex)
		{
			_logger.Error(ex,
				"Error handling event {EventName} for view {ViewId}",
				eventMessage.EventName,
				eventMessage.ViewId);
			await Clients.Caller.ReceiveError(new UiErrorMessage
			{
				SessionId = eventMessage.SessionId,
				Message = $"Failed to handle event: {ex.Message}",
				ViewId = eventMessage.ViewId
			});
		}
	}

	/// <summary>
	///     Request to reload a specific view session (e.g., after reconnect)
	/// </summary>
	public async Task ReloadView(string sessionId)
	{
		try
		{
			_logger.Debug("Reloading view for session {SessionId}", sessionId);

			var viewTree = _stateManager.BuildViewTree(sessionId);
			if (viewTree != null)
			{
				await Clients.Caller.ReceiveViewTree(new ViewTreeMessage
				{
					SessionId = sessionId,
					ViewTree = viewTree,
					RootViewId = viewTree.NodeId
				});
			}
			else
			{
				_logger.Warning("No view tree found for session {SessionId}", sessionId);
				await Clients.Caller.ReceiveError(new UiErrorMessage
				{
					SessionId = sessionId,
					Message = "Session not found or expired"
				});
			}
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error reloading view for session {SessionId}", sessionId);
			await Clients.Caller.ReceiveError(new UiErrorMessage
			{
				SessionId = sessionId,
				Message = $"Failed to reload view: {ex.Message}"
			});
		}
	}

	/// <summary>
	///     Register a view from a plugin (called by plugins)
	/// </summary>
	public Task RegisterView(RegisterViewMessage message)
	{
		try
		{
			_logger.Debug("Registering view {ViewId} from plugin {PluginId}", message.ViewId, message.PluginId);

			_registry.RegisterView(new MdUiViewMetadata
			{
				ViewId = message.ViewId,
				Namespace = message.Namespace,
				TransportMode = TransportMode.Proxied,
				PluginId = message.PluginId
			});

			return Task.CompletedTask;
		}
		catch (Exception ex)
		{
			_logger.Error(ex,
				"Error registering view {ViewId} from plugin {PluginId}",
				message.ViewId,
				message.PluginId);
			throw;
		}
	}

	/// <summary>
	///     Unregister all views from a plugin (called when plugin disconnects)
	/// </summary>
	public Task UnregisterPluginViews(UnregisterViewsMessage message)
	{
		try
		{
			_logger.Debug("Unregistering all views from plugin {PluginId}", message.PluginId);
			_registry.UnregisterPluginViews(message.PluginId);
			return Task.CompletedTask;
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error unregistering views from plugin {PluginId}", message.PluginId);
			throw;
		}
	}

	/// <summary>
	///     Get all registered views (for debugging/development purposes)
	/// </summary>
	public Task<IEnumerable<MdUiViewMetadata>> GetRegisteredViews()
	{
		try
		{
			_logger.Verbose("Getting all registered views for client {ConnectionId}", Context.ConnectionId);
			return Task.FromResult(_registry.GetAllViews());
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error getting registered views");
			throw;
		}
	}

	/// <summary>
	///     Get the connection ID for a session ID (used by the update service)
	/// </summary>
	public static string? GetConnectionIdForSession(string sessionId)
	{
		lock (_connectionSessionsLock)
		{
			return _sessionToConnection.TryGetValue(sessionId, out var connectionId) ? connectionId : null;
		}
	}
}
