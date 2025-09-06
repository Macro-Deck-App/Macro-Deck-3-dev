using MacroDeck.Server.Domain.Entities;
using MacroDeck.Server.Domain.Enums;

namespace MacroDeck.Server.Application.Services;

public interface IWidgetService
{
	Task<WidgetEntity> CreateWidgetInFolder(int folderId, WidgetType type, int row, int column, string? data);

	Task<WidgetEntity> GetWidgetById(int id);

	Task UpdateWidget(int id, WidgetEntity widget);

	Task DeleteWidgetById(int id);
}
