using MacroDeck.Server.Application.Repositories;
using MacroDeck.Server.Domain.Entities;
using MacroDeck.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MacroDeck.Server.Infrastructure.Repositories;

public class WidgetRepository : IWidgetRepository
{
	private readonly DatabaseContext _context;

	public WidgetRepository(DatabaseContext context)
	{
		_context = context;
	}

	public Task<bool> WidgetAtPositionExists(int folderId, int row, int column)
	{
		return _context.Widgets.AsNoTracking()
					   .Where(x => x.FolderRef == folderId)
					   .Where(x => row >= x.Row && row < x.Row + x.RowSpan)
					   .Where(x => column >= x.Column && column < x.Column + x.ColSpan)
					   .AnyAsync();
	}

	public async Task Create(WidgetEntity widget)
	{
		await _context.Widgets.AddAsync(widget);
		await _context.SaveChangesAsync();
	}

	public Task Save()
	{
		return _context.SaveChangesAsync();
	}

	public Task<bool> CheckOverlapByHeight(int folderRef, int? widgetId, int row, int? rowSpan, int column)
	{
		rowSpan ??= 1;

		var newWidgetBottomRow = row + rowSpan.Value - 1;

		return _context.Widgets.AsNoTracking()
					   .Where(x => x.FolderRef == folderRef)
					   .Where(x => widgetId == null || x.Id != widgetId)
					   .Where(x => x.Column == column)
					   .AnyAsync(x =>
									 x.Row <= newWidgetBottomRow
									 && x.Row + x.RowSpan - 1 >= row);
	}

	public Task<bool> CheckOverlapByWidth(int folderRef, int? widgetId, int row, int column, int? colSpan)
	{
		colSpan ??= 1;

		var newWidgetRightColumn = column + colSpan.Value - 1;

		return _context.Widgets.AsNoTracking()
					   .Where(x => x.FolderRef == folderRef)
					   .Where(x => widgetId == null || x.Id != widgetId)
					   .Where(x => x.Row == row)
					   .AnyAsync(x => x.Column <= newWidgetRightColumn && x.Column + x.ColSpan - 1 >= column);
	}

	public ValueTask<WidgetEntity?> GetWidgetById(int id)
	{
		return _context.Widgets.FindAsync(id);
	}

	public Task DeleteWidgetById(int id)
	{
		return _context.Widgets.Where(x => x.Id == id).ExecuteDeleteAsync();
	}
}
