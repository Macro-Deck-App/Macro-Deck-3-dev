using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components.Input;

/// <summary>
/// Radio button input
/// </summary>
public class MdRadio<T> : StatelessMdUiView
{
    public T? Value { get; set; }
    public List<MdRadioOption<T>> Options { get; set; } = new();
    public Action<T>? OnChanged { get; set; }
    public bool Disabled { get; set; }
    
    public MdRadio(params MdRadioOption<T>[] options)
    {
        Options = options.ToList();
    }
    
    public override MdUiView Build()
    {
        return this;
    }
}

public class MdRadioOption<T>
{
    public required T Value { get; init; }
    public required string Label { get; init; }
}
