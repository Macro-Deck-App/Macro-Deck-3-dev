using System.ComponentModel;

namespace MacroDeck.Server.Domain.Enums;

public enum ErrorCode
{
	[Description("An unknown error occurred. Check the logs for more information.")]
	Unknown = -1000,

	[Description("The requested resource was not found.")]
	NotFound = -1001,

	[Description("Invalid display name.")]
	InvalidDisplayName = -1002,

	[Description("The parent folder does not exist.")]
	ParentFolderDoesNotExist = -2000,

	[Description("A folder with the same name already exists.")]
	FolderAlreadyExists = -2001,

	[Description("Invalid row.")]
	InvalidRow = -3000,

	[Description("Invalid column.")]
	InvalidColumn = -3001,

	[Description("A widget with the same position already exists.")]
	WidgetAlreadyExists = -3002
}
