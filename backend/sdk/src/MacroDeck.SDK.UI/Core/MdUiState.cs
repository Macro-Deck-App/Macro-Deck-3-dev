using MacroDeck.SDK.UI.DependencyInjection;
using Serilog;

namespace MacroDeck.SDK.UI.Core;

/// <summary>
///     State class for stateful views
/// </summary>
public abstract class MdUiState
{
	private static readonly ILogger Logger = Log.ForContext<MdUiState>();
	private readonly List<IState> _states = [];
	private StatefulMdUiView? _view;
	private string? _sessionId;

	internal void Attach(StatefulMdUiView view)
	{
		_view = view;
	}

	/// <summary>
	///     Sets the session ID for this state (called by framework)
	/// </summary>
	public void SetSessionId(string sessionId)
	{
		_sessionId = sessionId;
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

	/// <summary>
	///     Requests to open a link in the user's browser.
	///     Shows a confirmation dialog to the user and returns whether they approved.
	/// </summary>
	/// <param name="url">The URL to open</param>
	/// <returns>True if user approved, false if denied or timeout</returns>
	protected async Task<bool> OpenLink(string url)
	{
		if (string.IsNullOrEmpty(_sessionId))
		{
			Logger.Warning("OpenLink called but SessionId is not set");
			return false;
		}

		try
		{
			var linkService = MdUiServiceProvider.GetService<MacroDeck.SDK.UI.Services.IMdUiLinkService>();
			if (linkService == null)
			{
				Logger.Warning("LinkService not available");
				return false;
			}

			return await linkService.RequestLinkOpen(_sessionId, url);
		}
		catch (Exception ex)
		{
			Logger.Error(ex, "Failed to open link {Url}", url);
			return false;
		}
	}
}
