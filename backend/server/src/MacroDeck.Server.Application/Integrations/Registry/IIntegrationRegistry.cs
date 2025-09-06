using MacroDeck.SDK.PluginSDK.Integration;
using MacroDeck.Server.Domain.Enums;

namespace MacroDeck.Server.Application.Integrations.Registry;

public interface IIntegrationRegistry
{
	void Register(IntegrationType type, MacroDeckIntegration integration);

	Task EnableInternalIntegrations(CancellationToken cancellationToken = default);
}
