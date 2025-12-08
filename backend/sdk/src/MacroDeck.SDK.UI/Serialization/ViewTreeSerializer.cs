using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using MacroDeck.SDK.UI.Components.Layout;
using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Serialization;

/// <summary>
///     Serializes UI views into a tree structure for transport
/// </summary>
public class ViewTreeSerializer
{
	private readonly Dictionary<string, string> _pathToIdMap = new(); // Maps stable path -> stable ID
	private readonly ConditionalWeakTable<MdUiView, string> _viewIdCache = new();
	private readonly Dictionary<string, MdUiView> _viewMap = new();
	private string? _lastRootTypeHash;

	/// <summary>
	///     Serializes a view into a ViewTreeNode
	/// </summary>
	public ViewTreeNode Serialize(MdUiView view)
	{
		_viewMap.Clear(); // Clear map for each serialization

		// Detect major structural changes by checking if the root view type changed
		// This helps identify navigation between different pages/views
		var currentRootTypeHash = ComputeRootHash(view);
		if (_lastRootTypeHash != null && _lastRootTypeHash != currentRootTypeHash)
		{
			// Root structure changed significantly - invalidate cache
			_pathToIdMap.Clear();
		}

		_lastRootTypeHash = currentRootTypeHash;

		// Note: We keep _viewIdCache for root/stateful views to maintain their identity
		return SerializeInternal(view, "root");
	}

	/// <summary>
	///     Computes a hash representing the root structure of the view tree.
	///     Used to detect when major structural changes occur.
	/// </summary>
	private string ComputeRootHash(MdUiView view)
	{
		// Build the root view to get its actual structure
		var builtView = view;
		if (view is StatefulMdUiView stateful)
		{
			stateful.EnsureState();
			builtView = stateful.Build();
		}
		else if (view is StatelessMdUiView stateless)
		{
			builtView = stateless.Build();
		}

		// Compute a deeper structural hash
		return ComputeStructuralHash(builtView, 0, 4);
	}

	/// <summary>
	///     Recursively computes a structural hash of the view tree.
	///     Only considers types and counts, not values.
	/// </summary>
	private string ComputeStructuralHash(MdUiView view, int depth, int maxDepth)
	{
		if (depth >= maxDepth)
		{
			return view.GetType().Name;
		}

		// Build stateful/stateless views to get actual structure
		var actualView = view;
		if (view is StatefulMdUiView stateful)
		{
			stateful.EnsureState();
			var built = stateful.Build();
			if (built != view)
			{
				actualView = built;
			}
		}
		else if (view is StatelessMdUiView stateless)
		{
			var built = stateless.Build();
			if (built != view)
			{
				actualView = built;
			}
		}

		var hash = actualView.GetType().Name;
		var children = GetImmediateChildren(actualView).ToList();

		// Include child count in hash (helps detect when children are added/removed)
		hash += $"[{children.Count}]";

		// Add hashes of children
		foreach (var child in children.Take(15)) // Increased from 10 to 15 for better detection
		{
			hash += ">" + ComputeStructuralHash(child, depth + 1, maxDepth);
		}

		return hash;
	}

	/// <summary>
	///     Gets immediate children of a view without building them.
	/// </summary>
	private IEnumerable<MdUiView> GetImmediateChildren(MdUiView view)
	{
		var type = view.GetType();
		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

		foreach (var prop in properties)
		{
			if (prop.PropertyType == typeof(MdUiView) || prop.PropertyType.IsSubclassOf(typeof(MdUiView)))
			{
				var child = prop.GetValue(view) as MdUiView;
				if (child != null)
				{
					yield return child;
				}
			}
			else if (typeof(IEnumerable<MdUiView>).IsAssignableFrom(prop.PropertyType))
			{
				if (prop.GetValue(view) is IEnumerable<MdUiView> children)
				{
					foreach (var child in children)
					{
						yield return child;
					}
				}
			}
		}
	}

	/// <summary>
	///     Invalidates the path cache, causing new IDs to be generated for changed structures.
	///     Call this when you know the view structure has fundamentally changed (e.g., navigation to different page).
	/// </summary>
	public void InvalidateCache()
	{
		_pathToIdMap.Clear();
		_lastRootTypeHash = null;
	}

	private ViewTreeNode SerializeInternal(MdUiView view, string path)
	{
		while (true)
		{
			// Generate stable ViewId based on the path in the tree, not just the instance
			// This ensures that views in the same position get the same ID even if rebuilt
			string viewId;

			// First check if we already have a cached ID for this instance (for root/stateful views)
			if (_viewIdCache.TryGetValue(view, out var cachedId))
			{
				viewId = cachedId;
			}
			// Then check if we have a stable ID for this path
			else if (_pathToIdMap.TryGetValue(path, out var pathId))
			{
				viewId = pathId;
			}
			// Otherwise generate a new ID and cache it by path
			else
			{
				viewId = Guid.NewGuid().ToString();
				_pathToIdMap[path] = viewId;
			}

			// Also cache by instance for root/stateful views
			if (!_viewIdCache.TryGetValue(view, out _))
			{
				_viewIdCache.Add(view, viewId);
			}

			view.ViewId = viewId;

			// Store view in map for event handling
			_viewMap[view.ViewId] = view;

			switch (view)
			{
				// For Stateful/Stateless views, build and serialize the result directly
				case StatefulMdUiView statefulView:
				{
					statefulView.EnsureState();
					var builtView = statefulView.Build();
					if (builtView != view)
					{
						// Serialize the built view with the same path to maintain stable IDs
						view = builtView;
						continue;
					}

					break;
				}
				case StatelessMdUiView:
				{
					var builtView = view.Build();
					if (builtView != view)
					{
						view = builtView;
						continue;
					}

					break;
				}
			}

			// For concrete components, serialize normally
			var node = new ViewTreeNode
			{
				ComponentType = view.GetType().Name,
				NodeId = view.ViewId
			};

			// Serialize common properties
			if (view.Padding != null)
			{
				node.Properties["padding"] = new
				{
					top = view.Padding.Top,
					right = view.Padding.Right,
					bottom = view.Padding.Bottom,
					left = view.Padding.Left
				};
			}

			if (view.Margin != null)
			{
				node.Properties["margin"] = new
				{
					top = view.Margin.Top,
					right = view.Margin.Right,
					bottom = view.Margin.Bottom,
					left = view.Margin.Left
				};
			}

			// Serialize component-specific properties
			SerializeProperties(view, node);

			// Serialize children if this is a container component
			SerializeChildren(view, node);

			return node;
		}
	}

	/// <summary>
	///     Gets the view map created during serialization
	/// </summary>
	public IReadOnlyDictionary<string, MdUiView> GetViewMap()
	{
		return _viewMap;
	}

	private void SerializeProperties(MdUiView view, ViewTreeNode node)
	{
		var type = view.GetType();
		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(p => p.CanRead
				&& p.Name != nameof(MdUiView.ViewId)
				&& p.Name != nameof(MdUiView.Padding)
				&& p.Name != nameof(MdUiView.Margin));

		foreach (var prop in properties)
		{
			var value = prop.GetValue(view);

			// Check if this is an event handler (Action or Action<T>)
			if (prop.PropertyType == typeof(Action)
				|| prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Action<>))
			{
				if (value != null && prop.Name.StartsWith("On"))
				{
					// Convert OnClick -> click, OnChanged -> changed
					var eventName = char.ToLowerInvariant(prop.Name[2]) + prop.Name.Substring(3);
					node.Events.Add(eventName);
				}

				continue; // Don't serialize the Action itself as a property
			}

			if (value != null)
			{
				// Convert property name to camelCase for frontend
				var propName = char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1);
				var serializedValue = SerializeValue(value);
				if (serializedValue != null)
				{
					node.Properties[propName] = serializedValue;
				}
			}
		}
	}

	private void SerializeChildren(MdUiView view, ViewTreeNode node)
	{
		var childIndex = 0;

		// Check for Child property (single child containers)
		var childProp = view.GetType().GetProperty("Child");
		if (childProp != null)
		{
			if (childProp.GetValue(view) is MdUiView child && child.Visible)
			{
				var childPath = $"{view.ViewId}.child";
				node.Children.Add(SerializeInternal(child, childPath));
				return;
			}
		}

		// Check for Children property (multi-child containers)
		var childrenProp = view.GetType().GetProperty("Children");
		if (childrenProp != null && childrenProp.GetValue(view) is IEnumerable<MdUiView> children)
		{
			foreach (var child in children.Where(c => c.Visible))
			{
				var childPath = $"{view.ViewId}.child[{childIndex++}]";
				node.Children.Add(SerializeInternal(child, childPath));
			}
		}
	}

	private static object? SerializeValue(object? value)
	{
		while (true)
		{
			if (value == null)
			{
				return null;
			}

			var type = value.GetType();

			// Handle primitives and strings
			if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
			{
				return value;
			}

			// Handle enums - convert to lowercase string for frontend
			if (type.IsEnum)
			{
				return value.ToString()!.ToLowerInvariant();
			}

			switch (value)
			{
				// Handle IState
				case IState state:
					value = state.GetValue();
					continue;
				// Handle BorderRadius
				case BorderRadius borderRadius:
					return new
					{
						topLeft = borderRadius.TopLeft,
						topRight = borderRadius.TopRight,
						bottomRight = borderRadius.BottomRight,
						bottomLeft = borderRadius.BottomLeft
					};
				// Handle Border
				case Border border:
					return new
					{
						width = border.Width,
						color = border.Color,
						style = border.Style.ToString().ToLowerInvariant()
					};
				// Handle collections of views (but don't serialize them as values)
				case IEnumerable<MdUiView>:
				// Handle MdUiView (single child)
				// These are handled separately in SerializeChildren
				case MdUiView:
					// These are handled separately in SerializeChildren
					return null;
				// Handle collections
				case IEnumerable enumerable when type != typeof(string):
				{
					var list = new List<object?>();
					foreach (var item in enumerable)
					{
						list.Add(SerializeValue(item));
					}

					return list;
				}
				default:
					return value.ToString();
			}
		}
	}
}
