using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components.Layout;

/// <summary>
/// Arranges children in a grid
/// </summary>
public class MdGrid : StatelessMdUiView
{
    public List<MdUiView> Children { get; set; } = new();
    public int Columns { get; set; } = 2;
    public double? ColumnSpacing { get; set; }
    public double? RowSpacing { get; set; }
    
    public MdGrid(int columns, params MdUiView[] children)
    {
        Columns = columns;
        Children = children.ToList();
    }
    
    public override MdUiView Build()
    {
        return this;
    }
}
