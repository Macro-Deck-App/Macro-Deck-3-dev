export interface FolderModel {
  id: number;
   displayName: string;
   index: number;
   parentFolderRef: number | null;
   tree: string;
   rows: number;
   columns: number;
   backgroundColor: string | null;
}


/*

public class FolderModel
{
	public int Id { get; set; }

	public required string DisplayName { get; set; }

	public int Index { get; set; }

	public int? ParentFolderRef { get; set; }

	public required string Tree { get; set; }

	public int Rows { get; set; }

	public int Columns { get; set; }

	public string? BackgroundColor { get; set; }
}

 */
