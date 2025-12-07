using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components.Input;

/// <summary>
/// Text input field
/// </summary>
public class MdTextField : StatelessMdUiView
{
    public string? Value { get; set; }
    public string? Placeholder { get; set; }
    public string? Label { get; set; }
    public Action<string>? OnChanged { get; set; }
    public bool Disabled { get; set; }
    public TextFieldType? Type { get; set; }
    public int? MaxLength { get; set; }
    public bool Sensitive { get; set; } // If true, shows password toggle button
    
    public MdTextField()
    {
    }
    
    public override MdUiView Build()
    {
        return this;
    }
}

public enum TextFieldType
{
    Text,
    Password,
    Email,
    Number,
    Tel,
    Url
}
