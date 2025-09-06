using MacroDeck.Server.Application.Exceptions;
using MacroDeck.Server.Application.Notifications.Widgets;
using MacroDeck.Server.Application.Repositories;
using MacroDeck.Server.Domain.Entities;
using MacroDeck.Server.Domain.Enums;

namespace MacroDeck.Server.Application.Services.Implementations;

public class WidgetService : IWidgetService
{
	private readonly IFolderRepository _folderRepository;
	private readonly IPublisherService _publisherService;
	private readonly IWidgetRepository _widgetRepository;

	public WidgetService(
		IWidgetRepository widgetRepository,
		IFolderRepository folderRepository,
		IPublisherService publisherService)
	{
		_widgetRepository = widgetRepository;
		_folderRepository = folderRepository;
		_publisherService = publisherService;
	}

	public async Task<WidgetEntity> CreateWidgetInFolder(
		int folderId,
		WidgetType type,
		int row,
		int column,
		string? data)
	{
		var folder = await _folderRepository.GetFolderById(folderId);
		if (folder is null)
		{
			throw new ErrorCodeException(ErrorCode.NotFound);
		}

		await ValidateWidgetPosition(row, column, folder);

		var widget = new WidgetEntity
					 {
						 FolderRef = folder.Id,
						 Type = type,
						 Row = row,
						 Column = column,
						 RowSpan = 1,
						 CanIncreaseRowSpan = false,
						 ColSpan = 1,
						 CanIncreaseColSpan = false,
						 Data = data,
						 Folder = folder
					 };

		await _widgetRepository.Create(widget);

		await UpdateResizeParameters(widget);

		await _widgetRepository.Save();

		await _publisherService.PublishNotification(new WidgetCreatedNotification(widget));

		return widget;
	}

	public async Task<WidgetEntity> GetWidgetById(int id)
	{
		var widget = await _widgetRepository.GetWidgetById(id);
		if (widget is null)
		{
			throw new ErrorCodeException(ErrorCode.NotFound);
		}

		return widget;
	}

	public async Task UpdateWidget(int id, WidgetEntity widget)
	{
		var existingWidget = await _widgetRepository.GetWidgetById(id);
		if (existingWidget is null)
		{
			throw new ErrorCodeException(ErrorCode.NotFound);
		}

		existingWidget.Type = widget.Type;
		existingWidget.Row = widget.Row;
		existingWidget.Column = widget.Column;
		existingWidget.RowSpan = widget.RowSpan;
		existingWidget.ColSpan = widget.ColSpan;
		existingWidget.Data = widget.Data;

		await UpdateResizeParameters(existingWidget);
		await _widgetRepository.Save();
	}

	public async Task DeleteWidgetById(int id)
	{
		var widget = await _widgetRepository.GetWidgetById(id);
		if (widget is null)
		{
			throw new ErrorCodeException(ErrorCode.NotFound);
		}

		await _widgetRepository.DeleteWidgetById(id);

		await _publisherService.PublishNotification(new WidgetDeletedNotification(widget));
	}

	private async Task UpdateResizeParameters(WidgetEntity widget)
	{
		var folder = await _folderRepository.GetFolderById(widget.FolderRef);
		if (folder is null)
		{
			return;
		}

		var wouldOverlapWithOtherWidgetHeight = await _widgetRepository.CheckOverlapByHeight(folder.Id,
			 widget.Id,
			 widget.Row,
			 widget.RowSpan + 1,
			 widget.Column);

		var wouldOverlapWithOtherWidgetWidth = await _widgetRepository.CheckOverlapByWidth(folder.Id,
			 widget.Id,
			 widget.Row,
			 widget.Column,
			 widget.ColSpan + 1);

		widget.CanDecreaseRowSpan = widget.RowSpan > 1;
		widget.CanIncreaseRowSpan = widget.Row + widget.RowSpan < folder.Rows && !wouldOverlapWithOtherWidgetHeight;
		widget.CanDecreaseColSpan = widget.ColSpan > 1;
		widget.CanIncreaseColSpan
			= widget.Column + widget.ColSpan < folder.Columns && !wouldOverlapWithOtherWidgetWidth;
	}

	private async Task ValidateWidgetPosition(int row, int column, FolderEntity folder)
	{
		if (row < 0 || row > folder.Rows - 1)
		{
			throw new ErrorCodeException(ErrorCode.InvalidRow);
		}

		if (column < 0 || row > folder.Columns - 1)
		{
			throw new ErrorCodeException(ErrorCode.InvalidColumn);
		}

		if (await _widgetRepository.WidgetAtPositionExists(folder.Id, row, column))
		{
			throw new ErrorCodeException(ErrorCode.WidgetAlreadyExists);
		}
	}
}
