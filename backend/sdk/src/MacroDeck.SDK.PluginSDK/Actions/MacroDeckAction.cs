namespace MacroDeck.SDK.PluginSDK.Actions;

public abstract class MacroDeckAction
{
	/// <summary>
	///     A human-readable name of the action.
	/// </summary>
	public abstract string Name { get; }

	/// <summary>
	///     A unique identifier for the action, usually in reverse domain name notation (e.g., "com.example.plugin.myaction").
	///     Make sure this ID is unique to avoid conflicts with other actions.
	///     Only use lowercase letters, numbers, and dots. Avoid special characters and spaces.
	/// </summary>
	public abstract string Id { get; }
}
