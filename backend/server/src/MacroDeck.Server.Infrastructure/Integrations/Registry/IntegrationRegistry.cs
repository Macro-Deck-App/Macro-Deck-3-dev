using System.Collections.Concurrent;
using MacroDeck.SDK.PluginSDK.Integration;
using MacroDeck.Server.Application.Integrations.Registry;
using MacroDeck.Server.Domain.Enums;
using Serilog;

namespace MacroDeck.Server.Infrastructure.Integrations.Registry;

public class IntegrationRegistry : IIntegrationRegistry
{
	private readonly ConcurrentDictionary<IntegrationType, MacroDeckIntegration> _integrations = new();
	private readonly ILogger _logger;

	public IntegrationRegistry(ILogger logger)
	{
		_logger = logger;
	}

	public void Register(IntegrationType type, MacroDeckIntegration integration)
	{
		_integrations.TryAdd(type, integration);
	}

	public async Task EnableInternalIntegrations(CancellationToken cancellationToken = default)
	{
		var tasks = _integrations.Where(x => x.Key == IntegrationType.Internal)
								 .Select(x => x.Value.Start(cancellationToken));
		await Task.WhenAll(tasks);

		_logger.Information("All internal integrations have been enabled");
	}
}
