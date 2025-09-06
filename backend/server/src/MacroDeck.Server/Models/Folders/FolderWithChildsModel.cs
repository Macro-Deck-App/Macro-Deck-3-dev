namespace MacroDeck.Server.Models.Folders;

public class FolderWithChildsModel : FolderModel
{
	public IEnumerable<FolderWithChildsModel> Childs { get; set; } = [];
}
