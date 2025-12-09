using System.Collections.Concurrent;
using MacroDeck.SDK.UI.Messages;
using MacroDeck.SDK.UI.Services;
using MacroDeck.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using ILogger = Serilog.ILogger;
using Log = Serilog.Log;

namespace MacroDeck.Server.Services;

public class LinkRequestService : IMdUiLinkService
{
	private static readonly ILogger Logger = Log.ForContext<LinkRequestService>();
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
			Logger.Information("Sending link request {RequestId} for {Url} to session {SessionId}",
				requestId,
				url,
				sessionId);

			await _hubContext.Clients.Group(sessionId)
				.LinkRequest(new LinkRequestMessage
				{
					RequestId = requestId,
					SessionId = sessionId,
					Url = url
				});

			using var cts = new CancellationTokenSource(timeoutMs);
			var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs, cts.Token));

			if (completed != tcs.Task)
			{
				Logger.Warning("Link request {RequestId} timed out", requestId);
				return false;
			}

			await cts.CancelAsync();
			return await tcs.Task;
		}
		finally
		{
			_pendingRequests.TryRemove(requestId, out _);
		}
	}

	public void HandleLinkResponse(LinkResponseMessage response)
	{
		if (_pendingRequests.TryRemove(response.RequestId, out var tcs))
		{
			Logger.Debug("Link response {RequestId}: {Approved}", response.RequestId, response.Approved);
			tcs.SetResult(response.Approved);
		}
		else
		{
			Logger.Warning("Unknown link response {RequestId}", response.RequestId);
		}
	}
}
