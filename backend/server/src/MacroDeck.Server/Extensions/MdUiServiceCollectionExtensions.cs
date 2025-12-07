using MacroDeck.SDK.UI.DependencyInjection;
using MacroDeck.SDK.UI.Registry;
using MacroDeck.Server.Application.UI.Registry;
using MacroDeck.Server.Application.UI.Services;
using MacroDeck.Server.Services;

namespace MacroDeck.Server.Extensions;

/// <summary>
///     Extension methods for registering UI services
/// </summary>
public static class MdUiServiceCollectionExtensions
{
	/// <summary>
	///     Adds MacroDeck UI framework services
	/// </summary>
	public static IServiceCollection AddMacroDeckUi(this IServiceCollection services)
	{
		// Register core services
		services.AddSingleton<MdUiRegistry>();
		services.AddSingleton<IMdUiRegistry>(sp => sp.GetRequiredService<MdUiRegistry>());
		services.AddSingleton<MdUiStateManager>();
		services.AddSingleton<MdAssetService>();
		services.AddSingleton<IMdUiUpdateService, SignalRMdUiUpdateService>();

		// Register view registration service
		services.AddHostedService<UiViewRegistrationService>();

		// Initialize service provider for views
		var serviceProvider = services.BuildServiceProvider();
		MdUiServiceProvider.Initialize(serviceProvider);

		return services;
	}
}
