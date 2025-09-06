using MacroDeck.Server.Application.Notifications;

namespace MacroDeck.Server.Application.Services;

public interface IPublisherService
{
	Task PublishNotification<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
		where TNotification : INotification;
}
