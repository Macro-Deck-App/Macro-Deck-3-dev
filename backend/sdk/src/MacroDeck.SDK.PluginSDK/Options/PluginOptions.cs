namespace MacroDeck.SDK.PluginSDK.Options;

public class PluginOptions
{
	/// <summary>
	///     The unique identifier for the plugin, usually in reverse domain name notation (e.g., "com.example.plugin").
	///     Make sure this ID is unique to avoid conflicts with other plugins.
	///     Only use lowercase letters, numbers, and dots. Avoid special characters and spaces.
	/// </summary>
	public required string PluginId { get; set; }

	/// <summary>
	///     The display name of the plugin as it will appear in Macro Deck. Should be user-friendly and descriptive.
	///     Avoid using special characters that might not be supported.
	/// </summary>
	public required string PluginName { get; set; }

	/// <summary>
	///     The website or URL where users can find more information about the plugin.
	///     Can be GitHub repository, documentation site, etc.
	/// </summary>
	public string? PluginWebsite { get; set; }

	/// <summary>
	///     Your name or the name of the organization that developed the plugin.
	/// </summary>
	public required string Author { get; set; }

	/// <summary>
	///     Website or URL where users can find more information about the author or organization.
	/// </summary>
	public string? AuthorWebsite { get; set; }

	/// <summary>
	///     Discord user ID of the author or organization. This can be used for support or contact purposes.
	/// </summary>
	public long? AuthorDiscordUserId { get; set; }

	/// <summary>
	///     The version of the plugin, following semantic versioning (e.g., "1.0.0").
	///     Supports pre-release versions (e.g., "1.0.0-pre1").
	/// </summary>
	public required PluginVersion Version { get; set; }

	/// <summary>
	///     The host to connect to. Set via command line argument --host.
	/// </summary>
	public string Host { get; set; } = "127.0.0.1";

	/// <summary>
	///     The port to connect to. Set via command line argument --port.
	/// </summary>
	public int Port { get; set; }

	/// <summary>
	///     Whether to use SSL/TLS for the connection. Set via command line argument --ssl.
	/// </summary>
	public bool UseSsl { get; set; }

	// Convenience properties for backward compatibility
	public string Id => PluginId;
	public string Name => PluginName;
}
