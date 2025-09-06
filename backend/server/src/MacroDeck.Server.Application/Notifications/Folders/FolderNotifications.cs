using MacroDeck.Server.Domain.Entities;

namespace MacroDeck.Server.Application.Notifications.Folders;

public record FolderCreatedNotification(FolderEntity Folder) : INotification;

public record FolderUpdatedNotification(FolderEntity Folder) : INotification;

public record FolderDeletedNotification(FolderEntity Folder) : INotification;
