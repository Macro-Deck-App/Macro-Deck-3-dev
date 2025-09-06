namespace MacroDeck.SDK.PluginSDK.Messages;

public class ConnectMessage
{
	public required string ExtensionId { get; set; }
	public required string ExtensionName { get; set; }
	public required string ExtensionVersion { get; set; }
	public required string SdkVersion { get; set; }
}

public class ConnectResponseMessage
{
	public bool Success { get; set; }
	public string Message { get; set; } = string.Empty;
	public string SessionId { get; set; } = string.Empty;
}