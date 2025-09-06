using MacroDeck.SDK.PluginSDK.Actions;

namespace MacroDeck.SDK.PluginSDK;

public sealed class ActionCollection
{
	private readonly List<Type> _actions = [];

	public void Add<T>()
		where T : MacroDeckAction
	{
		_actions.Add(typeof(T));
	}

	internal ICollection<Type> GetActions()
	{
		return _actions;
	}
}
