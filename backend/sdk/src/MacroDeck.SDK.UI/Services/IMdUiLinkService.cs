namespace MacroDeck.SDK.UI.Services;

/// <summary>
/// Service for requesting user approval to open links
/// </summary>
public interface IMdUiLinkService
{
	/// <summary>
	/// Requests user approval to open a link
	/// </summary>
	/// <param name="sessionId">The session ID</param>
	/// <param name="url">The URL to open</param>
	/// <param name="timeoutMs">Timeout in milliseconds (default 5 minutes)</param>
	/// <returns>True if approved, false if denied or timeout</returns>
	Task<bool> RequestLinkOpen(string sessionId, string url, int timeoutMs = 300000);
}
