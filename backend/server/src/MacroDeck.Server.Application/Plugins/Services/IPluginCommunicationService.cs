namespace MacroDeck.Server.Application.Plugins.Services;

public interface IPluginCommunicationService
{
	Task SendToPlugin(string connectionId, byte[] messageBytes);
}