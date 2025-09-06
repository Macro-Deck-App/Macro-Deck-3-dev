using MacroDeck.SDK.PluginSDK.Options;
using Microsoft.Extensions.DependencyInjection;

namespace MacroDeck.SDK.PluginSDK;

public class PluginBuilder
{
	private readonly ActionCollection _actions = new();
	private readonly PluginOptions _pluginOptions;
	private readonly IServiceCollection _services = new ServiceCollection();

	public PluginBuilder(PluginOptions pluginOptions)
	{
		_pluginOptions = pluginOptions;
	}

	public PluginBuilder ConfigureServices(Action<IServiceCollection> services)
	{
		services(_services);
		return this;
	}

	public PluginBuilder RegisterActions(Action<ActionCollection> actions)
	{
		actions(_actions);
		return this;
	}

	public Plugin Build()
	{
		return new Plugin(_pluginOptions, _actions, _services);
	}
}
