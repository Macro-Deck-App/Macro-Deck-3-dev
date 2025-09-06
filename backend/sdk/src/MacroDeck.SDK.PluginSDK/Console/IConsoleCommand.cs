namespace MacroDeck.SDK.PluginSDK.Console;

/// <summary>
/// Interface for interactive console commands
/// </summary>
public interface IConsoleCommand
{
	/// <summary>
	/// Name of the command
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Description of what the command does
	/// </summary>
	string Description { get; }

	/// <summary>
	/// Execute the command
	/// </summary>
	/// <param name="args">Command arguments</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the command execution</returns>
	Task ExecuteAsync(string[] args, CancellationToken cancellationToken = default);
}