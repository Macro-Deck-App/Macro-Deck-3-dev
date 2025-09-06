using MacroDeck.Server.Application.HostedServices;
using MacroDeck.Server.Application.Plugins.Services;
using MacroDeck.Server.Application.Services;
using MacroDeck.Server.Application.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace MacroDeck.Server.Application;

public static class ApplicationModule
{
	public static IServiceCollection AddApplicationModule(this IServiceCollection services)
	{
		services.AddScoped<IFolderService, FolderService>();
		services.AddScoped<IWidgetService, WidgetService>();
		services.AddScoped<IIntegrationConfigurationService, IntegrationConfigurationService>();

		services.AddSingleton<IPublisherService, PublisherService>();

		services.AddSingleton<IPluginRegistry, PluginRegistry>();
		services.AddScoped<IPluginActionInvoker, PluginActionInvoker>();

		services.AddHostedService<CreateDefaultFolderHostedService>();
		services.AddHostedService<LoadInternalIntegrationsHostedService>();

		return services;
	}
}
