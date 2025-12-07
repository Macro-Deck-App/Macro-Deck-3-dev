using MacroDeck.Server.Application.UI.Registry;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace MacroDeck.Server.Controllers.Api.UI;

/// <summary>
///     DTO for view metadata
/// </summary>
public class ViewMetadataDto
{
	public required string ViewId { get; set; }
	public required string Namespace { get; set; }
	public required string TransportMode { get; set; }
	public string? PluginId { get; set; }
}

/// <summary>
///     Controller for UI view registry
/// </summary>
[ApiController]
[Route("api/ui/views")]
public class MdUiViewController : ControllerBase
{
	private readonly ILogger _logger = Log.ForContext<MdUiViewController>();
	private readonly MdUiRegistry _registry;

	public MdUiViewController(MdUiRegistry registry)
	{
		_registry = registry;
	}

    /// <summary>
    ///     Gets all registered views
    /// </summary>
    [HttpGet]
	public IActionResult GetRegisteredViews()
	{
		try
		{
			var views = _registry.GetAllRegisteredViews();
			var dtos = views.Select(v => new ViewMetadataDto
			{
				ViewId = v.ViewId,
				Namespace = v.Namespace,
				TransportMode = v.TransportMode.ToString(),
				PluginId = v.PluginId
			});
			return Ok(dtos);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error retrieving registered views");
			return StatusCode(500, "Failed to retrieve registered views");
		}
	}

    /// <summary>
    ///     Gets a specific view by ID
    /// </summary>
    [HttpGet("{viewId}")]
	public IActionResult GetView(string viewId)
	{
		try
		{
			var metadata = _registry.GetViewMetadata(viewId);
			if (metadata == null)
			{
				return NotFound();
			}

			var dto = new ViewMetadataDto
			{
				ViewId = metadata.ViewId,
				Namespace = metadata.Namespace,
				TransportMode = metadata.TransportMode.ToString(),
				PluginId = metadata.PluginId
			};

			return Ok(dto);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error retrieving view {ViewId}", viewId);
			return StatusCode(500, "Failed to retrieve view");
		}
	}
}
