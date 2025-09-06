namespace MacroDeck.SDK.PluginSDK.Messages;

public class ActionDefinition
{
	public required string ActionId { get; set; }
	public required string ActionName { get; set; }
}

public class RegisterExtensionMessage
{
	public List<ActionDefinition> Actions { get; set; } = new();
}

public class RegisterExtensionResponseMessage
{
	public bool Success { get; set; }
	public string Message { get; set; } = string.Empty;
}
