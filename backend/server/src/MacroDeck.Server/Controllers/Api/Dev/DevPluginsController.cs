using System.Text.Json;
using MacroDeck.Server.Application.Plugins.Services;
using Microsoft.AspNetCore.Mvc;

namespace MacroDeck.Server.Controllers.Api.Dev;

[Route("api/dev/plugins")]
public class DevPluginsController : BaseController
{
	private readonly IPluginActionInvoker _actionInvoker;
	private readonly IPluginRegistry _pluginRegistry;

	public DevPluginsController(
		IPluginRegistry pluginRegistry,
		IPluginActionInvoker actionInvoker)
	{
		_pluginRegistry = pluginRegistry;
		_actionInvoker = actionInvoker;
	}

	[HttpGet]
	public async Task<IActionResult> GetPlugins()
	{
		var plugins = await _pluginRegistry.GetAllPlugins();

		var pluginDtos = plugins.Select(p => new
		{
			p.PluginId,
			p.Name,
			p.Version,
			p.SdkVersion,
			p.ConnectedAt,
			p.LastHeartbeat,
			Actions = p.Actions.Select(a => new
			{
				a.ActionId,
				a.ActionName,
				a.Description,
				ConfigurationFields = a.ConfigurationFields.Select(cf => new
				{
					cf.FieldName,
					cf.FieldType,
					cf.Required,
					cf.DefaultValue
				})
			})
		});

		return Ok(pluginDtos);
	}

	[HttpPost("{pluginId}/callAction/{actionId}")]
	public async Task<IActionResult> CallAction(
		string pluginId,
		string actionId,
		[FromBody] object? actionConfig = null)
	{
		var plugin = await _pluginRegistry.GetPlugin(pluginId);
		if (plugin == null)
		{
			return NotFound($"Plugin '{pluginId}' not found");
		}

		if (!await _pluginRegistry.HasAction(pluginId, actionId))
		{
			return NotFound($"Action '{actionId}' not found in plugin '{pluginId}'");
		}

		var configJson = actionConfig != null ? JsonSerializer.Serialize(actionConfig) : null;

		var success = await _actionInvoker.InvokeAction(pluginId, actionId, configJson);

		if (success)
		{
			return Ok(new { Message = $"Action '{actionId}' invoked successfully" });
		}

		return BadRequest($"Failed to invoke action '{actionId}'");
	}
}
