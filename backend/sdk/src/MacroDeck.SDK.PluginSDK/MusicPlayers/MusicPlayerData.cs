namespace MacroDeck.SDK.PluginSDK.MusicPlayers;

public record MusicPlayerData
{
	public bool Playing { get; set; }

	public byte[]? AlbumArtBytes { get; set; }

	public int? PlaybackPercentage { get; set; }

	public long? CurrentPlaybackDuration { get; set; }

	public long? TotalPlaybackDuration { get; set; }

	public string? Title { get; set; }

	public string? Artists { get; set; }

	public string? Album { get; set; }
}
