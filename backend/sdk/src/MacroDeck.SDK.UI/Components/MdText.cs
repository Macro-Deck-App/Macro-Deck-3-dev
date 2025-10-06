using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components;

/// <summary>
/// Displays text
/// </summary>
public class MdText : StatelessMdUiView
{
    public string? Text { get; set; }
    public double? FontSize { get; set; }
    public FontWeight? FontWeight { get; set; }
    public string? Color { get; set; }
    public TextAlign? TextAlign { get; set; }
    
    public MdText(string text)
    {
        Text = text;
    }
    
    public override MdUiView Build()
    {
        return this;
    }
}

public enum TextAlign
{
    Left,
    Center,
    Right,
    Justify
}

public enum FontWeight
{
    Normal,
    Bold,
    Lighter,
    Bolder
}
