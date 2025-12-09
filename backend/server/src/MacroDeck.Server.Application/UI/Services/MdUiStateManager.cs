using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using MacroDeck.SDK.UI.Core;
using MacroDeck.SDK.UI.Registry;
using MacroDeck.SDK.UI.Serialization;
using Serilog;

namespace MacroDeck.Server.Application.UI.Services;

/// <summary>
///     Manages UI state for sessions
/// </summary>
public class MdUiStateManager
{
	private static readonly ILogger Log = Serilog.Log.ForContext<MdUiStateManager>();
	private readonly object _callbackLock = new();
	private readonly IMdUiRegistry _registry;
	private readonly ConcurrentDictionary<string, SessionState> _sessions = new();
	private bool _callbackInitialized;
	private IMdUiUpdateService? _updateService;

	public MdUiStateManager(IMdUiRegistry registry)
	{
		_registry = registry;
		InitializeViewChangedCallback();
	}

	/// <summary>
	///     Initialize the global view changed callback once
	/// </summary>
	private void InitializeViewChangedCallback()
	{
		lock (_callbackLock)
		{
			if (_callbackInitialized)
			{
				return;
			}

			// Set up change notification ONCE for all sessions
			MdUiFramework.SetViewChangedCallback(changedView =>
			{
				// When any view changes, find which session it belongs to and rebuild
				var session = _sessions.Values.FirstOrDefault(s =>
					s.RootView == changedView || IsChildOf(changedView, s.RootView));

				if (session != null)
				{
					// Schedule a rebuild (debounced to avoid multiple rebuilds per change batch)
					ScheduleRebuild(session);
				}
			});

			_callbackInitialized = true;
		}
	}

	/// <summary>
	///     Sets the update service (called after DI is fully configured)
	/// </summary>
	public void SetUpdateService(IMdUiUpdateService updateService)
	{
		_updateService = updateService;
	}

	/// <summary>
	///     Creates or gets a session state
	/// </summary>
	public SessionState GetOrCreateSession(string sessionId)
	{
		return _sessions.GetOrAdd(sessionId, _ => new SessionState(sessionId));
	}

	/// <summary>
	///     Removes a session
	/// </summary>
	public void RemoveSession(string sessionId)
	{
		if (_sessions.TryRemove(sessionId, out var session))
		{
			session.Dispose();
		}
	}

	/// <summary>
	///     Sets the root view for a session
	/// </summary>
	public ViewTreeNode SetRootView(string sessionId, string viewId)
	{
		Log.Debug("SetRootView called for session {SessionId}, view {ViewId}", sessionId, viewId);
		var session = GetOrCreateSession(sessionId);
		Log.Debug("Session created/retrieved for {SessionId}, total sessions: {Count}", sessionId, _sessions.Count);

		var view = _registry.CreateViewInstance(viewId);

		// If it's a stateful view, set the session ID so OpenLink can work
		if (view is StatefulMdUiView statefulView)
		{
			statefulView.EnsureState();
			var state = GetStateFromView(statefulView);
			state?.SetSessionId(sessionId);
		}

		session.RootView = view;
		session.RootViewId = viewId;

		// Build the initial view tree
		var tree = BuildViewTree(sessionId)!;

		// Store the initial tree for future diffs
		session.LastSentTree = tree;

		Log.Debug("SetRootView completed for session {SessionId}, ViewMap has {ViewMapCount} views",
			sessionId,
			session.ViewMap?.Count ?? 0);

		return tree;
	}

	/// <summary>
	///     Gets the state from a stateful view using reflection
	/// </summary>
	private MdUiState? GetStateFromView(StatefulMdUiView view)
	{
		var stateField = view.GetType()
			.BaseType?.GetField("_state",
				BindingFlags.NonPublic | BindingFlags.Instance);
		return stateField?.GetValue(view) as MdUiState;
	}

	private void ScheduleRebuild(SessionState session)
	{
		// Cancel any pending rebuild
		session.RebuildCancellationTokenSource?.Cancel();
		session.RebuildCancellationTokenSource = new CancellationTokenSource();
		var cancellationToken = session.RebuildCancellationTokenSource.Token;

		// Schedule a new rebuild with a small delay to debounce multiple changes
		_ = Task.Run(async () =>
			{
				try
				{
					// Wait a tiny bit to batch multiple state changes
					await Task.Delay(5, cancellationToken);

					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}

					// Rebuild synchronously to avoid race conditions
					ViewTreeNode? updatedTree;
					PropertyUpdateBatch? propertyUpdates = null;

					lock (session.RebuildLock)
					{
						var previousTree = session.LastSentTree;
						updatedTree = BuildViewTree(session.SessionId);

						if (updatedTree != null)
						{
							// If we have a previous tree, compute the diff
							if (previousTree != null)
							{
								propertyUpdates = ComputePropertyDiff(session.SessionId, previousTree, updatedTree);
							}

							// Store the new tree for future diffs
							session.LastSentTree = updatedTree;
						}
					}

					// Send outside the lock
					if (_updateService != null)
					{
						if (propertyUpdates != null && propertyUpdates.Updates.Count > 0)
						{
							// Send incremental property updates
							await _updateService.SendPropertyUpdatesAsync(propertyUpdates);
						}
						else if (updatedTree != null)
						{
							// Send full tree if structural changes detected (propertyUpdates is null) or no previous tree
							Log.Debug("Sending full view tree for session {SessionId}", session.SessionId);
							await _updateService.SendViewTreeAsync(session.SessionId, updatedTree);
						}
					}
				}
				catch (OperationCanceledException)
				{
					// Rebuild was cancelled, this is expected
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Error during scheduled rebuild for session {SessionId}", session.SessionId);
				}
			},
			cancellationToken);
	}

	/// <summary>
	///     Computes the property differences between two view trees.
	///     Returns null if structural changes were detected (nodes added/removed).
	/// </summary>
	private PropertyUpdateBatch? ComputePropertyDiff(string sessionId, ViewTreeNode oldTree, ViewTreeNode newTree)
	{
		var batch = new PropertyUpdateBatch { SessionId = sessionId };
		var oldNodes = FlattenTree(oldTree).ToDictionary(n => n.NodeId);
		var newNodes = FlattenTree(newTree).ToDictionary(n => n.NodeId);

		// Check for structural changes (nodes added or removed)
		var oldNodeIds = new HashSet<string>(oldNodes.Keys);
		var newNodeIds = new HashSet<string>(newNodes.Keys);

		if (!oldNodeIds.SetEquals(newNodeIds))
		{
			// Structural change detected - need full tree update
			return null;
		}

		// Check all new nodes for property changes
		foreach (var (nodeId, newNode) in newNodes)
		{
			if (oldNodes.TryGetValue(nodeId, out var oldNode))
			{
				// Node exists in both trees - check for property changes
				var changedProperties = new Dictionary<string, object?>();

				// Check all properties in the new node
				foreach (var (key, newValue) in newNode.Properties)
				{
					if (!oldNode.Properties.TryGetValue(key, out var oldValue)
						|| !ArePropertiesEqual(oldValue, newValue))
					{
						changedProperties[key] = newValue;
					}
				}

				// Check for removed properties
				foreach (var key in oldNode.Properties.Keys)
				{
					if (!newNode.Properties.ContainsKey(key))
					{
						changedProperties[key] = null;
					}
				}

				if (changedProperties.Count > 0)
				{
					batch.Updates.Add(new PropertyUpdate
					{
						NodeId = nodeId,
						Properties = changedProperties
					});
				}
			}
		}

		return batch;
	}

	/// <summary>
	///     Flattens a view tree into a list of nodes
	/// </summary>
	private IEnumerable<ViewTreeNode> FlattenTree(ViewTreeNode node)
	{
		yield return node;

		foreach (var child in node.Children)
		{
			foreach (var descendant in FlattenTree(child))
			{
				yield return descendant;
			}
		}
	}

	/// <summary>
	///     Compares two property values for equality
	/// </summary>
	private bool ArePropertiesEqual(object? oldValue, object? newValue)
	{
		if (oldValue == null && newValue == null)
		{
			return true;
		}

		if (oldValue == null || newValue == null)
		{
			return false;
		}

		// For complex objects, use JSON comparison
		if (oldValue is Dictionary<string, object?> || newValue is Dictionary<string, object?>)
		{
			var oldJson = JsonSerializer.Serialize(oldValue);
			var newJson = JsonSerializer.Serialize(newValue);
			return oldJson == newJson;
		}

		return oldValue.Equals(newValue);
	}

	private bool IsChildOf(MdUiView child, MdUiView? parent)
	{
		if (parent == null)
		{
			return false;
		}

		if (parent == child)
		{
			return true;
		}

		// For stateful views, check the built child
		if (parent is StatefulMdUiView statefulView)
		{
			statefulView.EnsureState();
			var builtChild = statefulView.Build();
			if (builtChild != parent)
			{
				return IsChildOf(child, builtChild);
			}
		}

		return false;
	}

	/// <summary>
	///     Builds the current view tree for a session
	/// </summary>
	public ViewTreeNode? BuildViewTree(string sessionId)
	{
		// Try to get existing session - don't create a new one
		if (!_sessions.TryGetValue(sessionId, out var session))
		{
			Log.Warning("Session {SessionId} not found when building view tree", sessionId);
			return null;
		}

		if (session.RootView == null)
		{
			return null;
		}

		lock (session.RebuildLock)
		{
			// Use the session's serializer to maintain stable ViewIds
			var tree = session.Serializer.Serialize(session.RootView);

			// Store the view map in the session for event handling
			session.ViewMap = new Dictionary<string, MdUiView>(session.Serializer.GetViewMap());
			return tree;
		}
	}

	/// <summary>
	///     Handles an event on a view
	/// </summary>
	public void HandleEvent(string sessionId, string viewId, string eventName, Dictionary<string, object>? parameters)
	{
		Log.Debug("Handling event {EventName} for view {ViewId} in session {SessionId}. Total sessions: {Count}",
			eventName,
			viewId,
			sessionId,
			_sessions.Count);

		// Try to get existing session - don't create a new one
		if (!_sessions.TryGetValue(sessionId, out var session))
		{
			Log.Warning(
				"Session {SessionId} not found - session may have expired or never been created. Available sessions: {Sessions}",
				sessionId,
				string.Join(", ", _sessions.Keys));
			return;
		}

		if (session.RootView == null)
		{
			Log.Warning("No root view found for session {SessionId}", sessionId);
			return;
		}

		MdUiView? view;

		// Lock to safely access the view map
		lock (session.RebuildLock)
		{
			// Look up the view in the view map
			if (session.ViewMap != null && session.ViewMap.TryGetValue(viewId, out view))
			{
				Log.Debug("Found view {ViewId} in view map (total: {ViewMapCount}), invoking event handler",
					viewId,
					session.ViewMap.Count);
			}
			else
			{
				Log.Warning(
					"View {ViewId} not found in tree for session {SessionId}. ViewMap has {ViewMapCount} views: {ViewIds}",
					viewId,
					sessionId,
					session.ViewMap?.Count ?? 0,
					session.ViewMap != null ? string.Join(", ", session.ViewMap.Keys) : "null");
				return;
			}
		}

		// Invoke the handler outside the lock to prevent deadlocks
		InvokeEventHandler(view, eventName, parameters);
	}

	private object? ConvertValue(object? value, Type targetType)
	{
		if (value == null)
		{
			return null;
		}

		// Handle JsonElement (from System.Text.Json deserialization)
		if (value is JsonElement jsonElement)
		{
			if (targetType == typeof(bool))
			{
				return jsonElement.GetBoolean();
			}

			if (targetType == typeof(int))
			{
				return jsonElement.GetInt32();
			}

			if (targetType == typeof(long))
			{
				return jsonElement.GetInt64();
			}

			if (targetType == typeof(double))
			{
				return jsonElement.GetDouble();
			}

			if (targetType == typeof(string))
			{
				return jsonElement.GetString();
			}

			// For other types, try to deserialize
			return JsonSerializer.Deserialize(jsonElement.GetRawText(), targetType);
		}

		// If the value is already the correct type, return it
		if (targetType.IsInstanceOfType(value))
		{
			return value;
		}

		// Try to convert using Convert.ChangeType
		try
		{
			return Convert.ChangeType(value, targetType);
		}
		catch
		{
			Log.Warning("Failed to convert value {Value} to type {TargetType}", value, targetType);
			return value;
		}
	}

	private void InvokeEventHandler(MdUiView view, string eventName, Dictionary<string, object>? parameters)
	{
		Log.Debug("Invoking event handler for event {EventName} on view {ViewType}", eventName, view.GetType().Name);

		var handlerName = "On" + char.ToUpperInvariant(eventName[0]) + eventName.Substring(1);
		var property = view.GetType().GetProperty(handlerName);

		if (property == null)
		{
			Log.Warning("No handler found for event {EventName} on view {ViewType}", eventName, view.GetType().Name);
			return;
		}

		var propertyType = property.PropertyType;
		var handler = property.GetValue(view);
		if (handler == null)
		{
			return;
		}

		// Handle Func<Task> (async click handlers)
		if (propertyType == typeof(Func<Task>))
		{
			var func = (Func<Task>)handler;
			_ = Task.Run(async () =>
			{
				try
				{
					await func();
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Error in async event handler {HandlerName}", handlerName);
				}
			});
			return;
		}

		// Handle Action (sync click handlers)
		if (propertyType == typeof(Action))
		{
			((Action)handler).Invoke();
			return;
		}

		// Handle Action<T> (e.g., OnChanged with value parameter)
		if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Action<>))
		{
			if (parameters != null && parameters.TryGetValue("value", out var rawValue))
			{
				var targetType = propertyType.GetGenericArguments()[0];
				var convertedValue = ConvertValue(rawValue, targetType);
				propertyType.GetMethod("Invoke")?.Invoke(handler, new[] { convertedValue });
			}

			return;
		}

		Log.Warning("Unsupported handler type {HandlerType} for event {EventName}", propertyType.Name, eventName);
	}
}

/// <summary>
///     Represents the state for a UI session
/// </summary>
public class SessionState : IDisposable
{
	public SessionState(string sessionId)
	{
		SessionId = sessionId;
		Serializer = new ViewTreeSerializer();
	}

	public string SessionId { get; }
	public MdUiView? RootView { get; set; }
	public string? RootViewId { get; set; }
	public Dictionary<string, MdUiView>? ViewMap { get; set; }

	/// <summary>
	///     Serializer instance that maintains stable ViewIds across rebuilds
	/// </summary>
	public ViewTreeSerializer Serializer { get; }

	/// <summary>
	///     Lock for thread-safe access to view map and rebuild operations
	/// </summary>
	public object RebuildLock { get; } = new();

	/// <summary>
	///     Cancellation token source for pending rebuilds (for debouncing)
	/// </summary>
	public CancellationTokenSource? RebuildCancellationTokenSource { get; set; }

	/// <summary>
	///     Last sent view tree for computing diffs
	/// </summary>
	public ViewTreeNode? LastSentTree { get; set; }

	public void Dispose()
	{
		lock (RebuildLock)
		{
			RebuildCancellationTokenSource?.Cancel();
			RebuildCancellationTokenSource?.Dispose();

			if (RootView is StatefulMdUiView statefulView)
			{
				statefulView.DisposeState();
			}

			ViewMap?.Clear();
		}
	}
}
