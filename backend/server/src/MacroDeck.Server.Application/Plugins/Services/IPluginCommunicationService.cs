using MacroDeck.SDK.PluginSDK.Messages;

namespace MacroDeck.Server.Application.Plugins.Services;

public interface IPluginCommunicationService
{
	Task InvokeAction(string connectionId, InvokeActionMessage message);
}