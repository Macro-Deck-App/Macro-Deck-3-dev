using MacroDeck.SDK.PluginSDK.Configuration;
using MacroDeck.SDK.PluginSDK.Integration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MacroDeck.Server.Application.Integrations.Internal;

public abstract class InternalIntegration : MacroDeckIntegration
{
	protected InternalIntegration(IServiceProvider services)
	{
		Services = services;
		Logger = Services.GetRequiredService<ILogger>();
		IntegrationConfigurationProvider = Services.GetRequiredService<IIntegrationConfigurationProvider>();
	}

	public IServiceProvider Services { get; }

	public ILogger Logger { get; }

	public IIntegrationConfigurationProvider IntegrationConfigurationProvider { get; }

	public abstract string Name { get; }

	public abstract string IntegrationId { get; }
}
