using System.CommandLine;
using MacroDeck.SDK.PluginSDK.Clients;
using MacroDeck.SDK.PluginSDK.Console;
using MacroDeck.SDK.PluginSDK.Extensions;
using MacroDeck.SDK.PluginSDK.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MacroDeck.SDK.PluginSDK;

public sealed class Plugin
{
	private readonly ActionCollection _actions;
	private readonly PluginOptions _pluginOptions;
	private readonly IServiceCollection _services;

	public Plugin(PluginOptions pluginOptions, ActionCollection actions, IServiceCollection services)
	{
		_pluginOptions = pluginOptions;
		_actions = actions;
		_services = services;
	}

	public async Task Run(string[] args)
	{
		var hostOption = new Option<string>("--host", () => "127.0.0.1", "Host to connect to");
		var portOption = new Option<int>("--port", "Port to connect to") { IsRequired = true };
		var sslOption = new Option<bool>("--ssl", "Use SSL/TLS for connection");

		var rootCommand = new RootCommand("MacroDeck Plugin")
		{
			hostOption,
			portOption,
			sslOption
		};

		rootCommand.SetHandler(async (host, port, ssl) =>
			{
				_pluginOptions.Host = host;
				_pluginOptions.Port = port;
				_pluginOptions.UseSsl = ssl;

				await RunPlugin();
			},
			hostOption,
			portOption,
			sslOption);

		var result = await rootCommand.InvokeAsync(args);
		if (result != 0)
		{
			Environment.Exit(1);
		}
	}

	private async Task RunPlugin()
	{
		AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

		var host = Host.CreateDefaultBuilder()
			.ConfigureSerilog()
			.ConfigureServices(services =>
			{
				services.AddSingleton(_pluginOptions);
				services.AddSingleton<MacroDeckPluginTransport>();

				foreach (var service in _services)
				{
					services.Add(service);
				}

				foreach (var action in _actions.GetActions())
				{
					services.AddScoped(action);
				}
			})
			.Build();

		using var scope = host.Services.CreateScope();
		var transport = scope.ServiceProvider.GetRequiredService<MacroDeckPluginTransport>();

		Log.Information("Starting plugin {PluginId} connecting to {Host}:{Port}",
			_pluginOptions.Id,
			_pluginOptions.Host,
			_pluginOptions.Port);

		var connected = await transport.Connect();
		if (!connected)
		{
			Log.Fatal("Failed to connect to MacroDeck server");
			Environment.Exit(1);
			return;
		}

		var registered = await transport.RegisterPlugin(_actions.GetActions().ToList());

		if (!registered)
		{
			Log.Fatal("Failed to register plugin with MacroDeck server");
			Environment.Exit(1);
			return;
		}

		Log.Information("Plugin successfully connected and registered");

		// Start console command interface
		var consoleRunner = new ConsoleCommandRunner();

		// Register built-in commands
		var commands = new List<IConsoleCommand>
		{
			new ExitCommand()
		};

		// Add help command (needs reference to all commands)
		commands.Add(new HelpCommand(commands));

		foreach (var command in commands)
		{
			consoleRunner.RegisterCommand(command);
		}

		// Start console in background
		var consoleCancellationTokenSource = new CancellationTokenSource();
		_ = Task.Run(() => consoleRunner.StartConsoleLoop(consoleCancellationTokenSource.Token),
			consoleCancellationTokenSource.Token);

		// Run the host (this will block until the application shuts down)
		await host.RunAsync(consoleCancellationTokenSource.Token);

		// Stop console when host stops
		await consoleCancellationTokenSource.CancelAsync();
	}

	private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		Log.Logger.Fatal(e.ExceptionObject as Exception,
			"Unhandled exception {Terminating}",
			e.IsTerminating ? "Terminating" : "Not terminating");
	}
}
