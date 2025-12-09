using MacroDeck.SDK.UI.DependencyInjection;
using MacroDeck.SDK.UI.Services;
using Serilog;

namespace MacroDeck.SDK.UI.Core;

public abstract class MdUiState
{
	private static readonly ILogger Logger = Log.ForContext<MdUiState>();
	private readonly List<IState> _states = [];
	private StatefulMdUiView? _view;
	private string? _sessionId;

	internal void Attach(StatefulMdUiView view) => _view = view;

	public void SetSessionId(string sessionId) => _sessionId = sessionId;

	public virtual void InitState() { }

	public virtual void Dispose() { }

	public abstract MdUiView Build();

	protected void SetState(Action update)
	{
		update();
		_view?.Rebuild();
	}

	protected State<T> CreateState<T>(T initialValue)
	{
		var state = new State<T>(initialValue);
		_states.Add(state);
		state.Changed += () => _view?.Rebuild();
		return state;
	}

	protected Computed<T> CreateComputed<T>(Func<T> compute, params IState[] dependencies)
	{
		var computed = new Computed<T>(compute, dependencies);
		_states.Add(computed);
		computed.Changed += () => _view?.Rebuild();
		return computed;
	}

	protected T GetService<T>() where T : notnull => MdUiServiceProvider.GetRequiredService<T>();

	protected async Task<bool> OpenLink(string url)
	{
		if (string.IsNullOrEmpty(_sessionId))
		{
			Logger.Warning("OpenLink called without session");
			return false;
		}

		try
		{
			var linkService = MdUiServiceProvider.GetService<IMdUiLinkService>();
			if (linkService == null)
			{
				Logger.Warning("LinkService unavailable");
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
