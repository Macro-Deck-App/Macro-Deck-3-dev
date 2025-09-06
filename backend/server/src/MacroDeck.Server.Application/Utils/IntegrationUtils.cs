using MacroDeck.Server.Application.Integrations.Internal;

namespace MacroDeck.Server.Application.Utils;

public static class IntegrationUtils
{
	public static IEnumerable<Type> FindInternalIntegrations()
	{
		var types = typeof(ApplicationModule).Assembly.GetTypes()
											 .Where(type => type is { IsInterface: false, IsAbstract: false });
		return types.Where(x => x.IsAssignableTo(typeof(InternalIntegration)));
	}
}
