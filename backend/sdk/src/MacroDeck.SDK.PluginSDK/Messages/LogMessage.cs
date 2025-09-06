using Serilog.Events;

namespace MacroDeck.SDK.PluginSDK.Messages;

public class LogMessage
{
	public LogEventLevel Level { get; set; }
	public required string Message { get; set; }
	public string Category { get; set; } = string.Empty;
	public long Timestamp { get; set; }
	public string? ExceptionJson { get; set; }
}