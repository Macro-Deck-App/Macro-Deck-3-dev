namespace MacroDeck.Server.Application.Plugins.Services;

public interface IPluginActionInvoker
{
	Task<bool> InvokeAction(string pluginId, string actionId, string? configurationJson = null);
}