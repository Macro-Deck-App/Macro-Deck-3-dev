using MacroDeck.SDK.PluginSDK.Messages;

namespace MacroDeck.SDK.PluginSDK.Hubs;

/// <summary>
/// Interface defining type-safe methods that the server can call on connected plugin clients
/// </summary>
public interface IPluginCommunicationClient
{
	/// <summary>
	/// Server requests plugin to invoke a specific action
	/// </summary>
	/// <param name="message">The action invocation message</param>
	/// <returns>Task representing the async operation</returns>
	Task InvokeAction(InvokeActionMessage message);
	
	/// <summary>
	/// Server requests plugin to shutdown gracefully
	/// </summary>
	/// <returns>Task representing the async operation</returns>
	Task RequestShutdown();
}