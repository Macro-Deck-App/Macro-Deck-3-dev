using MacroDeck.Server.Application.Handlers;
using MacroDeck.Server.Application.Notifications.Folders;
using MacroDeck.Server.Domain.Enums;
using MacroDeck.Server.Hubs;
using MacroDeck.Server.Hubs.Clients;
using MacroDeck.Server.Models.Common;
using Microsoft.AspNetCore.SignalR;

namespace MacroDeck.Server.NotificationHandlers.Folders;

public class FolderDeletedNotificationHandler : INotificationHandler<FolderDeletedNotification>
{
	private readonly IHubContext<SystemNotificationHub, ISystemNotificationClient> _eventHubContext;

	public FolderDeletedNotificationHandler(
		IHubContext<SystemNotificationHub, ISystemNotificationClient> eventHubContext)
	{
		_eventHubContext = eventHubContext;
	}

	public async ValueTask Handle(FolderDeletedNotification notification, CancellationToken cancellationToken)
	{
		await _eventHubContext.Clients.All
							  .PublishSystemNotification(new SystemNotificationModel(SystemNotificationType
																  .FolderDeleted,
															  notification.Folder.Id));
	}
}
