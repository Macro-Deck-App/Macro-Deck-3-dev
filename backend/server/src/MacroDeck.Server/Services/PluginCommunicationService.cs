using MacroDeck.Server.Application.Plugins.Services;
using MacroDeck.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MacroDeck.Server.Services;

public class PluginCommunicationService : IPluginCommunicationService
{
	private readonly IHubContext<PluginCommunicationHub> _hubContext;

	public PluginCommunicationService(IHubContext<PluginCommunicationHub> hubContext)
	{
		_hubContext = hubContext;
	}

	public async Task SendToPlugin(string connectionId, byte[] messageBytes)
	{
		await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", messageBytes);
	}
}