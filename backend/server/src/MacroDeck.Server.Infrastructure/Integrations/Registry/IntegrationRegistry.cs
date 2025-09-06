using System.Collections.Concurrent;
using MacroDeck.SDK.PluginSDK.Extensions;
using MacroDeck.SDK.PluginSDK.Integration;
using MacroDeck.Server.Application.Integrations.Registry;
using MacroDeck.Server.Domain.Enums;
using Serilog;

namespace MacroDeck.Server.Infrastructure.Integrations.Registry;

public class IntegrationRegistry : IIntegrationRegistry
{
	private readonly ConcurrentDictionary<string, IMacroDeckExtension> _extensions = new();
	private readonly ILogger _logger;

	public IntegrationRegistry(ILogger logger)
	{
		_logger = logger;
	}

	public void Register(IMacroDeckExtension extension)
	{
		_extensions.TryAdd(extension.Id, extension);
		_logger.Information("Registered {ExtensionType} extension: {Name} ({Id})", 
			extension.ExtensionType, extension.Name, extension.Id);
	}

	public async Task EnableInternalExtensions(CancellationToken cancellationToken = default)
	{
		var internalExtensions = _extensions.Values
			.Where(x => x.ExtensionType == ExtensionType.Internal)
			.ToList();

		var tasks = internalExtensions.Select(x => x.Start(cancellationToken));
		await Task.WhenAll(tasks);

		_logger.Information("Enabled {Count} internal extensions", internalExtensions.Count);
	}

	public IMacroDeckExtension? GetExtension(string extensionId)
	{
		_extensions.TryGetValue(extensionId, out var extension);
		return extension;
	}

	public IEnumerable<IMacroDeckExtension> GetExtensions(ExtensionType? type = null)
	{
		if (type.HasValue)
		{
			return _extensions.Values.Where(x => x.ExtensionType == type.Value);
		}
		return _extensions.Values;
	}

	// Legacy method for backward compatibility
	[Obsolete("Use Register(IMacroDeckExtension) instead")]
	public void Register(IntegrationType type, MacroDeckIntegration integration)
	{
		if (integration is IMacroDeckExtension extension)
		{
			Register(extension);
		}
	}

	// Legacy method for backward compatibility
	[Obsolete("Use EnableInternalExtensions instead")]
	public async Task EnableInternalIntegrations(CancellationToken cancellationToken = default)
	{
		await EnableInternalExtensions(cancellationToken);
	}
}
