using MacroDeck.SDK.UI.DependencyInjection;

namespace MacroDeck.SDK.UI.Core;

/// <summary>
///     Base class for all UI views in the MacroDeck UI framework
/// </summary>
public abstract class MdUiView
{
    /// <summary>
    ///     Unique identifier for this view instance
    /// </summary>
    public string ViewId { get; set; } = string.Empty;

    /// <summary>
    ///     Optional padding around the view
    /// </summary>
    public EdgeInsets? Padding { get; set; }

    /// <summary>
    ///     Optional margin around the view
    /// </summary>
    public EdgeInsets? Margin { get; set; }

    /// <summary>
    ///     Custom CSS styles to apply inline
    /// </summary>
    public string? CustomCss { get; set; }

    /// <summary>
    ///     Custom CSS classes to apply
    /// </summary>
    public string[]? CustomClasses { get; set; }

    /// <summary>
    ///     Build method to construct the UI tree
    /// </summary>
    public abstract MdUiView Build();

    /// <summary>
    ///     Gets a service from the DI container
    /// </summary>
    protected T GetService<T>()
		where T : notnull
	{
		return MdUiServiceProvider.GetRequiredService<T>();
	}
}

/// <summary>
///     Represents spacing on all sides
/// </summary>
public record EdgeInsets(double Top, double Right, double Bottom, double Left)
{
	public static EdgeInsets All(double value)
	{
		return new EdgeInsets(value, value, value, value);
	}

	public static EdgeInsets Symmetric(double vertical, double horizontal)
	{
		return new EdgeInsets(vertical, horizontal, vertical, horizontal);
	}

	public static EdgeInsets Only(double top = 0, double right = 0, double bottom = 0, double left = 0)
	{
		return new EdgeInsets(top, right, bottom, left);
	}
}
