namespace MacroDeck.SDK.UI.Core;

/// <summary>
/// Central coordination for the UI framework
/// </summary>
public static class MdUiFramework
{
    private static Action<MdUiView>? _viewChangedCallback;
    
    /// <summary>
    /// Sets the callback for view changes
    /// </summary>
    public static void SetViewChangedCallback(Action<MdUiView> callback)
    {
        _viewChangedCallback = callback;
    }
    
    /// <summary>
    /// Notifies that a view has changed and needs to be rebuilt
    /// </summary>
    public static void NotifyViewChanged(MdUiView view)
    {
        _viewChangedCallback?.Invoke(view);
    }
}
