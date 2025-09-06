using System.Security.Cryptography;
using Google.Protobuf;
using MacroDeck.Protobuf;
using MacroDeck.Server.Application.Plugins.Services;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using ILogger = Serilog.ILogger;
using ProtobufLogLevel = MacroDeck.Protobuf.LogLevel;

namespace MacroDeck.Server.Hubs;

public class PluginCommunicationHub : Hub
{
	private readonly IPluginEncryptionService _encryptionService;
	private readonly ILogger _logger = Log.ForContext<PluginCommunicationHub>();
	private readonly IPluginRegistry _pluginRegistry;

	public PluginCommunicationHub(
		IPluginRegistry pluginRegistry,
		IPluginEncryptionService encryptionService)
	{
		_pluginRegistry = pluginRegistry;
		_encryptionService = encryptionService;
	}

	public override async Task OnConnectedAsync()
	{
		_logger.Debug("New connection established: {ConnectionId}", Context.ConnectionId);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		await _pluginRegistry.UnregisterPlugin(Context.ConnectionId);

		if (exception != null)
		{
			_logger.Warning(exception,
				"Plugin connection {ConnectionId} disconnected with exception",
				Context.ConnectionId);
		}
		else
		{
			_logger.Debug("Plugin connection {ConnectionId} disconnected", Context.ConnectionId);
		}

		await base.OnDisconnectedAsync(exception);
	}

	public async Task<byte[]> SendMessage(byte[] messageBytes)
	{
		try
		{
			var message = PluginMessage.Parser.ParseFrom(messageBytes);
			_logger.Verbose("Received message {MessageType} from {ConnectionId}",
				message.MessageType,
				Context.ConnectionId);

			return message.MessageType switch
			{
				MessageType.Connect => await HandleConnect(message),
				MessageType.RegisterPlugin => await HandleRegisterPlugin(message),
				MessageType.Heartbeat => await HandleHeartbeat(message),
				MessageType.LogMessage => await HandleLogMessage(message),
				MessageType.KeyExchange => await HandleKeyExchange(message),
				MessageType.InvokeActionResponse => await HandleInvokeActionResponse(message),
				_ => CreateErrorResponse(message.MessageId, $"Unknown message type: {message.MessageType}")
			};
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error processing message from {ConnectionId}", Context.ConnectionId);
			return CreateErrorResponse(Guid.NewGuid().ToString(), "Internal server error");
		}
	}

	private async Task<byte[]> HandleConnect(PluginMessage message)
	{
		var connectMessage = ConnectMessage.Parser.ParseFrom(message.Payload);
		var success = await _pluginRegistry.RegisterPlugin(Context.ConnectionId, connectMessage);

		var plugin = await _pluginRegistry.GetPluginByConnectionId(Context.ConnectionId);
		var publicKey = GenerateServerKeyPair();

		var response = new ConnectResponseMessage
		{
			Success = success,
			Message = success ? "Connected successfully" : "Connection failed",
			SessionId = plugin?.SessionId ?? "",
			PublicKey = ByteString.CopyFrom(publicKey)
		};

		return CreateResponse(message.MessageId, MessageType.ConnectResponse, response);
	}

	private async Task<byte[]> HandleRegisterPlugin(PluginMessage message)
	{
		var registerMessage = RegisterPluginMessage.Parser.ParseFrom(message.Payload);
		var success = await _pluginRegistry.UpdatePluginActions(message.PluginId, registerMessage.Actions);

		var response = new RegisterPluginResponseMessage
		{
			Success = success,
			Message = success ? "Plugin registered successfully" : "Plugin registration failed"
		};

		return CreateResponse(message.MessageId, MessageType.RegisterPluginResponse, response);
	}

	private async Task<byte[]> HandleHeartbeat(PluginMessage message)
	{
		await _pluginRegistry.UpdateHeartbeat(Context.ConnectionId);
		return CreateResponse(message.MessageId,
			MessageType.Heartbeat,
			new HeartbeatMessage
			{
				Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
			});
	}

	private Task<byte[]> HandleLogMessage(PluginMessage message)
	{
		var logMessage = LogMessage.Parser.ParseFrom(message.Payload);

		switch (logMessage.Level)
		{
			case ProtobufLogLevel.Trace:
				_logger.Verbose("[Plugin:{PluginId}] [{Category}] {Message}",
					message.PluginId,
					logMessage.Category,
					logMessage.Message);
				break;
			case ProtobufLogLevel.Debug:
				_logger.Debug("[Plugin:{PluginId}] [{Category}] {Message}",
					message.PluginId,
					logMessage.Category,
					logMessage.Message);
				break;
			case ProtobufLogLevel.Information:
				_logger.Information("[Plugin:{PluginId}] [{Category}] {Message}",
					message.PluginId,
					logMessage.Category,
					logMessage.Message);
				break;
			case ProtobufLogLevel.Warning:
				_logger.Warning("[Plugin:{PluginId}] [{Category}] {Message}",
					message.PluginId,
					logMessage.Category,
					logMessage.Message);
				break;
			case ProtobufLogLevel.Error:
				_logger.Error("[Plugin:{PluginId}] [{Category}] {Message}",
					message.PluginId,
					logMessage.Category,
					logMessage.Message);
				break;
			case ProtobufLogLevel.Critical:
				_logger.Fatal("[Plugin:{PluginId}] [{Category}] {Message}",
					message.PluginId,
					logMessage.Category,
					logMessage.Message);
				break;
			default:
				_logger.Information("[Plugin:{PluginId}] [{Category}] {Message}",
					message.PluginId,
					logMessage.Category,
					logMessage.Message);
				break;
		}

		return Task.FromResult(CreateResponse(message.MessageId, MessageType.LogMessage, new LogMessage()));
	}

	private async Task<byte[]> HandleKeyExchange(PluginMessage message)
	{
		var keyExchangeMessage = KeyExchangeMessage.Parser.ParseFrom(message.Payload);

		// Generate server key pair
		var (serverPublicKey, _) = _encryptionService.GenerateKeyPair();

		// Generate session key
		var sessionKey = _encryptionService.GenerateSessionKey();

		// Encrypt session key with client's public key
		var clientPublicKey = keyExchangeMessage.PublicKey.ToByteArray();
		var encryptedSessionKey = _encryptionService.EncryptSessionKey(sessionKey, clientPublicKey);

		// Store session key for this plugin
		var plugin = await _pluginRegistry.GetPluginByConnectionId(Context.ConnectionId);
		if (plugin != null)
		{
			plugin.SessionKey = sessionKey;
			_logger.Debug("Stored session key for plugin {PluginId}", plugin.PluginId);
		}

		var response = new KeyExchangeResponseMessage
		{
			PublicKey = ByteString.CopyFrom(serverPublicKey),
			EncryptedSessionKey = ByteString.CopyFrom(encryptedSessionKey)
		};

		return CreateResponse(message.MessageId, MessageType.KeyExchangeResponse, response);
	}

	private Task<byte[]> HandleInvokeActionResponse(PluginMessage message)
	{
		var response = InvokeActionResponseMessage.Parser.ParseFrom(message.Payload);
		_logger.Debug("Received action response from plugin {PluginId}: Success={Success}, Message={Message}",
			message.PluginId,
			response.Success,
			response.Message);

		return Task.FromResult(Array.Empty<byte>());
	}

	private static byte[] CreateResponse(string messageId, MessageType messageType, IMessage payload)
	{
		var response = new PluginMessage
		{
			MessageId = messageId,
			MessageType = messageType,
			Payload = ByteString.CopyFrom(payload.ToByteArray())
		};

		return response.ToByteArray();
	}

	private byte[] CreateErrorResponse(string messageId, string error)
	{
		var response = new ConnectResponseMessage
		{
			Success = false,
			Message = error
		};

		return CreateResponse(messageId, MessageType.ConnectResponse, response);
	}

	private static byte[] GenerateServerKeyPair()
	{
		using var rsa = RSA.Create(2048);
		return rsa.ExportRSAPublicKey();
	}
}
