using System.Collections.Concurrent;
using System.Text.Json;
using Google.Protobuf;
using MacroDeck.Protobuf;
using MacroDeck.SDK.PluginSDK.Actions;
using MacroDeck.SDK.PluginSDK.Options;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;
using Serilog.Events;
using ProtobufLogLevel = MacroDeck.Protobuf.LogLevel;

namespace MacroDeck.SDK.PluginSDK.Clients;

public class MacroDeckPluginTransport : IDisposable
{
	private readonly ILogger _logger = Log.ForContext<MacroDeckPluginTransport>();
	private readonly ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _pendingRequests = new();
	private readonly PluginOptions _pluginOptions;
	private HubConnection? _connection;

	public MacroDeckPluginTransport(PluginOptions pluginOptions)
	{
		_pluginOptions = pluginOptions;
	}

	public bool IsConnected { get; private set; }

	public string? SessionId { get; private set; }

	public void Dispose()
	{
		IsConnected = false;
		_connection?.DisposeAsync();
	}

	public async Task<bool> Connect()
	{
		try
		{
			var protocol = _pluginOptions.UseSsl ? "https" : "http";
			var url = $"{protocol}://{_pluginOptions.Host}:{_pluginOptions.Port}/api/hubs/plugin-communication";

			_connection = new HubConnectionBuilder()
				.WithUrl(url)
				.WithAutomaticReconnect()
				.Build();

			_connection.On<byte[]>("ReceiveMessage", HandleIncomingMessage);
			_connection.Reconnected += OnReconnected;
			_connection.Closed += OnDisconnected;

			await _connection.StartAsync();

			var connectResult = await SendConnect();
			if (connectResult)
			{
				IsConnected = true;
				_logger.Information("Connected to MacroDeck server at {Url}", url);

				StartHeartbeat();
				return true;
			}

			_logger.Error("Failed to connect to MacroDeck server");
			return false;
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error connecting to MacroDeck server");
			return false;
		}
	}

	public async Task<bool> RegisterPlugin(IEnumerable<MacroDeckAction> actions)
	{
		if (!IsConnected || _connection == null)
		{
			_logger.Warning("Cannot register plugin - not connected");
			return false;
		}

		try
		{
			var actionDefinitions = actions.Select(a => new ActionDefinition
				{
					ActionId = a.Id,
					ActionName = a.Name,
					Description = a.Name // For now, using name as description
				})
				.ToList();

			var registerMessage = new RegisterPluginMessage();
			registerMessage.Actions.AddRange(actionDefinitions);

			var message = new PluginMessage
			{
				MessageId = Guid.NewGuid().ToString(),
				MessageType = MessageType.RegisterPlugin,
				PluginId = _pluginOptions.Id,
				Payload = ByteString.CopyFrom(registerMessage.ToByteArray())
			};

			var response = await SendMessageWithResponse(message);
			var registerResponse = RegisterPluginResponseMessage.Parser.ParseFrom(response.Payload);

			if (registerResponse.Success)
			{
				_logger.Information("Plugin registered successfully with {ActionCount} actions",
					actionDefinitions.Count);
				return true;
			}

			_logger.Warning("Plugin registration failed: {Message}", registerResponse.Message);
			return false;
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error registering plugin");
			return false;
		}
	}

	public async Task SendLogMessage(LogEventLevel level, string message, string category, Exception? exception = null)
	{
		if (!IsConnected || _connection == null)
		{
			return;
		}

		try
		{
			var protobufLogLevel = level switch
			{
				LogEventLevel.Verbose => ProtobufLogLevel.Trace,
				LogEventLevel.Debug => ProtobufLogLevel.Debug,
				LogEventLevel.Information => ProtobufLogLevel.Information,
				LogEventLevel.Warning => ProtobufLogLevel.Warning,
				LogEventLevel.Error => ProtobufLogLevel.Error,
				LogEventLevel.Fatal => ProtobufLogLevel.Critical,
				_ => ProtobufLogLevel.Information
			};

			var logMessage = new LogMessage
			{
				Level = protobufLogLevel,
				Message = message,
				Category = category,
				Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
				ExceptionJson = exception != null ? JsonSerializer.Serialize(exception) : ""
			};

			var pluginMessage = new PluginMessage
			{
				MessageId = Guid.NewGuid().ToString(),
				MessageType = MessageType.LogMessage,
				PluginId = _pluginOptions.Id,
				Payload = ByteString.CopyFrom(logMessage.ToByteArray())
			};

			await SendMessage(pluginMessage);
		}
		catch (Exception ex)
		{
			// Avoid infinite logging loop
			Console.WriteLine($"Failed to send log message: {ex.Message}");
		}
	}

	private async Task<bool> SendConnect()
	{
		var connectMessage = new ConnectMessage
		{
			PluginId = _pluginOptions.Id,
			PluginName = _pluginOptions.Name,
			PluginVersion = _pluginOptions.Version.ToString(),
			SdkVersion = "1.0.0" // TODO: Get from assembly
		};

		var message = new PluginMessage
		{
			MessageId = Guid.NewGuid().ToString(),
			MessageType = MessageType.Connect,
			PluginId = _pluginOptions.Id,
			Payload = ByteString.CopyFrom(connectMessage.ToByteArray())
		};

		try
		{
			var response = await SendMessageWithResponse(message);
			var connectResponse = ConnectResponseMessage.Parser.ParseFrom(response.Payload);

			if (connectResponse.Success)
			{
				SessionId = connectResponse.SessionId;
				return true;
			}

			_logger.Error("Connect failed: {Message}", connectResponse.Message);
			return false;
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error sending connect message");
			return false;
		}
	}

	private void StartHeartbeat()
	{
		_ = Task.Run(async () =>
		{
			while (IsConnected && _connection?.State == HubConnectionState.Connected)
			{
				try
				{
					var heartbeatMessage = new HeartbeatMessage
					{
						Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
					};

					var message = new PluginMessage
					{
						MessageId = Guid.NewGuid().ToString(),
						MessageType = MessageType.Heartbeat,
						PluginId = _pluginOptions.Id,
						Payload = ByteString.CopyFrom(heartbeatMessage.ToByteArray())
					};

					await SendMessage(message);
					await Task.Delay(30000); // Heartbeat every 30 seconds
				}
				catch (Exception ex)
				{
					_logger.Warning(ex, "Failed to send heartbeat");
					await Task.Delay(5000); // Retry after 5 seconds on error
				}
			}
		});
	}

	private async Task SendMessage(PluginMessage message)
	{
		if (_connection?.State == HubConnectionState.Connected)
		{
			await _connection.InvokeAsync("SendMessage", message.ToByteArray());
		}
	}

	private async Task<PluginMessage> SendMessageWithResponse(PluginMessage message)
	{
		var tcs = new TaskCompletionSource<byte[]>();
		_pendingRequests[message.MessageId] = tcs;

		try
		{
			await SendMessage(message);
			var responseBytes = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(30));
			return PluginMessage.Parser.ParseFrom(responseBytes);
		}
		finally
		{
			_pendingRequests.TryRemove(message.MessageId, out _);
		}
	}

	private void HandleIncomingMessage(byte[] messageBytes)
	{
		try
		{
			var message = PluginMessage.Parser.ParseFrom(messageBytes);

			// Check if this is a response to a pending request
			if (_pendingRequests.TryRemove(message.MessageId, out var tcs))
			{
				tcs.SetResult(messageBytes);
				return;
			}

			// Handle server-initiated messages (like action invocations)
			_ = Task.Run(async () => await ProcessServerMessage(message));
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error handling incoming message");
		}
	}

	private async Task ProcessServerMessage(PluginMessage message)
	{
		try
		{
			switch (message.MessageType)
			{
				case MessageType.InvokeAction:
					await HandleActionInvocation(message);
					break;
				default:
					_logger.Warning("Unhandled server message type: {MessageType}", message.MessageType);
					break;
			}
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error processing server message {MessageType}", message.MessageType);
		}
	}

	private async Task HandleActionInvocation(PluginMessage message)
	{
		// This will be implemented to work with the action handler system
		_logger.Debug("Received action invocation: {MessageId}", message.MessageId);

		// TODO: Integrate with action handler to actually invoke actions
		var response = new InvokeActionResponseMessage
		{
			Success = true,
			Message = "Action invoked successfully"
		};

		var responseMessage = new PluginMessage
		{
			MessageId = message.MessageId,
			MessageType = MessageType.InvokeActionResponse,
			PluginId = _pluginOptions.Id,
			Payload = ByteString.CopyFrom(response.ToByteArray())
		};

		await SendMessage(responseMessage);
	}

	private async Task OnReconnected(string? connectionId)
	{
		_logger.Information("Reconnected to MacroDeck server");
		IsConnected = true;
		await SendConnect(); // Re-authenticate
	}

	private Task OnDisconnected(Exception? exception)
	{
		IsConnected = false;
		_logger.Warning(exception, "Disconnected from MacroDeck server");
		return Task.CompletedTask;
	}
}
