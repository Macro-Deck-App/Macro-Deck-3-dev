using MacroDeck.SDK.PluginSDK.Extensions;

namespace MacroDeck.Server.Application.Integrations.Internal;

/// <summary>
/// Obsolete: Use InternalExtension from MacroDeck.SDK.PluginSDK.Extensions instead
/// </summary>
[Obsolete("Use InternalExtension from MacroDeck.SDK.PluginSDK.Extensions instead")]
public abstract class InternalIntegration : InternalExtension
{
	protected InternalIntegration(IServiceProvider services) : base(services)
	{
	}

	public abstract string IntegrationId { get; }

	public override string Id => IntegrationId;
}
