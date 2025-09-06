namespace MacroDeck.SDK.PluginSDK.Console;

public class ExitCommand : IConsoleCommand
{
	public string Name => "exit";

	public string Description => "Exit the plugin";

	public Task ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
	{
		System.Console.WriteLine("Exiting plugin...");
		Environment.Exit(0);
		return Task.CompletedTask;
	}
}