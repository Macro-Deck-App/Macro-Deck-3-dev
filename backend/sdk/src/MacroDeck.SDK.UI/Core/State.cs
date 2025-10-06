namespace MacroDeck.SDK.UI.Core;

/// <summary>
/// Represents a reactive state value
/// </summary>
public class State<T> : IState
{
    private T _value;
    
    public event Action? Changed;

    public State(T initial)
    {
        _value = initial;
    }

    public T Value
    {
        get => _value;
        set
        {
            if (!Equals(_value, value))
            {
                _value = value;
                Changed?.Invoke();
            }
        }
    }
    
    public object? GetValue() => _value;
}
