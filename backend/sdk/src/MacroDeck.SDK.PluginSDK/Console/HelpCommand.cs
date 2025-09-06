namespace MacroDeck.SDK.PluginSDK.Console;

public class HelpCommand : IConsoleCommand
{
	private readonly IEnumerable<IConsoleCommand> _commands;

	public HelpCommand(IEnumerable<IConsoleCommand> commands)
	{
		_commands = commands;
	}

	public string Name => "help";

	public string Description => "Show available commands";

	public Task ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
	{
		System.Console.WriteLine("Available commands:");
		foreach (var command in _commands.OrderBy(c => c.Name))
		{
			System.Console.WriteLine($"  {command.Name.PadRight(15)} - {command.Description}");
		}

		return Task.CompletedTask;
	}
}