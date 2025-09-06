using MacroDeck.SDK.PluginSDK.Configuration;
using MacroDeck.Server.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MacroDeck.Server.Application.Providers;

public class InternalIntegrationConfigurationProvider : IIntegrationConfigurationProvider
{
	private readonly IServiceProvider _serviceProvider;

	public InternalIntegrationConfigurationProvider(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public async Task<byte[]?> GetIntegrationConfiguration(string integrationId, string key)
	{
		using var scope = _serviceProvider.CreateScope();
		var service = scope.ServiceProvider.GetRequiredService<IIntegrationConfigurationService>();
		return await service.GetDecryptedConfiguration(integrationId, key);
	}

	public async Task SetIntegrationConfiguration(string integrationId, string key, byte[] value)
	{
		using var scope = _serviceProvider.CreateScope();
		var service = scope.ServiceProvider.GetRequiredService<IIntegrationConfigurationService>();
		await service.SetDecryptedConfiguration(integrationId, key, value);
	}
}
