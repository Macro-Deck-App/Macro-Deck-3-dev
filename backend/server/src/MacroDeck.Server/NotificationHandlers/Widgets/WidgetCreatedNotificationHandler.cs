using MacroDeck.Server.Application.Handlers;
using MacroDeck.Server.Application.Notifications.Widgets;
using MacroDeck.Server.Domain.Enums;
using MacroDeck.Server.Hubs;
using MacroDeck.Server.Hubs.Clients;
using MacroDeck.Server.Models.Common;
using Microsoft.AspNetCore.SignalR;

namespace MacroDeck.Server.NotificationHandlers.Widgets;

public class WidgetCreatedNotificationHandler : INotificationHandler<WidgetCreatedNotification>
{
	private readonly IHubContext<SystemNotificationHub, ISystemNotificationClient> _eventHubContext;

	public WidgetCreatedNotificationHandler(
		IHubContext<SystemNotificationHub, ISystemNotificationClient> eventHubContext)
	{
		_eventHubContext = eventHubContext;
	}

	public async ValueTask Handle(WidgetCreatedNotification notification, CancellationToken cancellationToken)
	{
		await _eventHubContext.Clients.All
							  .PublishSystemNotification(new SystemNotificationModel(SystemNotificationType
																  .WidgetCreated,
															  notification.Widget.Id));
	}
}
