using MacroDeck.Server.Domain.Entities.Base;

namespace MacroDeck.Server.Domain.Entities;

public class IntegrationConfigurationEntity : BaseCreatedUpdatedEntity
{
	public required string IntegrationId { get; set; }

	public required string ConfigurationKey { get; set; }

	public required byte[] EncryptedConfigurationValue { get; set; }

	public bool CanBeAccessFromOtherIntegrations { get; set; }
}
