using System.Collections.Concurrent;
using MacroDeck.SDK.PluginSDK.Messages;
using MacroDeck.Server.Application.Plugins.Models;
using Serilog;

namespace MacroDeck.Server.Application.Plugins.Services;

public class PluginRegistry : IPluginRegistry
{
	private readonly ILogger _logger = Log.ForContext<PluginRegistry>();
	private readonly ConcurrentDictionary<string, PluginInfo> _pluginsByConnectionId = new();
	private readonly ConcurrentDictionary<string, PluginInfo> _pluginsById = new();

	public Task<bool> RegisterPlugin(string connectionId, ConnectMessage connectMessage)
	{
		try
		{
			var sessionId = Guid.NewGuid().ToString();

			var pluginInfo = new PluginInfo
			{
				PluginId = connectMessage.ExtensionId,
				Name = connectMessage.ExtensionName,
				Version = connectMessage.ExtensionVersion,
				SdkVersion = connectMessage.SdkVersion,
				SessionId = sessionId,
				ConnectionId = connectionId,
				ConnectedAt = DateTime.UtcNow
			};

			if (_pluginsById.TryGetValue(connectMessage.ExtensionId, out var existingPlugin))
			{
				_pluginsByConnectionId.TryRemove(existingPlugin.ConnectionId, out _);
				_logger.Warning("Plugin {PluginId} was already connected, replacing connection",
					connectMessage.ExtensionId);
			}

			_pluginsByConnectionId[connectionId] = pluginInfo;
			_pluginsById[connectMessage.ExtensionId] = pluginInfo;

			_logger.Information("Plugin {PluginId} ({Name} v{Version}) connected with session {SessionId}",
				connectMessage.ExtensionId,
				connectMessage.ExtensionName,
				connectMessage.ExtensionVersion,
				sessionId);

			return Task.FromResult(true);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Failed to register plugin {PluginId}", connectMessage.ExtensionId);
			return Task.FromResult(false);
		}
	}

	public Task<bool> UpdatePluginActions(string pluginId, IEnumerable<ActionDefinition> actions)
	{
		if (_pluginsById.TryGetValue(pluginId, out var plugin))
		{
			plugin.Actions.Clear();
			plugin.Actions.AddRange(actions);

			_logger.Information("Updated {ActionCount} actions for plugin {PluginId}",
				plugin.Actions.Count,
				pluginId);

			return Task.FromResult(true);
		}

		_logger.Warning("Attempted to update actions for unknown plugin {PluginId}", pluginId);
		return Task.FromResult(false);
	}

	public Task UnregisterPlugin(string connectionId)
	{
		if (_pluginsByConnectionId.TryRemove(connectionId, out var plugin))
		{
			_pluginsById.TryRemove(plugin.PluginId, out _);

			_logger.Information("Plugin {PluginId} ({Name}) disconnected",
				plugin.PluginId,
				plugin.Name);
		}

		return Task.CompletedTask;
	}

	public Task<PluginInfo?> GetPlugin(string pluginId)
	{
		_pluginsById.TryGetValue(pluginId, out var plugin);
		return Task.FromResult(plugin);
	}

	public Task<PluginInfo?> GetPluginByConnectionId(string connectionId)
	{
		_pluginsByConnectionId.TryGetValue(connectionId, out var plugin);
		return Task.FromResult(plugin);
	}

	public Task<PluginInfo?> GetPluginByActionId(string actionId)
	{
		var plugin = _pluginsById.Values.FirstOrDefault(p => p.Actions.Any(a => a.ActionId == actionId));
		return Task.FromResult(plugin);
	}

	public Task<IEnumerable<PluginInfo>> GetAllPlugins()
	{
		return Task.FromResult(_pluginsById.Values.AsEnumerable());
	}

	public Task UpdateHeartbeat(string connectionId)
	{
		if (_pluginsByConnectionId.TryGetValue(connectionId, out var plugin))
		{
			plugin.LastHeartbeat = DateTime.UtcNow;
		}

		return Task.CompletedTask;
	}

	public Task<bool> HasAction(string pluginId, string actionId)
	{
		if (_pluginsById.TryGetValue(pluginId, out var plugin))
		{
			return Task.FromResult(plugin.Actions.Any(a => a.ActionId == actionId));
		}

		return Task.FromResult(false);
	}
}
