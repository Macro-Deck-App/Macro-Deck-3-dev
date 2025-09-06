using MacroDeck.Server.Application.Integrations.Registry;
using Microsoft.Extensions.Hosting;

namespace MacroDeck.Server.Application.HostedServices;

public class LoadInternalIntegrationsHostedService : IHostedService
{
	private readonly IIntegrationRegistry _integrationRegistry;
	private readonly IServiceProvider _serviceProvider;

	public LoadInternalIntegrationsHostedService(
		IIntegrationRegistry integrationRegistry,
		IServiceProvider serviceProvider)
	{
		_integrationRegistry = integrationRegistry;
		_serviceProvider = serviceProvider;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		/*	var internalIntegrationTypes = IntegrationUtils.FindInternalIntegrations();
			foreach (var type in internalIntegrationTypes)
			{
				if (Activator.CreateInstance(type, _serviceProvider) is InternalIntegration instance)
				{
					_integrationRegistry.Register(IntegrationType.Internal, instance);
				}
			}

			await _integrationRegistry.EnableInternalIntegrations(cancellationToken);*/
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
