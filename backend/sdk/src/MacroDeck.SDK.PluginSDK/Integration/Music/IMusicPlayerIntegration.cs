using MacroDeck.SDK.PluginSDK.MusicPlayers;

namespace MacroDeck.SDK.PluginSDK.Integration.Music;

public interface IMusicPlayerIntegration
{
	ValueTask<MusicPlayerData> GetMusicPlayerData();
}
