using MacroDeck.Server.Application.Repositories;
using MacroDeck.Server.Domain.Entities;
using Microsoft.AspNetCore.DataProtection;
using Serilog;

namespace MacroDeck.Server.Application.Services.Implementations;

public class IntegrationConfigurationService : IIntegrationConfigurationService
{
	private readonly IDataProtectionProvider _dataProtectionProvider;
	private readonly IIntegrationConfigurationRepository _integrationConfigurationRepository;
	private readonly ILogger _logger;

	public IntegrationConfigurationService(
		IIntegrationConfigurationRepository integrationConfigurationRepository,
		IDataProtectionProvider dataProtectionProvider,
		ILogger logger)
	{
		_integrationConfigurationRepository = integrationConfigurationRepository;
		_dataProtectionProvider = dataProtectionProvider;
		_logger = logger;
	}

	public async Task<byte[]?> GetDecryptedConfiguration(string integrationId, string key)
	{
		var configuration
			= await _integrationConfigurationRepository.GetConfigurationByIntegrationIdAndKey(integrationId, key);
		if (configuration is null)
		{
			return null;
		}

		try
		{
			return GetProtector(integrationId, key).Unprotect(configuration.EncryptedConfigurationValue);
		}
		catch (Exception e)
		{
			_logger.Error(e, "Failed to decrypt configuration");
			return null;
		}
	}

	public async Task SetDecryptedConfiguration(
		string integrationId,
		string key,
		byte[] value,
		bool canBeAccessFromOtherIntegrations = false)
	{
		var configuration
			= await _integrationConfigurationRepository.GetConfigurationByIntegrationIdAndKey(integrationId, key);
		if (configuration is not null)
		{
			configuration.EncryptedConfigurationValue = GetProtector(integrationId, key).Protect(value);
			await _integrationConfigurationRepository.Save();
			return;
		}

		configuration = new IntegrationConfigurationEntity
						{
							IntegrationId = integrationId,
							ConfigurationKey = key,
							EncryptedConfigurationValue = GetProtector(integrationId, key).Protect(value),
							CanBeAccessFromOtherIntegrations = canBeAccessFromOtherIntegrations
						};
		await _integrationConfigurationRepository.CreateConfiguration(configuration);
	}

	private IDataProtector GetProtector(string integrationId, string key)
	{
		return _dataProtectionProvider.CreateProtector($"integration_config:{integrationId}_key:{key}");
	}
}
