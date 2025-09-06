namespace MacroDeck.SDK.PluginSDK.Messages;

public class InvokeActionMessage
{
	public required string ActionId { get; set; }
	public object? Parameters { get; set; }
}

public class InvokeActionResponseMessage
{
	public bool Success { get; set; }
	public string Message { get; set; } = string.Empty;
	public object? Result { get; set; }
}
