using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components.Input;

/// <summary>
/// Toggle switch input
/// </summary>
public class MdSwitch : StatelessMdUiView
{
    public bool Value { get; set; }
    public string? Label { get; set; }
    public Action<bool>? OnChanged { get; set; }
    public bool Disabled { get; set; }
    
    public MdSwitch(bool value = false)
    {
        Value = value;
    }
    
    public override MdUiView Build()
    {
        return this;
    }
}
