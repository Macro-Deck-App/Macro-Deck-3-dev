using MacroDeck.SDK.PluginSDK.Hubs;
using MacroDeck.SDK.PluginSDK.Messages;
using MacroDeck.Server.Application.Plugins.Services;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace MacroDeck.Server.Hubs;

public class PluginCommunicationHub : Hub<IPluginCommunicationClient>
{
	private readonly ILogger _logger = Log.ForContext<PluginCommunicationHub>();
	private readonly IPluginRegistry _pluginRegistry;

	public PluginCommunicationHub(IPluginRegistry pluginRegistry)
	{
		_pluginRegistry = pluginRegistry;
	}

	public override async Task OnConnectedAsync()
	{
		_logger.Debug("New connection established: {ConnectionId}", Context.ConnectionId);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		await _pluginRegistry.UnregisterPlugin(Context.ConnectionId);

		if (exception != null)
		{
			_logger.Warning(exception,
				"Plugin connection {ConnectionId} disconnected with exception",
				Context.ConnectionId);
		}
		else
		{
			_logger.Debug("Plugin connection {ConnectionId} disconnected", Context.ConnectionId);
		}

		await base.OnDisconnectedAsync(exception);
	}

	// Connection handling
	public async Task<ConnectResponseMessage> Connect(ConnectMessage connectMessage)
	{
		_logger.Verbose("Received Connect message from plugin {ExtensionId} (Connection: {ConnectionId})",
			connectMessage.ExtensionId, Context.ConnectionId);

		try
		{
			var success = await _pluginRegistry.RegisterPlugin(Context.ConnectionId, connectMessage);
			var plugin = await _pluginRegistry.GetPluginByConnectionId(Context.ConnectionId);

			return new ConnectResponseMessage
			{
				Success = success,
				Message = success ? "Connected successfully" : "Connection failed",
				SessionId = plugin?.SessionId ?? ""
			};
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error handling connect from plugin {ExtensionId}", connectMessage.ExtensionId);
			return new ConnectResponseMessage
			{
				Success = false,
				Message = "Connection failed due to server error"
			};
		}
	}

	// Plugin registration
	public async Task<RegisterExtensionResponseMessage> RegisterExtension(string pluginId, RegisterExtensionMessage registerMessage)
	{
		_logger.Verbose("Received RegisterExtension message from plugin {PluginId} (Connection: {ConnectionId})",
			pluginId, Context.ConnectionId);

		try
		{
			var success = await _pluginRegistry.UpdatePluginActions(pluginId, registerMessage.Actions);

			return new RegisterExtensionResponseMessage
			{
				Success = success,
				Message = success ? "Extension registered successfully" : "Extension registration failed"
			};
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error handling RegisterExtension from plugin {PluginId}", pluginId);
			return new RegisterExtensionResponseMessage
			{
				Success = false,
				Message = "Registration failed due to server error"
			};
		}
	}

	// Heartbeat
	public async Task Heartbeat(string pluginId, HeartbeatMessage heartbeatMessage)
	{
		_logger.Verbose("Received Heartbeat from plugin {PluginId} (Connection: {ConnectionId})",
			pluginId, Context.ConnectionId);

		try
		{
			await _pluginRegistry.UpdateHeartbeat(Context.ConnectionId);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error handling Heartbeat from plugin {PluginId}", pluginId);
		}
	}

	// Logging
	public Task LogMessage(string pluginId, LogMessage logMessage)
	{
		try
		{
			switch (logMessage.Level)
			{
				case LogEventLevel.Verbose:
					_logger.Verbose("[Plugin:{PluginId}] [{Category}] {Message}",
						pluginId, logMessage.Category, logMessage.Message);
					break;
				case LogEventLevel.Debug:
					_logger.Debug("[Plugin:{PluginId}] [{Category}] {Message}",
						pluginId, logMessage.Category, logMessage.Message);
					break;
				case LogEventLevel.Information:
					_logger.Information("[Plugin:{PluginId}] [{Category}] {Message}",
						pluginId, logMessage.Category, logMessage.Message);
					break;
				case LogEventLevel.Warning:
					_logger.Warning("[Plugin:{PluginId}] [{Category}] {Message}",
						pluginId, logMessage.Category, logMessage.Message);
					break;
				case LogEventLevel.Error:
					_logger.Error("[Plugin:{PluginId}] [{Category}] {Message}",
						pluginId, logMessage.Category, logMessage.Message);
					break;
				case LogEventLevel.Fatal:
					_logger.Fatal("[Plugin:{PluginId}] [{Category}] {Message}",
						pluginId, logMessage.Category, logMessage.Message);
					break;
				default:
					_logger.Information("[Plugin:{PluginId}] [{Category}] {Message}",
						pluginId, logMessage.Category, logMessage.Message);
					break;
			}
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error handling LogMessage from plugin {PluginId}", pluginId);
		}

		return Task.CompletedTask;
	}

	// Action response handling
	public Task InvokeActionResponse(string pluginId, InvokeActionResponseMessage response)
	{
		try
		{
			_logger.Debug("Received action response from plugin {PluginId}: Success={Success}, Message={Message}",
				pluginId, response.Success, response.Message);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error handling InvokeActionResponse from plugin {PluginId}", pluginId);
		}

		return Task.CompletedTask;
	}

	// Server-to-Client methods (type-safe)
	
	/// <summary>
	/// Sends an action invocation request to a specific plugin
	/// </summary>
	public async Task SendInvokeActionToPlugin(string connectionId, InvokeActionMessage message)
	{
		try
		{
			await Clients.Client(connectionId).InvokeAction(message);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error sending InvokeAction to plugin connection {ConnectionId}", connectionId);
		}
	}


	/// <summary>
	/// Requests a plugin to shutdown gracefully
	/// </summary>
	public async Task RequestPluginShutdown(string connectionId)
	{
		try
		{
			await Clients.Client(connectionId).RequestShutdown();
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error requesting plugin shutdown, connection {ConnectionId}", connectionId);
		}
	}
}