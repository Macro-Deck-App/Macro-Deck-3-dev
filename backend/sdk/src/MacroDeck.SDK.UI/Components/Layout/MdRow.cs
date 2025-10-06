using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components.Layout;

/// <summary>
/// Arranges children horizontally
/// </summary>
public class MdRow : StatelessMdUiView
{
    public List<MdUiView> Children { get; set; } = new();
    public MainAxisAlignment? MainAxisAlignment { get; set; }
    public CrossAxisAlignment? CrossAxisAlignment { get; set; }
    public double? Spacing { get; set; }
    
    public MdRow(params MdUiView[] children)
    {
        Children = children.ToList();
    }
    
    public override MdUiView Build()
    {
        return this;
    }
}
