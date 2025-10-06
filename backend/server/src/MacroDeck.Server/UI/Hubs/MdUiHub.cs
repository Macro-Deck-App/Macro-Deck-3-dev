using MacroDeck.SDK.UI.Messages;
using MacroDeck.SDK.UI.Registry;
using MacroDeck.Server.Application.UI.Registry;
using MacroDeck.Server.Application.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using ILogger = Serilog.ILogger;

namespace MacroDeck.Server.UI.Hubs;

/// <summary>
///     SignalR Hub for UI communication with the frontend
/// </summary>
public class MdUiHub : Hub<IMdUiClient>
{
	private readonly ILogger _logger = Log.ForContext<MdUiHub>();
	private readonly MdUiRegistry _registry;
	private readonly MdUiStateManager _stateManager;
	private readonly IHubContext<MdUiHub, IMdUiClient> _hubContext;

	public MdUiHub(MdUiStateManager stateManager, MdUiRegistry registry, IHubContext<MdUiHub, IMdUiClient> hubContext)
	{
		_stateManager = stateManager;
		_registry = registry;
		_hubContext = hubContext;
	}

	public override async Task OnConnectedAsync()
	{
		_logger.Debug("UI client connected: {ConnectionId}", Context.ConnectionId);
		
		// Subscribe to view registry events
		_registry.ViewRegistered += OnViewRegistered;
		_registry.ViewUnregistered += OnViewUnregistered;
		
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		_logger.Debug("UI client disconnected: {ConnectionId}", Context.ConnectionId);
		
		// Unsubscribe from view registry events
		_registry.ViewRegistered -= OnViewRegistered;
		_registry.ViewUnregistered -= OnViewUnregistered;
		
		_stateManager.RemoveSession(Context.ConnectionId);
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
    ///     Navigate to a view
    /// </summary>
    public async Task NavigateToView(string viewId)
	{
		try
		{
			_logger.Debug("Client {ConnectionId} navigating to view {ViewId}", Context.ConnectionId, viewId);

			var viewTree = _stateManager.SetRootView(Context.ConnectionId, viewId);

			await Clients.Caller.ReceiveViewTree(new ViewTreeMessage
			{
				SessionId = Context.ConnectionId,
				ViewTree = viewTree,
				RootViewId = viewId
			});
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error navigating to view {ViewId}", viewId);
			await Clients.Caller.ReceiveError(new UiErrorMessage
			{
				SessionId = Context.ConnectionId,
				Message = $"Failed to navigate to view: {ex.Message}"
			});
		}
	}

    /// <summary>
    ///     Handle an event from the frontend
    /// </summary>
    public async Task SendEvent(UiEventMessage eventMessage)
	{
		try
		{
			_logger.Debug("Received event {EventName} for view {ViewId} from session {SessionId}",
				eventMessage.EventName,
				eventMessage.ViewId,
				eventMessage.SessionId);

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
    ///     Request to reload the current view (e.g., after reconnect)
    /// </summary>
    public async Task ReloadView()
	{
		try
		{
			var viewTree = _stateManager.BuildViewTree(Context.ConnectionId);
			if (viewTree != null)
			{
				await Clients.Caller.ReceiveViewTree(new ViewTreeMessage
				{
					SessionId = Context.ConnectionId,
					ViewTree = viewTree,
					RootViewId = viewTree.NodeId
				});
			}
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error reloading view for session {SessionId}", Context.ConnectionId);
			await Clients.Caller.ReceiveError(new UiErrorMessage
			{
				SessionId = Context.ConnectionId,
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
}


