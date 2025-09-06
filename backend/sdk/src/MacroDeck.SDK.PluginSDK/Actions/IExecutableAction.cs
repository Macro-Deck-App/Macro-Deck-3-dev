namespace MacroDeck.SDK.PluginSDK.Actions;

/// <summary>
/// Interface for actions that can be executed via SignalR
/// </summary>
public interface IExecutableAction
{
	/// <summary>
	/// Execute the action with the provided parameters
	/// </summary>
	/// <param name="parameters">Action parameters as key-value pairs</param>
	/// <returns>Task representing the async operation</returns>
	Task ExecuteAsync(Dictionary<string, object>? parameters = null);
}