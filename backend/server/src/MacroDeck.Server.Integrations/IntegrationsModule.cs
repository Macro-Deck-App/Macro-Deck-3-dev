using MacroDeck.SDK.PluginSDK.Extensions;
using MacroDeck.Server.Application.Integrations.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace MacroDeck.Server.Integrations;

public class IntegrationsModule
{
	public static async Task StartInternalIntegrations(
		IServiceProvider serviceProvider,
		CancellationToken cancellationToken = default)
	{
		var internalExtensionTypes = FindInternalIntegrations();
		var integrationRegistry = serviceProvider.GetRequiredService<IIntegrationRegistry>();
		foreach (var type in internalExtensionTypes)
		{
			if (Activator.CreateInstance(type, serviceProvider) is IMacroDeckExtension instance)
			{
				integrationRegistry.Register(instance);
			}
		}

		await integrationRegistry.EnableInternalExtensions(cancellationToken);
	}

	public static IEnumerable<Type> FindInternalIntegrations()
	{
		var types = typeof(IntegrationsModule).Assembly.GetTypes()
			.Where(type => type is { IsInterface: false, IsAbstract: false });
		return types.Where(x
			=> x.IsAssignableTo(typeof(IMacroDeckExtension)) && x.IsAssignableTo(typeof(InternalExtension)));
	}
}
