namespace MacroDeck.SDK.UI.Core;

/// <summary>
/// Interface for all state objects
/// </summary>
public interface IState
{
    /// <summary>
    /// Event fired when the state value changes
    /// </summary>
    event Action? Changed;
    
    /// <summary>
    /// Gets the current value as object
    /// </summary>
    object? GetValue();
}
