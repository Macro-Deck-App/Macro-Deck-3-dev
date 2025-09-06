namespace MacroDeck.Server.Application.Services;

public interface IIntegrationConfigurationService
{
	Task<byte[]?> GetDecryptedConfiguration(string integrationId, string key);

	Task SetDecryptedConfiguration(
		string integrationId,
		string key,
		byte[] value,
		bool canBeAccessFromOtherIntegrations = false);
}
