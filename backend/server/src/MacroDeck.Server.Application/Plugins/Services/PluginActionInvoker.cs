using MacroDeck.SDK.PluginSDK.Messages;
using Serilog;

namespace MacroDeck.Server.Application.Plugins.Services;

public class PluginActionInvoker : IPluginActionInvoker
{
	private readonly IPluginCommunicationService _communicationService;
	private readonly ILogger _logger;
	private readonly IPluginRegistry _pluginRegistry;

	public PluginActionInvoker(
		IPluginRegistry pluginRegistry,
		IPluginCommunicationService communicationService)
	{
		_logger = Log.ForContext<PluginActionInvoker>();
		_pluginRegistry = pluginRegistry;
		_communicationService = communicationService;
	}

	public async Task<bool> InvokeAction(string pluginId, string actionId, object? parameters = null)
	{
		var plugin = await _pluginRegistry.GetPlugin(pluginId);
		if (plugin == null)
		{
			_logger.Warning("Attempted to invoke action on unknown plugin {PluginId}", pluginId);
			return false;
		}

		if (!await _pluginRegistry.HasAction(pluginId, actionId))
		{
			_logger.Warning("Plugin {PluginId} does not have action {ActionId}", pluginId, actionId);
			return false;
		}

		var invokeMessage = new InvokeActionMessage
		{
			ActionId = actionId,
			Parameters = parameters
		};

		try
		{
			await _communicationService.InvokeAction(plugin.ConnectionId, invokeMessage);
			_logger.Debug("Action {ActionId} invoked on plugin {PluginId}", actionId, pluginId);
			return true;
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Failed to invoke action {ActionId} on plugin {PluginId}", actionId, pluginId);
			return false;
		}
	}
}
