using Google.Protobuf;
using MacroDeck.Protobuf;
using Serilog;

namespace MacroDeck.Server.Application.Plugins.Services;

public class PluginActionInvoker : IPluginActionInvoker
{
	private readonly ILogger _logger;
	private readonly IPluginRegistry _pluginRegistry;
	private readonly IPluginCommunicationService _communicationService;

	public PluginActionInvoker(
		IPluginRegistry pluginRegistry,
		IPluginCommunicationService communicationService)
	{
		_logger = Log.ForContext<PluginActionInvoker>();
		_pluginRegistry = pluginRegistry;
		_communicationService = communicationService;
	}

	public async Task<bool> InvokeAction(string pluginId, string actionId, string? configurationJson = null)
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
			ConfigurationJson = configurationJson ?? ""
		};

		var message = new PluginMessage
		{
			MessageId = Guid.NewGuid().ToString(),
			MessageType = MessageType.InvokeAction,
			PluginId = pluginId,
			Payload = ByteString.CopyFrom(invokeMessage.ToByteArray())
		};

		try
		{
			await _communicationService.SendToPlugin(plugin.ConnectionId, message.ToByteArray());
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