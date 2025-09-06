using MacroDeck.Server.Domain.Enums;

namespace MacroDeck.Server.Models.Common;

public class SystemNotificationModel
{

	public SystemNotificationModel(SystemNotificationType type, object? parameters)
	{
		Type = type;
		Parameters = parameters;
	}

	public SystemNotificationType Type { get; set; }

	public object? Parameters { get; set; }
}
