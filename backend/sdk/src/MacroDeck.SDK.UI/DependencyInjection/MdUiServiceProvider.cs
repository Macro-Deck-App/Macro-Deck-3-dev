using Microsoft.Extensions.DependencyInjection;

namespace MacroDeck.SDK.UI.DependencyInjection;

/// <summary>
///     Service provider for dependency injection in UI views
/// </summary>
public static class MdUiServiceProvider
{
	private static IServiceProvider? _serviceProvider;

    /// <summary>
    ///     Initializes the service provider
    /// </summary>
    public static void Initialize(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

    /// <summary>
    ///     Gets a required service from the DI container
    /// </summary>
    public static T GetRequiredService<T>()
		where T : notnull
	{
		if (_serviceProvider == null)
		{
			throw new InvalidOperationException("Service provider not initialized. Call Initialize first.");
		}

		return _serviceProvider.GetRequiredService<T>();
	}

    /// <summary>
    ///     Gets a service from the DI container
    /// </summary>
    public static T? GetService<T>()
		where T : class
	{
		if (_serviceProvider == null)
		{
			throw new InvalidOperationException("Service provider not initialized. Call Initialize first.");
		}

		return _serviceProvider.GetService<T>();
	}

    /// <summary>
    ///     Gets a service from the DI container by type
    /// </summary>
    public static object? GetService(Type serviceType)
	{
		if (_serviceProvider == null)
		{
			throw new InvalidOperationException("Service provider not initialized. Call Initialize first.");
		}

		return _serviceProvider.GetService(serviceType);
	}
}
