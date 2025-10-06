using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components;

/// <summary>
/// A clickable button
/// </summary>
public class MdButton : StatelessMdUiView
{
    public string? Text { get; set; }
    public Action? OnClick { get; set; }
    public MdUiView? Child { get; set; }
    
    /// <summary>
    /// Bootstrap button role (Primary, Secondary, etc.)
    /// </summary>
    public ButtonRole Role { get; set; } = ButtonRole.Primary;
    
    /// <summary>
    /// Custom background color (overrides Role if set)
    /// </summary>
    public string? BackgroundColor { get; set; }
    
    /// <summary>
    /// Custom text color (overrides Role if set)
    /// </summary>
    public string? TextColor { get; set; }
    
    public bool Disabled { get; set; }
    
    public MdButton(string text, Action? onClick = null)
    {
        Text = text;
        OnClick = onClick;
    }
    
    public MdButton(MdUiView child, Action? onClick = null)
    {
        Child = child;
        OnClick = onClick;
    }
    
    public override MdUiView Build()
    {
        return this;
    }
}

/// <summary>
/// Bootstrap button roles
/// </summary>
public enum ButtonRole
{
    Primary,
    Secondary,
    Success,
    Danger,
    Warning,
    Info,
    Light,
    Dark,
    Link
}
