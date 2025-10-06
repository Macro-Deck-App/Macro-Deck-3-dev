using MacroDeck.SDK.UI.DependencyInjection;

namespace MacroDeck.SDK.UI.Core;

/// <summary>
///     State class for stateful views
/// </summary>
public abstract class MdUiState
{
	private readonly List<IState> _states = [];
	private StatefulMdUiView? _view;

	internal void Attach(StatefulMdUiView view)
	{
		_view = view;
	}

	/// <summary>
	///     Called when the state is initialized
	/// </summary>
	public virtual void InitState()
	{
	}

	/// <summary>
	///     Called when the state is disposed
	/// </summary>
	public virtual void Dispose()
	{
	}

	/// <summary>
	///     Build the UI
	/// </summary>
	public abstract MdUiView Build();

	/// <summary>
	///     Updates the state and triggers a rebuild
	/// </summary>
	protected void SetState(Action update)
	{
		update();
		_view?.Rebuild();
	}

	/// <summary>
	///     Creates a new reactive state
	/// </summary>
	protected State<T> CreateState<T>(T initialValue)
	{
		var state = new State<T>(initialValue);
		_states.Add(state);
		state.Changed += () => _view?.Rebuild();
		return state;
	}

	/// <summary>
	///     Creates a computed state
	/// </summary>
	protected Computed<T> CreateComputed<T>(Func<T> compute, params IState[] dependencies)
	{
		var computed = new Computed<T>(compute, dependencies);
		_states.Add(computed);
		computed.Changed += () => _view?.Rebuild();
		return computed;
	}

	/// <summary>
	///     Gets a service from the DI container
	/// </summary>
	protected T GetService<T>()
		where T : notnull
	{
		return MdUiServiceProvider.GetRequiredService<T>();
	}
}
