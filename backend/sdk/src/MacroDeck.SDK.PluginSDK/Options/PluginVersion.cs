namespace MacroDeck.SDK.PluginSDK.Options;

public struct PluginVersion
{
	/// <summary>
	///     The plugin's major version. May introduce breaking changes.
	/// </summary>
	public int Major { get; set; }

	/// <summary>
	///     The plugin's minor version. May introduce new features in a backwards-compatible manner.
	/// </summary>
	public int Minor { get; set; }

	/// <summary>
	///     The plugin's patch version. May introduce backwards-compatible bug fixes.
	/// </summary>
	public int Patch { get; set; }

	/// <summary>
	///     Can be used to indicate pre-release versions
	/// </summary>
	public int? PreReleaseRevision { get; set; }

	/// <summary>
	///     Initializes a new instance of the <see cref="PluginVersion" /> struct.
	/// </summary>
	public PluginVersion(int major, int minor, int patch)
	{
		Major = major;
		Minor = minor;
		Patch = patch;
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="PluginVersion" /> struct with pre-release version.
	/// </summary>
	public PluginVersion(int major, int minor, int patch, int? preReleaseRevision)
	{
		Major = major;
		Minor = minor;
		Patch = patch;
		PreReleaseRevision = preReleaseRevision;
	}

	public override string ToString()
	{
		return PreReleaseRevision.HasValue
			? $"{Major}.{Minor}.{Patch}-pre{PreReleaseRevision.Value}"
			: $"{Major}.{Minor}.{Patch}";

	}
}
