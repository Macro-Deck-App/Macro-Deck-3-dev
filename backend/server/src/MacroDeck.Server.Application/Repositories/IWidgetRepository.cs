using MacroDeck.Server.Domain.Entities;

namespace MacroDeck.Server.Application.Repositories;

public interface IWidgetRepository
{
	Task<bool> WidgetAtPositionExists(int folderId, int row, int column);

	Task Create(WidgetEntity widget);

	Task Save();

	Task<bool> CheckOverlapByHeight(
		int folderRef,
		int? widgetId,
		int row,
		int? rowSpan,
		int column);

	Task<bool> CheckOverlapByWidth(
		int folderRef,
		int? widgetId,
		int row,
		int column,
		int? colSpan);

	ValueTask<WidgetEntity?> GetWidgetById(int id);

	Task DeleteWidgetById(int id);
}
