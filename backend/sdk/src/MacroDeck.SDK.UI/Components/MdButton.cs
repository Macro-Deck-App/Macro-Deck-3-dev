using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components;

public class MdButton : StatelessMdUiView
{

	public MdButton(string text, Func<Task>? onClick = null)
	{
		Text = text;
		OnClick = onClick;
	}

	public MdButton(string text, Action? onClick)
	{
		Text = text;
		if (onClick != null)
		{
			OnClick = () =>
			{
				onClick();
				return Task.CompletedTask;
			};
		}
	}

	public MdButton(MdUiView child, Func<Task>? onClick = null)
	{
		Child = child;
		OnClick = onClick;
	}

	public MdButton(MdUiView child, Action? onClick)
	{
		Child = child;
		if (onClick != null)
		{
			OnClick = () =>
			{
				onClick();
				return Task.CompletedTask;
			};
		}
	}

	public string? Text { get; set; }
	public Func<Task>? OnClick { get; set; }
	public MdUiView? Child { get; set; }
	public ButtonRole Role { get; set; } = ButtonRole.Primary;
	public string? BackgroundColor { get; set; }
	public string? TextColor { get; set; }
	public bool Disabled { get; set; }

	public override MdUiView Build()
	{
		return this;
	}
}

public enum ButtonRole
{
	Primary,
	Secondary,
	Success,
	Danger,
	Warning,
	Info,
	Light,
	Dark,
	Link
}
