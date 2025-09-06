namespace MacroDeck.SDK.PluginSDK.Extensions;

/// <summary>
/// Type of MacroDeck extension
/// </summary>
public enum ExtensionType
{
	/// <summary>
	/// Internal integration built into the server
	/// </summary>
	Internal,

	/// <summary>
	/// External plugin connecting via SignalR
	/// </summary>
	Plugin
}