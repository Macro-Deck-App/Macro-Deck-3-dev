using MacroDeck.Server.Application.Handlers;
using MacroDeck.Server.Application.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace MacroDeck.Server.Application.Services.Implementations;

public class PublisherService : IPublisherService
{
	private readonly IServiceProvider _serviceProvider;

	public PublisherService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public async Task PublishNotification<TNotification>(
		TNotification notification,
		CancellationToken cancellationToken = default)
		where TNotification : INotification
	{
		var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();

		foreach (var handler in handlers)
		{
			await handler.Handle(notification, cancellationToken);
		}
	}
}
