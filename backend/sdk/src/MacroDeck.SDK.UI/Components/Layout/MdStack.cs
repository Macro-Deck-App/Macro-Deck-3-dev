using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components.Layout;

/// <summary>
///     Stacks children on top of each other (z-axis)
/// </summary>
public class MdStack : StatelessMdUiView
{
	public MdStack(params MdUiView[] children)
	{
		Children = children.ToList();
	}

	public List<MdUiView> Children { get; set; }
	public StackAlignment? Alignment { get; set; }

	public override MdUiView Build()
	{
		return this;
	}
}

public enum StackAlignment
{
	TopLeft,
	TopCenter,
	TopRight,
	CenterLeft,
	Center,
	CenterRight,
	BottomLeft,
	BottomCenter,
	BottomRight
}
