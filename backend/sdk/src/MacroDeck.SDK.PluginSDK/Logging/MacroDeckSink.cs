using MacroDeck.SDK.PluginSDK.Clients;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Serilog.Events;

namespace MacroDeck.SDK.PluginSDK.Logging;

public class MacroDeckSink : ILogEventSink
{
	private readonly IServiceProvider _serviceProvider;

	public MacroDeckSink(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public void Emit(LogEvent logEvent)
	{
		try
		{
			var transport = _serviceProvider.GetService<MacroDeckPluginTransport>();
			if (transport?.IsConnected == true)
			{
				var message = logEvent.RenderMessage();
				var category = logEvent.Properties.TryGetValue("SourceContext", out var sourceContext)
					? sourceContext.ToString().Trim('"')
					: "Plugin";

				var exception = logEvent.Exception;

				_ = Task.Run(async () => await transport.SendLogMessage(logEvent.Level, message, category, exception));
			}
		}
		catch
		{
			// Ignore errors to avoid logging loops
		}
	}
}
