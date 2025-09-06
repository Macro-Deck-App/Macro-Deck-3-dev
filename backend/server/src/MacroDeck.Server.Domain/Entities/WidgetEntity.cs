using MacroDeck.Server.Domain.Entities.Base;
using MacroDeck.Server.Domain.Enums;

namespace MacroDeck.Server.Domain.Entities;

public class WidgetEntity : BaseCreatedUpdatedEntity
{
	public required int FolderRef { get; set; }

	public required WidgetType Type { get; set; }

	public required int Row { get; set; }

	public required int Column { get; set; }

	public int RowSpan { get; set; } = 1;

	public bool CanIncreaseRowSpan { get; set; }

	public bool CanDecreaseRowSpan { get; set; }

	public int ColSpan { get; set; } = 1;

	public bool CanIncreaseColSpan { get; set; }

	public bool CanDecreaseColSpan { get; set; }

	public string? Data { get; set; }

	public required FolderEntity Folder { get; set; }
}
