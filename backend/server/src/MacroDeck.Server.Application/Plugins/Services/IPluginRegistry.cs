using MacroDeck.SDK.PluginSDK.Messages;
using MacroDeck.Server.Application.Plugins.Models;

namespace MacroDeck.Server.Application.Plugins.Services;

public interface IPluginRegistry
{
	Task<bool> RegisterPlugin(string connectionId, ConnectMessage connectMessage);
	Task<bool> UpdatePluginActions(string pluginId, IEnumerable<ActionDefinition> actions);
	Task UnregisterPlugin(string connectionId);
	Task<PluginInfo?> GetPlugin(string pluginId);
	Task<PluginInfo?> GetPluginByConnectionId(string connectionId);
	Task<PluginInfo?> GetPluginByActionId(string actionId);
	Task<IEnumerable<PluginInfo>> GetAllPlugins();
	Task UpdateHeartbeat(string connectionId);
	Task<bool> HasAction(string pluginId, string actionId);
}