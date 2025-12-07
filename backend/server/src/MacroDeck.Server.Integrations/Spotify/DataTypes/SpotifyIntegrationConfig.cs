namespace MacroDeck.Server.Integrations.Spotify.DataTypes;

public class SpotifyIntegrationConfig
{
	public required bool Enabled { get; set; }

	public string? ClientId { get; set; }

	public string? AccessToken { set; get; }

	public string? RefreshToken { set; get; }
}
