namespace MacroDeck.SDK.UI.Navigation;

/// <summary>
/// Context for navigation within a view namespace
/// </summary>
public class NavigationContext
{
    private readonly Stack<string> _stack = new();
    private readonly string _namespace;
    
    public NavigationContext(string viewNamespace)
    {
        _namespace = viewNamespace;
    }
    
    /// <summary>
    /// Current view ID
    /// </summary>
    public string? CurrentViewId => _stack.Count > 0 ? _stack.Peek() : null;
    
    /// <summary>
    /// Event raised when navigation changes
    /// </summary>
    public event Action<string?>? NavigationChanged;
    
    /// <summary>
    /// Navigates to a view, clearing the stack
    /// </summary>
    public void Set(string viewId)
    {
        ValidateViewId(viewId);
        _stack.Clear();
        _stack.Push(viewId);
        NavigationChanged?.Invoke(viewId);
    }
    
    /// <summary>
    /// Pushes a view onto the stack
    /// </summary>
    public void Push(string viewId)
    {
        ValidateViewId(viewId);
        _stack.Push(viewId);
        NavigationChanged?.Invoke(viewId);
    }
    
    /// <summary>
    /// Pops the current view from the stack
    /// </summary>
    public bool Pop()
    {
        if (_stack.Count <= 1)
        {
            return false; // Can't pop the root view
        }
        
        _stack.Pop();
        NavigationChanged?.Invoke(CurrentViewId);
        return true;
    }
    
    /// <summary>
    /// Validates that the view ID belongs to this context's namespace
    /// </summary>
    private void ValidateViewId(string viewId)
    {
        if (!viewId.StartsWith(_namespace + "."))
        {
            throw new InvalidOperationException(
                $"Cannot navigate to view '{viewId}' from namespace '{_namespace}'. " +
                "Views can only navigate within their own namespace.");
        }
    }
}
