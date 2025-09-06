using MacroDeck.Server.Domain.Entities;

namespace MacroDeck.Server.Application.Repositories;

public interface IIntegrationConfigurationRepository
{
	Task<IntegrationConfigurationEntity?> GetConfigurationByIntegrationIdAndKey(string integrationId, string key);

	Task CreateConfiguration(IntegrationConfigurationEntity integrationConfiguration);

	Task Save();
}
