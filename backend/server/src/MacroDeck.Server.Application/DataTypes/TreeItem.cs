namespace MacroDeck.Server.Application.DataTypes;

public class TreeItem<T>
{
	public required T Item { get; set; }

	public IEnumerable<TreeItem<T>> Children { get; set; } = [];
}
