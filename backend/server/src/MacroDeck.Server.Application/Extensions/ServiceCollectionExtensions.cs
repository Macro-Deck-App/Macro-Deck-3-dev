using System.Reflection;
using MacroDeck.Server.Application.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace MacroDeck.Server.Application.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddNotificationHandlersFromAllAssemblies(this IServiceCollection services)
	{
		var assemblies = AppDomain.CurrentDomain.GetAssemblies();

		foreach (var assembly in assemblies)
		{
			services.AddNotificationHandlers(assembly);
		}

		return services;
	}

	private static IServiceCollection AddNotificationHandlers(this IServiceCollection services, Assembly assembly)
	{
		var handlerInterfaceType = typeof(INotificationHandler<>);

		var types = assembly
					.GetTypes()
					.Where(t => t is { IsAbstract: false, IsInterface: false })
					.SelectMany(t => t.GetInterfaces()
									  .Where(i => i.IsGenericType
												  && i.GetGenericTypeDefinition() == handlerInterfaceType)
									  .Select(i => new { HandlerType = i, ImplementationType = t }))
					.ToList();

		foreach (var type in types)
		{
			services.AddTransient(type.HandlerType, type.ImplementationType);
		}

		return services;
	}
}
