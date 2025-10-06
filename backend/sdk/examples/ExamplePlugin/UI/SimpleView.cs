using MacroDeck.SDK.UI.Components;
using MacroDeck.SDK.UI.Components.Layout;
using MacroDeck.SDK.UI.Core;
using MacroDeck.SDK.UI.Registry;

namespace ExamplePlugin.UI;

/// <summary>
///     Simple stateless view example
/// </summary>
[MdUiView(ViewId = "app.macrodeck.exampleplugin.SimpleView")]
public class SimpleView : StatelessMdUiView
{
	public string Title { get; set; } = "Simple View";
	public string Description { get; set; } = "This is a stateless view with no internal state.";

	public override MdUiView Build()
	{
		return new MdContainer
		{
			Padding = EdgeInsets.All(20),
			Child = new MdColumn(new MdText(Title)
				{
					FontSize = 20,
					FontWeight = FontWeight.Bold,
					Color = "#333333"
				},
				new MdText(Description)
				{
					FontSize = 14,
					Color = "#666666",
					Margin = EdgeInsets.Only(10)
				})
		};
	}
}
