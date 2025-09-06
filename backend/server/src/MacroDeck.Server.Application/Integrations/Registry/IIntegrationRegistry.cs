using MacroDeck.SDK.PluginSDK.Extensions;
using MacroDeck.SDK.PluginSDK.Integration;
using MacroDeck.Server.Domain.Enums;

namespace MacroDeck.Server.Application.Integrations.Registry;

public interface IIntegrationRegistry
{
	/// <summary>
	/// Register a MacroDeck extension (internal integration or external plugin)
	/// </summary>
	void Register(IMacroDeckExtension extension);

	/// <summary>
	/// Enable all internal extensions
	/// </summary>
	Task EnableInternalExtensions(CancellationToken cancellationToken = default);

	/// <summary>
	/// Get an extension by its ID
	/// </summary>
	IMacroDeckExtension? GetExtension(string extensionId);

	/// <summary>
	/// Get all extensions, optionally filtered by type
	/// </summary>
	IEnumerable<IMacroDeckExtension> GetExtensions(ExtensionType? type = null);

	// Legacy methods for backward compatibility
	[Obsolete("Use Register(IMacroDeckExtension) instead")]
	void Register(IntegrationType type, MacroDeckIntegration integration);

	[Obsolete("Use EnableInternalExtensions instead")]
	Task EnableInternalIntegrations(CancellationToken cancellationToken = default);
}
