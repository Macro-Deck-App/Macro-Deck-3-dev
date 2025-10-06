using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components.Input;

/// <summary>
/// Checkbox input
/// </summary>
public class MdCheckbox : StatelessMdUiView
{
    public bool Value { get; set; }
    public string? Label { get; set; }
    public Action<bool>? OnChanged { get; set; }
    public bool Disabled { get; set; }
    
    public MdCheckbox(bool value = false)
    {
        Value = value;
    }
    
    public override MdUiView Build()
    {
        return this;
    }
}
