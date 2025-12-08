using System.Collections.Concurrent;
using MacroDeck.SDK.UI.Messages;
using MacroDeck.SDK.UI.Services;
using MacroDeck.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using ILogger = Serilog.ILogger;

namespace MacroDeck.Server.Services;

/// <summary>
///     Service for requesting user approval to open links
/// </summary>
public class LinkRequestService : IMdUiLinkService
{
	private static readonly ILogger Log = Serilog.Log.ForContext<LinkRequestService>();
	private readonly IHubContext<MdUiHub, IMdUiClient> _hubContext;
	private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _pendingRequests = new();

	public LinkRequestService(IHubContext<MdUiHub, IMdUiClient> hubContext)
	{
		_hubContext = hubContext;
	}

	public async Task<bool> RequestLinkOpen(string sessionId, string url, int timeoutMs = 300000)
	{
		var requestId = Guid.NewGuid().ToString();
		var tcs = new TaskCompletionSource<bool>();

		_pendingRequests[requestId] = tcs;

		try
		{
			var message = new LinkRequestMessage
			{
				RequestId = requestId,
				SessionId = sessionId,
				Url = url
			};

			Log.Information("Sending link request {RequestId} for URL {Url} to session {SessionId}",
				requestId, url, sessionId);

			await _hubContext.Clients.Group(sessionId).LinkRequest(message);

			using var cts = new CancellationTokenSource(timeoutMs);
			var timeoutTask = Task.Delay(timeoutMs, cts.Token);
			var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

			if (completedTask == timeoutTask)
			{
				Log.Warning("Link request {RequestId} timed out after {TimeoutMs}ms", requestId, timeoutMs);
				return false;
			}

			cts.Cancel();
			var result = await tcs.Task;
			Log.Information("Link request {RequestId} completed with result: {Result}", requestId, result);
			return result;
		}
		finally
		{
			_pendingRequests.TryRemove(requestId, out _);
		}
	}

	/// <summary>
	///     Called when frontend responds to a link request
	/// </summary>
	public void HandleLinkResponse(LinkResponseMessage response)
	{
		if (_pendingRequests.TryGetValue(response.RequestId, out var tcs))
		{
			Log.Debug("Received link response for {RequestId}: {Approved}", response.RequestId, response.Approved);
			tcs.SetResult(response.Approved);
		}
		else
		{
			Log.Warning("Received link response for unknown request {RequestId}", response.RequestId);
		}
	}
}
