using MacroDeck.Server.Application.Services;
using MacroDeck.Server.Mappings;
using MacroDeck.Server.Models.Widgets;
using Microsoft.AspNetCore.Mvc;

namespace MacroDeck.Server.Controllers.Api.Folders;

[Route("api/folders/{folderId:int}/widgets")]
public class FolderWidgetsController : BaseController
{
	private readonly IFolderService _folderService;
	private readonly WidgetMapper _widgetMapper;
	private readonly IWidgetService _widgetService;

	public FolderWidgetsController(
		IFolderService folderService,
		IWidgetService widgetService,
		WidgetMapper widgetMapper)
	{
		_folderService = folderService;
		_widgetService = widgetService;
		_widgetMapper = widgetMapper;
	}

	[HttpPost]
	public async Task<IActionResult> CreateWidgetInFolder(int folderId, [FromBody] CreateWidgetModel createWidget)
	{
		var widget = await _widgetService.CreateWidgetInFolder(folderId,
															   createWidget.Type,
															   createWidget.Row,
															   createWidget.Column,
															   createWidget.Data);
		return Created(new Uri($"api/widgets/{widget.Id}", UriKind.Relative),
					   _widgetMapper.WidgetEntityToWidgetModel(widget));
	}

	[HttpGet]
	public async Task<IEnumerable<WidgetModel>> GetWidgetsByFolderId(int folderId)
	{
		var folder = await _folderService.GetFolderById(folderId);
		return _widgetMapper.WidgetEntityListToWidgetModelList(folder.Widgets);
	}
}
