using MacroDeck.SDK.PluginSDK.Logging;
using Serilog;
using Serilog.Configuration;

namespace MacroDeck.SDK.PluginSDK.Extensions;

public static class LoggerConfigurationExtensions
{
	public static LoggerConfiguration MacroDeck(
		this LoggerSinkConfiguration sinkConfiguration,
		IServiceProvider serviceProvider)
	{
		return sinkConfiguration.Sink(new MacroDeckSink(serviceProvider));
	}
}
