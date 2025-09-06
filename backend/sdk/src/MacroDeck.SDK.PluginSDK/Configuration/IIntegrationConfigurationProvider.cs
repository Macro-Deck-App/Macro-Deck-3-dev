namespace MacroDeck.SDK.PluginSDK.Configuration;

public interface IIntegrationConfigurationProvider
{
	Task<byte[]?> GetIntegrationConfiguration(string integrationId, string key);

	Task SetIntegrationConfiguration(string integrationId, string key, byte[] value);
}
