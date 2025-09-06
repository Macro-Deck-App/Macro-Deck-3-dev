using MacroDeck.Protobuf;

namespace MacroDeck.Server.Application.Plugins.Models;

public class PluginInfo
{
	public required string PluginId { get; init; }
	public required string Name { get; init; }
	public required string Version { get; init; }
	public required string SdkVersion { get; init; }
	public required string SessionId { get; init; }
	public required string ConnectionId { get; init; }
	public DateTime ConnectedAt { get; init; }
	public DateTime? LastHeartbeat { get; set; }
	public List<ActionDefinition> Actions { get; init; } = [];
	public byte[]? SessionKey { get; set; }
}