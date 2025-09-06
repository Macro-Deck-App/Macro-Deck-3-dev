using System.Reflection;
using System.Text.Json;
using MacroDeck.SDK.PluginSDK.Actions;
using MacroDeck.SDK.PluginSDK.Configuration;
using MacroDeck.SDK.PluginSDK.Hubs;
using MacroDeck.SDK.PluginSDK.Messages;
using MacroDeck.SDK.PluginSDK.Options;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace MacroDeck.SDK.PluginSDK.Clients;

public class MacroDeckPluginTransport : IDisposable, IPluginCommunicationClient
{
	private readonly IIntegrationConfigurationProvider? _configurationProvider;
	private readonly ILogger _logger = Log.ForContext<MacroDeckPluginTransport>();
	private readonly PluginOptions _pluginOptions;
	private readonly Dictionary<string, Type> _registeredActionTypes = new();
	private readonly IServiceProvider _serviceProvider;
	private HubConnection? _connection;

	public MacroDeckPluginTransport(
		PluginOptions pluginOptions,
		IServiceProvider serviceProvider,
		IIntegrationConfigurationProvider? configurationProvider = null)
	{
		_pluginOptions = pluginOptions;
		_serviceProvider = serviceProvider;
		_configurationProvider = configurationProvider;
	}

	public bool IsConnected { get; private set; }

	public string? SessionId { get; private set; }

	public void Dispose()
	{
		IsConnected = false;
		_connection?.DisposeAsync();
	}


	public async Task InvokeAction(InvokeActionMessage message)
	{
		try
		{
			_logger.Debug("Received action invocation: {ActionId}", message.ActionId);

			if (!_registeredActionTypes.TryGetValue(message.ActionId, out var actionType))
			{
				_logger.Warning("Action not found: {ActionId}", message.ActionId);

				var notFoundResponse = new InvokeActionResponseMessage
				{
					Success = false,
					Message = $"Action '{message.ActionId}' not found"
				};

				await _connection!.InvokeAsync("InvokeActionResponse", _pluginOptions.Id, notFoundResponse);
				return;
			}

			// Create action instance from DI
			using var scope = _serviceProvider.CreateScope();
			var action = (MacroDeckAction)ActivatorUtilities.CreateInstance(scope.ServiceProvider, actionType);

			switch (action)
			{
				// Execute the action based on its type
				case SimpleMacroDeckAction simpleAction:
					simpleAction.Context = new ActionContext
					{
						Configuration = message.Parameters
					};
					await simpleAction.OnInvoke();
					break;
				case SliderMacroDeckAction sliderAction:
					/*	when message.Parameters?.TryGetValue("sliderValue", out var sliderValue) == true:
						// Handle different slider value types
						switch (sliderValue)
						{
							case int intValue:
								await sliderAction.OnSlide(intValue);
								break;
							case float floatValue:
								await sliderAction.OnSlide(floatValue);
								break;
							case double doubleValue:
								await sliderAction.OnSlide(doubleValue);
								break;
							case long longValue:
								await sliderAction.OnSlide(longValue);
								break;
							default:
								_logger.Warning("Unsupported slider value type: {ValueType}", sliderValue?.GetType());
								break;
						}*/

					break;
				default:
				{
					_logger.Warning(
						"Action {ActionId} of type {ActionType} is not supported or missing required parameters",
						message.ActionId,
						actionType.Name);

					var notSupportedResponse = new InvokeActionResponseMessage
					{
						Success = false,
						Message = $"Action '{message.ActionId}' type not supported or missing parameters"
					};

					await _connection!.InvokeAsync("InvokeActionResponse", _pluginOptions.Id, notSupportedResponse);
					return;
				}
			}

			var response = new InvokeActionResponseMessage
			{
				Success = true,
				Message = "Action invoked successfully"
			};

			await _connection!.InvokeAsync("InvokeActionResponse", _pluginOptions.Id, response);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error handling action invocation");

			var errorResponse = new InvokeActionResponseMessage
			{
				Success = false,
				Message = $"Error invoking action: {ex.Message}"
			};

			if (_connection != null)
			{
				await _connection.InvokeAsync("InvokeActionResponse", _pluginOptions.Id, errorResponse);
			}
		}
	}


	public Task RequestShutdown()
	{
		try
		{
			_logger.Information("Received shutdown request from server");

			// TODO: Implement graceful shutdown logic
			// This could trigger cleanup and dispose resources

			IsConnected = false;
			_connection?.DisposeAsync();

			return Task.CompletedTask;
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Error handling shutdown request");
			return Task.CompletedTask;
		}
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

			// Register server-to-client methods (type-safe)
			_connection.On<InvokeActionMessage>("InvokeAction", InvokeAction);
			_connection.On("RequestShutdown", RequestShutdown);
			_connection.Reconnected += OnReconnected;
			_connection.Closed += OnDisconnected;

			await _connection.StartAsync();

			var connectResult = await SendConnect();
			if (connectResult)
			{
				IsConnected = true;
				_logger.Information("Connected to MacroDeck server at {Url}", url);
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

	public async Task<bool> RegisterPlugin(List<Type> actionTypes)
	{
		if (!IsConnected || _connection == null)
		{
			_logger.Warning("Cannot register plugin - not connected");
			return false;
		}

		try
		{
			// Store action types locally for execution
			_registeredActionTypes.Clear();
			var actionDefinitions = new List<ActionDefinition>();

			foreach (var actionType in actionTypes)
			{
				// Create a temporary instance to get the action metadata
				var tempAction = (MacroDeckAction)ActivatorUtilities.CreateInstance(_serviceProvider, actionType);

				_registeredActionTypes[tempAction.Id] = actionType;
				_logger.Verbose("Registered action type locally: {ActionId} - {ActionName} ({ActionType})",
					tempAction.Id,
					tempAction.Name,
					actionType.Name);

				actionDefinitions.Add(new ActionDefinition
				{
					ActionId = tempAction.Id,
					ActionName = tempAction.Name
				});
			}

			var registerMessage = new RegisterExtensionMessage
			{
				Actions = actionDefinitions
			};

			var registerResponse = await _connection!.InvokeAsync<RegisterExtensionResponseMessage>("RegisterExtension",
				_pluginOptions.Id,
				registerMessage);

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
			var logMessage = new LogMessage
			{
				Level = level,
				Message = message,
				Category = category,
				Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
				ExceptionJson = exception != null ? JsonSerializer.Serialize(exception) : null
			};

			await _connection!.InvokeAsync("LogMessage", _pluginOptions.Id, logMessage);
		}
		catch (Exception ex)
		{
			// Avoid infinite logging loop
			System.Console.WriteLine($"Failed to send log message: {ex.Message}");
		}
	}

	private async Task<bool> SendConnect()
	{
		// Auto-detect SDK version from assembly
		var sdkVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

		var connectMessage = new ConnectMessage
		{
			ExtensionId = _pluginOptions.Id,
			ExtensionName = _pluginOptions.Name,
			ExtensionVersion = _pluginOptions.Version.ToString(),
			SdkVersion = sdkVersion
		};

		try
		{
			var connectResponse = await _connection!.InvokeAsync<ConnectResponseMessage>("Connect", connectMessage);

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
