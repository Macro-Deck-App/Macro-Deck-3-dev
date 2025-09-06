using MacroDeck.SDK.PluginSDK.Extensions;
using MacroDeck.Server.Application.Integrations.Internal;

namespace MacroDeck.Server.Application.Utils;

public static class IntegrationUtils
{
	public static IEnumerable<Type> FindInternalExtensions()
	{
		var types = typeof(ApplicationModule).Assembly.GetTypes()
											 .Where(type => type is { IsInterface: false, IsAbstract: false });
		return types.Where(x => x.IsAssignableTo(typeof(IMacroDeckExtension)) && 
		                       (x.IsAssignableTo(typeof(InternalExtension)) || x.IsAssignableTo(typeof(InternalIntegration))));
	}

	[Obsolete("Use FindInternalExtensions instead")]
	public static IEnumerable<Type> FindInternalIntegrations()
	{
		return FindInternalExtensions();
	}
}
