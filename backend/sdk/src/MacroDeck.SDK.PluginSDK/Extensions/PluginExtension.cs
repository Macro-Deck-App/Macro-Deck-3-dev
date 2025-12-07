using MacroDeck.SDK.PluginSDK.Actions;
using MacroDeck.SDK.PluginSDK.Options;

namespace MacroDeck.SDK.PluginSDK.Extensions;

/// <summary>
///     Base class for external MacroDeck plugins
/// </summary>
public abstract class PluginExtension : IMacroDeckExtension
{
	protected PluginExtension(PluginOptions pluginOptions)
	{
		PluginOptions = pluginOptions;
	}

	public PluginOptions PluginOptions { get; }

	public string Id => PluginOptions.Id;

	public string Name => PluginOptions.Name;

	public string Version => PluginOptions.Version.ToString();

	public ExtensionType ExtensionType => ExtensionType.Plugin;

	public Type? IntegrationConfigurationView { get; }

	public abstract List<MacroDeckAction> GetActions();

	public abstract Task Start(CancellationToken cancellationToken = default);

	public abstract Task Stop(CancellationToken cancellationToken = default);
}
