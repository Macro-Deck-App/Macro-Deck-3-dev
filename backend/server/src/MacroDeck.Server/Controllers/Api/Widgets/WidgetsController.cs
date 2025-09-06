using MacroDeck.Server.Application.Services;
using MacroDeck.Server.Domain.Entities;
using MacroDeck.Server.Mappings;
using MacroDeck.Server.Models.Widgets;
using Microsoft.AspNetCore.Mvc;

namespace MacroDeck.Server.Controllers.Api.Widgets;

[Route("api/widgets")]
public class WidgetsController : BaseController
{
	private readonly WidgetMapper _widgetMapper;
	private readonly IWidgetService _widgetService;

	public WidgetsController(
		IWidgetService widgetService,
		WidgetMapper widgetMapper)
	{
		_widgetService = widgetService;
		_widgetMapper = widgetMapper;
	}

	[HttpGet("{id:int}")]
	public async Task<WidgetModel> GetWidgetById(int id)
	{
		var widget = await _widgetService.GetWidgetById(id);
		return _widgetMapper.WidgetEntityToWidgetModel(widget);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> UpdateWidget(int id, [FromBody] WidgetModel widget)
	{
		var widgetEntity = new WidgetEntity
		{
			FolderRef = 0, // Will be ignored by service
			Type = widget.Type,
			Row = widget.Row,
			Column = widget.Column,
			RowSpan = widget.RowSpan,
			ColSpan = widget.ColSpan,
			Data = widget.Data,
			Folder = null! // Will be ignored by service
		};
		
		await _widgetService.UpdateWidget(id, widgetEntity);
		return Ok();
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> DeleteWidget(int id)
	{
		await _widgetService.DeleteWidgetById(id);
		return Ok();
	}
}
