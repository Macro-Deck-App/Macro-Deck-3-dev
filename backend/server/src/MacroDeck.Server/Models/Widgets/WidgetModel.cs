using MacroDeck.Server.Domain.Enums;

namespace MacroDeck.Server.Models.Widgets;

public class WidgetModel
{
	public required int Id { get; set; }

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
}
