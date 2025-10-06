using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components;

/// <summary>
/// A scrollable view container
/// </summary>
public class MdScrollView : StatelessMdUiView
{
    public MdUiView? Child { get; set; }
    public ScrollDirection? Direction { get; set; }
    
    public MdScrollView(MdUiView child)
    {
        Child = child;
    }
    
    public override MdUiView Build()
    {
        return this;
    }
}

public enum ScrollDirection
{
    Vertical,
    Horizontal,
    Both
}
