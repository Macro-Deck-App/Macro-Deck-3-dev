namespace MacroDeck.SDK.PluginSDK.Actions;

/// <summary>
///     Base class for simple actions that execute when triggered
/// </summary>
public abstract class SimpleMacroDeckAction : MacroDeckAction
{
	/// <summary>
	///     Execute the simple action
	/// </summary>
	/// <returns>Task representing the async operation</returns>
	public abstract Task OnInvoke();
}
