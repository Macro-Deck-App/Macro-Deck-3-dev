using MacroDeck.SDK.PluginSDK.Configuration.Enums;
using MacroDeck.SDK.PluginSDK.Configuration.Forms;

namespace MacroDeck.SDK.PluginSDK.Integration;

public abstract class MacroDeckIntegration
{
	public virtual ConfigurationType? ConfigurationType => null;

	public abstract Task Start(CancellationToken cancellation);

	public abstract Task Stop(CancellationToken cancellation);

	public virtual Task<SimpleForm>? GetIntegrationConfigurationForm()
	{
		return null;
	}
}
