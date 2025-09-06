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
				a.ActionName
			})
		});

		return Ok(pluginDtos);
	}

	[HttpPost("/api/dev/actions/{actionId}/invoke")]
	public async Task<IActionResult> InvokeAction(
		string actionId,
		[FromBody] object? actionConfig = null)
	{
		// Find plugin by action ID since action IDs are unique
		var plugin = await _pluginRegistry.GetPluginByActionId(actionId);
		if (plugin == null)
		{
			return NotFound($"Action '{actionId}' not found");
		}

		var success = await _actionInvoker.InvokeAction(plugin.PluginId, actionId, actionConfig);

		if (success)
		{
			return Ok(new { Message = $"Action '{actionId}' invoked successfully" });
		}

		return BadRequest($"Failed to invoke action '{actionId}'");
	}
}
