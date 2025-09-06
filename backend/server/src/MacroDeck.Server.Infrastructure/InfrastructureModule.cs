using MacroDeck.Server.Application.Integrations.Registry;
using MacroDeck.Server.Application.Repositories;
using MacroDeck.Server.Infrastructure.Integrations.Registry;
using MacroDeck.Server.Infrastructure.Persistence;
using MacroDeck.Server.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MacroDeck.Server.Infrastructure;

public static class InfrastructureModule
{
	public static IServiceCollection AddInfrastructureModule(this IServiceCollection services)
	{
		services.AddScoped<IFolderRepository, FolderRepository>();
		services.AddScoped<IWidgetRepository, WidgetRepository>();
		services.AddScoped<IIntegrationConfigurationRepository, IntegrationConfigurationRepository>();

		services.AddSingleton<IIntegrationRegistry, IntegrationRegistry>();

		services.AddDbContext<DatabaseContext>();

		return services;
	}
}
