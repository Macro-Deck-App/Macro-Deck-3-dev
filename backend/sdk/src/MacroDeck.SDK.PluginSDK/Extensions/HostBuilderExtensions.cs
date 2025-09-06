using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace MacroDeck.SDK.PluginSDK.Extensions;

public static class HostBuilderExtensions
{
	public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder)
	{
		return hostBuilder.UseSerilog((_, _, configuration) =>
			configuration
				.MinimumLevel.Verbose()
				.MinimumLevel.Override("Microsoft",
					LogEventLevel.Information)
				.MinimumLevel.Override("System.Net.Http.HttpClient",
					Debugger.IsAttached
						? LogEventLevel.Debug
						: LogEventLevel.Information)
#if !DEBUG
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
#endif
				.WriteTo.Console(theme: AnsiConsoleTheme.Code));
	}
}
