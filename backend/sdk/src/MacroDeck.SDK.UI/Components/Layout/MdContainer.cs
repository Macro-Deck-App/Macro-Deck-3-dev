using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components.Layout;

/// <summary>
/// A container with a single child
/// </summary>
public class MdContainer : StatelessMdUiView
{
    public MdUiView? Child { get; set; }
    public string? BackgroundColor { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }
    public BorderRadius? BorderRadius { get; set; }
    public Border? Border { get; set; }
    
    public MdContainer(MdUiView? child = null)
    {
        Child = child;
    }
    
    public override MdUiView Build()
    {
        return this;
    }
}

public record BorderRadius(double TopLeft, double TopRight, double BottomRight, double BottomLeft)
{
    public static BorderRadius All(double radius) => new(radius, radius, radius, radius);
    public static BorderRadius Circular(double radius) => All(radius);
}

public record Border(double Width, string Color, BorderStyle Style = BorderStyle.Solid);

public enum BorderStyle
{
    Solid,
    Dashed,
    Dotted
}
