using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components;

/// <summary>
///     Displays a loading spinner
/// </summary>
public class MdLoading : StatelessMdUiView
{
    /// <summary>
    ///     Size of the spinner (small, medium, large)
    /// </summary>
    public LoadingSize Size { get; set; } = LoadingSize.Medium;

	public override MdUiView Build()
	{
		return this;
	}
}

public enum LoadingSize
{
	Small,
	Medium,
	Large
}
