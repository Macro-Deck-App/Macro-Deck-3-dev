using MacroDeck.SDK.PluginSDK.Clients;
using MacroDeck.SDK.PluginSDK.Extensions;
using MacroDeck.SDK.PluginSDK.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.CommandLine;

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
		}, hostOption, portOption, sslOption);

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

		// Configure Serilog to also log to MacroDeck
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.WriteTo.Console()
			.WriteTo.MacroDeck(scope.ServiceProvider)
			.CreateLogger();

		Log.Information("Starting plugin {PluginId} connecting to {Host}:{Port}", 
			_pluginOptions.Id, _pluginOptions.Host, _pluginOptions.Port);

		var connected = await transport.Connect();
		if (!connected)
		{
			Log.Fatal("Failed to connect to MacroDeck server");
			Environment.Exit(1);
			return;
		}

		var registered = await transport.RegisterPlugin(_actions.GetActions().Select(t => 
			Activator.CreateInstance(t) as Actions.MacroDeckAction).Where(a => a != null)!);

		if (!registered)
		{
			Log.Fatal("Failed to register plugin with MacroDeck server");
			Environment.Exit(1);
			return;
		}

		Log.Information("Plugin successfully connected and registered");

		await host.RunAsync();
	}

	private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		Log.Logger.Fatal(e.ExceptionObject as Exception,
			"Unhandled exception {Terminating}",
			e.IsTerminating ? "Terminating" : "Not terminating");
	}
}
