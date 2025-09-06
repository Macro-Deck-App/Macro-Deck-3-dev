using MacroDeck.Server.Domain.Enums;

namespace MacroDeck.Server.Models.Widgets;

public class CreateWidgetModel
{
	public required WidgetType Type { get; set; }

	public required int Row { get; set; }

	public required int Column { get; set; }

	public string? Data { get; set; }
}
