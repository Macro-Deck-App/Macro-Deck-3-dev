using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components.Layout;

/// <summary>
/// Arranges children vertically
/// </summary>
public class MdColumn : StatelessMdUiView
{
    public List<MdUiView> Children { get; set; } = new();
    public MainAxisAlignment? MainAxisAlignment { get; set; }
    public CrossAxisAlignment? CrossAxisAlignment { get; set; }
    public double? Spacing { get; set; }
    
    public MdColumn(params MdUiView[] children)
    {
        Children = children.ToList();
    }
    
    public override MdUiView Build()
    {
        return this;
    }
}
