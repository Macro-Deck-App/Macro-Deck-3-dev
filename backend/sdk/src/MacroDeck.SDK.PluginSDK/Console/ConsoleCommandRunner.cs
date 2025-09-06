using Serilog;

namespace MacroDeck.SDK.PluginSDK.Console;

public class ConsoleCommandRunner
{
	private readonly Dictionary<string, IConsoleCommand> _commands;
	private readonly ILogger _logger = Log.ForContext<ConsoleCommandRunner>();

	public ConsoleCommandRunner()
	{
		_commands = new Dictionary<string, IConsoleCommand>();
	}

	public void RegisterCommand(IConsoleCommand command)
	{
		_commands[command.Name.ToLower()] = command;
	}

	public async Task StartConsoleLoop(CancellationToken cancellationToken = default)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				System.Console.Write("> ");
				var input = await ReadLineAsync(cancellationToken);

				if (string.IsNullOrWhiteSpace(input))
				{
					continue;
				}

				var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				var commandName = parts[0].ToLower();
				var args = parts.Skip(1).ToArray();

				if (_commands.TryGetValue(commandName, out var command))
				{
					await command.ExecuteAsync(args, cancellationToken);
				}
				else
				{
					System.Console.WriteLine($"Unknown command: {commandName}. Type 'help' for available commands.");
				}
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error executing command");
				System.Console.WriteLine($"Error executing command: {ex.Message}");
			}
		}
	}

	private static Task<string> ReadLineAsync(CancellationToken cancellationToken = default)
	{
		return Task.Run(() =>
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					if (System.Console.KeyAvailable)
					{
						return System.Console.ReadLine() ?? string.Empty;
					}

					Thread.Sleep(100);
				}

				return string.Empty;
			},
			cancellationToken);
	}
}
