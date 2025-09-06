using MacroDeck.SDK.PluginSDK.Extensions;
using MacroDeck.Server.Application.Integrations.Registry;
using MacroDeck.Server.Application.Utils;
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
		var internalExtensionTypes = IntegrationUtils.FindInternalExtensions();
		foreach (var type in internalExtensionTypes)
		{
			if (Activator.CreateInstance(type, _serviceProvider) is IMacroDeckExtension instance)
			{
				_integrationRegistry.Register(instance);
			}
		}

		await _integrationRegistry.EnableInternalExtensions(cancellationToken);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
