namespace MacroDeck.Server.Models.Folders;

public class CreateFolderModel
{
	public string DisplayName { get; set; } = string.Empty;

	public int? ParentFolderId { get; set; }
}
