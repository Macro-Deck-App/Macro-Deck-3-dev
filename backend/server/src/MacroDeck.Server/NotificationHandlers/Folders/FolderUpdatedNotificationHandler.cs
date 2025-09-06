using MacroDeck.Server.Application.Handlers;
using MacroDeck.Server.Application.Notifications.Folders;
using MacroDeck.Server.Domain.Enums;
using MacroDeck.Server.Hubs;
using MacroDeck.Server.Hubs.Clients;
using MacroDeck.Server.Models.Common;
using Microsoft.AspNetCore.SignalR;

namespace MacroDeck.Server.NotificationHandlers.Folders;

public class FolderUpdatedNotificationHandler : INotificationHandler<FolderUpdatedNotification>
{
	private readonly IHubContext<SystemNotificationHub, ISystemNotificationClient> _eventHubContext;

	public FolderUpdatedNotificationHandler(
		IHubContext<SystemNotificationHub, ISystemNotificationClient> eventHubContext)
	{
		_eventHubContext = eventHubContext;
	}

	public async ValueTask Handle(FolderUpdatedNotification notification, CancellationToken cancellationToken)
	{
		await _eventHubContext.Clients.All
							  .PublishSystemNotification(new SystemNotificationModel(SystemNotificationType
																  .FolderUpdated,
															  notification.Folder.Id));
	}
}
