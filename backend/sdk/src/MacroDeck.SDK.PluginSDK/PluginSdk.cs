using MacroDeck.SDK.PluginSDK.Options;

namespace MacroDeck.SDK.PluginSDK;

public class PluginSdk
{
	public static PluginBuilder CreatePluginBuilder(PluginOptions pluginOptions)
	{
		return new PluginBuilder(pluginOptions);
	}
}
