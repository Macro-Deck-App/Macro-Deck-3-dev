using System.Text.Json;
using MacroDeck.SDK.PluginSDK.Actions;
using MacroDeck.SDK.PluginSDK.Extensions;
using MacroDeck.SDK.PluginSDK.Integration.Music;
using MacroDeck.SDK.PluginSDK.MusicPlayers;
using MacroDeck.Server.Application.Integrations.Internal.Spotify.DataTypes;

namespace MacroDeck.Server.Application.Integrations.Internal.Spotify;

public class SpotifyIntegration : InternalExtension, IMusicPlayerIntegration
{
	public SpotifyIntegration(IServiceProvider services)
		: base(services)
	{
	}

	public override string Id => "MacroDeck.Spotify";

	public override string Name => "Spotify";

	public override string Version => "1.0.0";

	public override List<MacroDeckAction> GetActions()
	{
		return [];
	}

	public ValueTask<MusicPlayerData> GetMusicPlayerData()
	{
		return ValueTask.FromResult(new MusicPlayerData());
	}

	public override Task Start(CancellationToken cancellationToken = default)
	{
		Logger.Information("Spotify integration started");
		return Task.CompletedTask;
	}

	public override Task Stop(CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}

	protected async Task SaveConfig(SpotifyIntegrationConfig config)
	{
		var serializedConfig = JsonSerializer.SerializeToUtf8Bytes(config);
		await IntegrationConfigurationProvider.SetIntegrationConfiguration(Id, "config", serializedConfig);
	}

	protected async Task<SpotifyIntegrationConfig> GetConfig()
	{
		var config = await IntegrationConfigurationProvider.GetIntegrationConfiguration(Id, "config");
		if (config is null)
		{
			return new SpotifyIntegrationConfig
				   {
					   Enabled = false
				   };
		}

		var deserializedConfig = JsonSerializer.Deserialize<SpotifyIntegrationConfig>(config);
		if (deserializedConfig is not null)
		{
			return deserializedConfig;
		}

		Logger.Error("Could not deserialize Spotify integration config");
		return new SpotifyIntegrationConfig
			   {
				   Enabled = false
			   };
	}
}
