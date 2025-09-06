using MacroDeck.Server.Domain.Entities.Base;

namespace MacroDeck.Server.Domain.Entities;

public class FolderEntity : BaseCreatedUpdatedEntity
{
	public required string DisplayName { get; set; }

	public int Index { get; set; }

	public int? ParentFolderRef { get; set; }

	public string Tree { get; set; } = string.Empty;

	public int Rows { get; set; }

	public int Columns { get; set; }

	public string? BackgroundColor { get; set; }

	public IList<WidgetEntity> Widgets { get; set; } = new List<WidgetEntity>();

	public FolderEntity? ParentFolder { get; set; }
}
