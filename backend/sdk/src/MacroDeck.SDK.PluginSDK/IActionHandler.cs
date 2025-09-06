using MacroDeck.SDK.PluginSDK.Actions;

namespace MacroDeck.SDK.PluginSDK;

public interface IActionHandler<TAction>
	where TAction : MacroDeckAction
{
}
