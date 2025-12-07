using MacroDeck.SDK.PluginSDK.Actions;
using MacroDeck.SDK.PluginSDK.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MacroDeck.SDK.PluginSDK.Extensions;

/// <summary>
///     Base class for internal MacroDeck extensions (integrations)
/// </summary>
public abstract class InternalExtension : IMacroDeckExtension
{
	protected InternalExtension(IServiceProvider services)
	{
		Services = services;
		Logger = Services.GetRequiredService<ILogger>();
		IntegrationConfigurationProvider = Services.GetRequiredService<IIntegrationConfigurationProvider>();
	}

	public IServiceProvider Services { get; }

	public ILogger Logger { get; }

	public IIntegrationConfigurationProvider IntegrationConfigurationProvider { get; }

	public abstract string Id { get; }

	public abstract string Name { get; }

	public abstract string Version { get; }

	public ExtensionType ExtensionType => ExtensionType.Internal;

	public virtual Type? IntegrationConfigurationView { get; }

	public virtual List<MacroDeckAction> GetActions()
	{
		return new List<MacroDeckAction>();
	}

	public abstract Task Start(CancellationToken cancellationToken = default);

	public abstract Task Stop(CancellationToken cancellationToken = default);
}
