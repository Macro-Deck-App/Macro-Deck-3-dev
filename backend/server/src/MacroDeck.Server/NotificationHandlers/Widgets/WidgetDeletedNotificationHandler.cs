using MacroDeck.Server.Application.Handlers;
using MacroDeck.Server.Application.Notifications.Widgets;
using MacroDeck.Server.Domain.Enums;
using MacroDeck.Server.Hubs;
using MacroDeck.Server.Hubs.Clients;
using MacroDeck.Server.Models.Common;
using MacroDeck.Server.Models.Widgets;
using Microsoft.AspNetCore.SignalR;

namespace MacroDeck.Server.NotificationHandlers.Widgets;

public class WidgetDeletedNotificationHandler : INotificationHandler<WidgetDeletedNotification>
{
	private readonly IHubContext<SystemNotificationHub, ISystemNotificationClient> _eventHubContext;

	public WidgetDeletedNotificationHandler(
		IHubContext<SystemNotificationHub, ISystemNotificationClient> eventHubContext)
	{
		_eventHubContext = eventHubContext;
	}

	public async ValueTask Handle(WidgetDeletedNotification notification, CancellationToken cancellationToken)
	{
		await _eventHubContext.Clients.All
							  .PublishSystemNotification(new SystemNotificationModel(SystemNotificationType
																  .WidgetDeleted,
															  new WidgetDeletedNotificationModel
															  {
																  WidgetId = notification.Widget.Id,
																  FolderId = notification.Widget.FolderRef
															  }));
	}
}
