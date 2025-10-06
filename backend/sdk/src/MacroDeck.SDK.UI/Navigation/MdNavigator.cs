namespace MacroDeck.SDK.UI.Navigation;

/// <summary>
/// Navigator for managing view navigation
/// </summary>
public class MdNavigator
{
    private readonly NavigationContext _context;
    
    public MdNavigator(NavigationContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Navigates to a view, clearing the navigation stack
    /// </summary>
    public void Set(string viewId)
    {
        _context.Set(viewId);
    }
    
    /// <summary>
    /// Pushes a new view onto the navigation stack
    /// </summary>
    public void Push(string viewId)
    {
        _context.Push(viewId);
    }
    
    /// <summary>
    /// Pops the current view from the navigation stack
    /// </summary>
    public bool Pop()
    {
        return _context.Pop();
    }
    
    /// <summary>
    /// Gets the current view ID
    /// </summary>
    public string? CurrentViewId => _context.CurrentViewId;
}
