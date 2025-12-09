using MacroDeck.SDK.UI.DependencyInjection;

namespace MacroDeck.SDK.UI.Core;

public abstract class MdUiView
{
	public string ViewId { get; set; } = string.Empty;
	public bool Visible { get; set; } = true;
	public EdgeInsets? Padding { get; set; }
	public EdgeInsets? Margin { get; set; }
	public string? CustomCss { get; set; }
	public string[]? CustomClasses { get; set; }

	public abstract MdUiView Build();

	protected T GetService<T>()
		where T : notnull
	{
		return MdUiServiceProvider.GetRequiredService<T>();
	}
}

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
