using System.ComponentModel;

namespace MacroDeck.Server.Domain.Enums;

public enum SystemNotificationType
{
	[Description("Folder was created.")]
	FolderCreated = 100,

	[Description("Folder was updated.")]
	FolderUpdated = 101,

	[Description("Folder was deleted.")]
	FolderDeleted = 102,

	[Description("Widget was created.")]
	WidgetCreated = 200,

	[Description("Widget was updated.")]
	WidgetUpdated = 201,

	[Description("Widget was deleted.")]
	WidgetDeleted = 202
}
