using MacroDeck.SDK.UI.Components;
using MacroDeck.SDK.UI.Components.Input;
using MacroDeck.SDK.UI.Components.Layout;
using MacroDeck.SDK.UI.Core;
using MacroDeck.SDK.UI.Registry;

namespace ExamplePlugin.UI;

/// <summary>
///     Example configuration view demonstrating the UI framework
/// </summary>
[MdUiView(ViewId = "app.macrodeck.exampleplugin.ConfigView")]
public class ExampleConfigView : StatefulMdUiView
{
	public override MdUiState CreateState()
	{
		return new ExampleConfigState();
	}
}

public class ExampleConfigState : MdUiState
{
	private State<int> _counter = null!;
	private Computed<string> _displayText = null!;
	private State<bool> _switchValue = null!;
	private State<string> _textValue = null!;

	public override void InitState()
	{
		_counter = CreateState(0);
		_textValue = CreateState("Hello from Example Plugin!");
		_switchValue = CreateState(false);

		_displayText = CreateComputed(() => $"Counter: {_counter.Value}, Switch: {(_switchValue.Value ? "ON" : "OFF")}",
			_counter,
			_switchValue);
	}

	public override MdUiView Build()
	{
		return new MdContainer
		{
			Padding = EdgeInsets.All(20),
			Child = new MdColumn(new MdText("Example Plugin Configuration")
				{
					FontSize = 24,
					FontWeight = FontWeight.Bold,
					Margin = EdgeInsets.Only(bottom: 20)
				},
				new MdText(_displayText.Value)
				{
					FontSize = 16,
					Margin = EdgeInsets.Only(bottom: 20)
				},
				new MdRow(new MdButton("Increment Counter",
						() =>
						{
							_counter.Value++;
						}),
					new MdButton("Decrement Counter",
						() =>
						{
							_counter.Value--;
						}))
				{
					Spacing = 10,
					Margin = EdgeInsets.Only(bottom: 20)
				},
				new MdTextField
				{
					Label = "Text Input",
					Value = _textValue.Value,
					Placeholder = "Enter text...",
					OnChanged = value => _textValue.Value = value,
					Margin = EdgeInsets.Only(bottom: 20)
				},
				new MdSwitch(_switchValue.Value)
				{
					Label = "Toggle Switch",
					OnChanged = value => _switchValue.Value = value,
					Margin = EdgeInsets.Only(bottom: 20)
				},
				new MdContainer
				{
					BackgroundColor = "#f0f0f0",
					Padding = EdgeInsets.All(15),
					BorderRadius = BorderRadius.Circular(8),
					Child = new MdColumn(new MdText("Info Box")
						{
							FontWeight = FontWeight.Bold,
							Margin = EdgeInsets.Only(bottom: 10)
						},
						new MdText($"Current Text: {_textValue.Value}"))
				})
			{
				Spacing = 10
			}
		};
	}
}
