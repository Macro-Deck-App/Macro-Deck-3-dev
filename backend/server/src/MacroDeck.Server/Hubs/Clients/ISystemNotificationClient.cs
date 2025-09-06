using MacroDeck.Server.Models.Common;

namespace MacroDeck.Server.Hubs.Clients;

public interface ISystemNotificationClient
{
	Task PublishSystemNotification(SystemNotificationModel systemNotification);
}
