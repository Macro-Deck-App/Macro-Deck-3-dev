using MacroDeck.SDK.UI.Core;
using MacroDeck.SDK.UI.Registry;

namespace MacroDeck.SDK.UI.Extensions;

/// <summary>
///     Collection of UI views to register from a plugin
/// </summary>
public class UiViewCollection
{
	private readonly List<ViewRegistration> _views = [];

    /// <summary>
    ///     Add a view with automatic ViewId from attribute
    /// </summary>
    public void Add<TView>()
		where TView : MdUiView
	{
		var viewType = typeof(TView);

		if (viewType.GetCustomAttributes(typeof(MdUiViewAttribute), false)
				.FirstOrDefault() is not MdUiViewAttribute attribute)
		{
			throw new InvalidOperationException($"View type {viewType.Name} must have MdUiViewAttribute with ViewId");
		}

		_views.Add(new ViewRegistration
		{
			ViewType = viewType,
			ViewId = attribute.ViewId ?? viewType.FullName!
		});
	}

    /// <summary>
    ///     Add a view with explicit ViewId
    /// </summary>
    public void Add<TView>(string viewId)
		where TView : MdUiView
	{
		_views.Add(new ViewRegistration
		{
			ViewType = typeof(TView),
			ViewId = viewId
		});
	}

    /// <summary>
    ///     Get all registered views
    /// </summary>
    public IReadOnlyList<ViewRegistration> GetViews()
	{
		return _views.AsReadOnly();
	}
}

/// <summary>
///     View registration info
/// </summary>
public class ViewRegistration
{
	public required Type ViewType { get; init; }
	public required string ViewId { get; init; }
}
