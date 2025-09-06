using MacroDeck.SDK.PluginSDK.Configuration;
using MacroDeck.Server.Application.Services;

namespace MacroDeck.Server.Application.Providers;

public class InternalIntegrationConfigurationProvider : IIntegrationConfigurationProvider
{
	private readonly IIntegrationConfigurationService _integrationConfigurationService;

	public InternalIntegrationConfigurationProvider(IIntegrationConfigurationService integrationConfigurationService)
	{
		_integrationConfigurationService = integrationConfigurationService;
	}

	public Task<byte[]?> GetIntegrationConfiguration(string integrationId, string key)
	{
		return _integrationConfigurationService.GetDecryptedConfiguration(integrationId, key);
	}

	public Task SetIntegrationConfiguration(string integrationId, string key, byte[] value)
	{
		return _integrationConfigurationService.SetDecryptedConfiguration(integrationId, key, value);
	}
}
