using MacroDeck.SDK.PluginSDK.Actions;

namespace MacroDeck.SDK.PluginSDK.Integration.Actions;

public interface IActionsIntegration
{
	List<MacroDeckAction> GetActions();
}
