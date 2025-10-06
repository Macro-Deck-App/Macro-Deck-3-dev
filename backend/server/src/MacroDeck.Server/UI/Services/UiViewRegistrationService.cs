using MacroDeck.SDK.UI.Registry;
using MacroDeck.Server.Application.UI.Services;
using MacroDeck.Server.UI.Views;
using Serilog;
using ILogger = Serilog.ILogger;

namespace MacroDeck.Server.UI.Services;

/// <summary>
///     Background service that registers UI views on startup
/// </summary>
public class UiViewRegistrationService : IHostedService
{
	private readonly ILogger _logger = Log.ForContext<UiViewRegistrationService>();
	private readonly IMdUiRegistry _registry;
	private readonly MdUiStateManager _stateManager;
	private readonly IMdUiUpdateService _updateService;

	public UiViewRegistrationService(
		IMdUiRegistry registry,
		MdUiStateManager stateManager,
		IMdUiUpdateService updateService)
	{
		_registry = registry;
		_stateManager = stateManager;
		_updateService = updateService;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.Information("Registering UI views...");

		try
		{
			// Initialize the state manager with the update service
			_stateManager.SetUpdateService(_updateService);
			
			// Register server views
			RegisterServerViews();

			_logger.Information("UI views registered successfully");
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Failed to register UI views");
			throw;
		}

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.Debug("UiViewRegistrationService stopping");
		return Task.CompletedTask;
	}

	private void RegisterServerViews()
	{
		// Register TestCounterView
		_registry.RegisterView(new MdUiViewMetadata
		{
			ViewId = "server.TestCounterView",
			ViewType = typeof(TestCounterView),
			Namespace = "server",
			TransportMode = TransportMode.Direct
		});

		_logger.Debug("Registered view: server.TestCounterView");

		// TODO: Add more server views here as they are created
		// Example:
		// _registry.RegisterView(new MdUiViewMetadata
		// {
		//     ViewId = "server.SettingsView",
		//     ViewType = typeof(SettingsView),
		//     Namespace = "server",
		//     TransportMode = TransportMode.Direct
		// });
	}
}
