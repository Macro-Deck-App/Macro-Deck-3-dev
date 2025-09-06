using MacroDeck.Server.Application.Notifications;

namespace MacroDeck.Server.Application.Handlers;

public interface INotificationHandler<in TNotification>
	where TNotification : INotification
{
	ValueTask Handle(TNotification notification, CancellationToken cancellationToken);
}
