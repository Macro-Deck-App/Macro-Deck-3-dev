namespace MacroDeck.SDK.UI.Core;

/// <summary>
/// A UI view with internal state
/// </summary>
public abstract class StatefulMdUiView : MdUiView
{
    private MdUiState? _state;

    /// <summary>
    /// Creates the state for this view
    /// </summary>
    public abstract MdUiState CreateState();

    public void EnsureState()
    {
        if (_state == null)
        {
            _state = CreateState();
            _state.Attach(this);
            _state.InitState();
        }
    }

    public override MdUiView Build()
    {
        EnsureState();
        return _state!.Build();
    }
    
    public void Rebuild()
    {
        // Trigger rebuild through the framework
        MdUiFramework.NotifyViewChanged(this);
    }
    
    public void DisposeState()
    {
        _state?.Dispose();
        _state = null;
    }
}
