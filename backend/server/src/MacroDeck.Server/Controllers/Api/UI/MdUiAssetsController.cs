using MacroDeck.Server.Application.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace MacroDeck.Server.UI.Controllers;

/// <summary>
/// Controller for serving UI assets
/// </summary>
[ApiController]
[Route("api/ui/assets")]
public class MdUiAssetsController : ControllerBase
{
    private readonly ILogger _logger = Log.ForContext<MdUiAssetsController>();
    private readonly MdAssetService _assetService;

    public MdUiAssetsController(MdAssetService assetService)
    {
        _assetService = assetService;
    }

    /// <summary>
    /// Gets an asset by ID
    /// </summary>
    [HttpGet("{assetId}")]
    public async Task<IActionResult> GetAsset(string assetId)
    {
        try
        {
            var asset = await _assetService.GetAssetAsync(assetId);
            if (asset == null)
            {
                return NotFound();
            }

            return File(asset.Data, asset.ContentType);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving asset {AssetId}", assetId);
            return StatusCode(500, "Failed to retrieve asset");
        }
    }
}
