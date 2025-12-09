using MacroDeck.SDK.UI.Components;
using MacroDeck.SDK.UI.Components.Input;
using MacroDeck.SDK.UI.Components.Layout;
using MacroDeck.SDK.UI.Core;
using MacroDeck.SDK.UI.Registry;

namespace MacroDeck.Server.Views;

[MdUiView(ViewId = "server.TestCounterView")]
public class TestCounterView : StatefulMdUiView
{
	public override MdUiState CreateState() => new TestCounterState();
}

public class TestCounterState : MdUiState
{
	private State<int> _counter = null!;
	private Computed<string> _counterText = null!;
	private State<string> _message = null!;
	private State<bool> _isLoading = null!;

	public override void InitState()
	{
		_counter = CreateState(0);
		_message = CreateState("Welcome to MacroDeck UI!");
		_counterText = CreateComputed(() => $"Counter: {_counter.Value}", _counter);
		_isLoading = CreateState(false);
	}

	public override MdUiView Build()
	{
		return new MdContainer
		{
			Child = new MdColumn(
				new MdText("MacroDeck UI Framework - Test View")
				{
					FontSize = 28,
					FontWeight = FontWeight.Bold,
					Padding = EdgeInsets.Only(bottom: 4)
				},
				new MdContainer
				{
					Padding = EdgeInsets.Symmetric(3, 0),
					BorderRadius = BorderRadius.Circular(8),
					Child = new MdText(_counterText.Value) { FontSize = 18, FontWeight = FontWeight.Bold }
				},
				new MdContainer
				{
					Padding = EdgeInsets.Symmetric(3, 0),
					BorderRadius = BorderRadius.Circular(8),
					Margin = EdgeInsets.Only(bottom: 20),
					Child = new MdText(_message.Value) { FontSize = 16 }
				},
				new MdRow(
					new MdButton("Increment", () =>
					{
						_counter.Value++;
						_message.Value = $"Counter incremented to {_counter.Value}!";
					}) { Role = ButtonRole.Success, Margin = EdgeInsets.Only(right: 10) },
					new MdButton("Decrement", () =>
					{
						_counter.Value--;
						_message.Value = $"Counter decremented to {_counter.Value}!";
					}) { Role = ButtonRole.Danger, Margin = EdgeInsets.Only(right: 10) },
					new MdButton("Reset", () =>
					{
						_counter.Value = 0;
						_message.Value = "Counter reset!";
					}) { Role = ButtonRole.Warning }
				) { Margin = EdgeInsets.Only(bottom: 20) },
				new MdTextField
				{
					Value = _message.Value,
					Label = "Message",
					Placeholder = "Enter a message...",
					OnChanged = v => _message.Value = v,
					Padding = EdgeInsets.Only(20)
				},
				new MdText("Loading Spinner Demo")
				{
					FontSize = 20,
					FontWeight = FontWeight.Bold,
					Margin = EdgeInsets.Only(top: 30, bottom: 10)
				},
				new MdRow(
					new MdButton(_isLoading.Value ? "Stop" : "Start Loading", () =>
					{
						_isLoading.Value = !_isLoading.Value;
						_message.Value = _isLoading.Value ? "Loading..." : "Stopped";
					}) { Role = ButtonRole.Primary }
				) { Margin = EdgeInsets.Only(bottom: 20) },
				new MdLoading { Visible = _isLoading.Value, Size = LoadingSize.Large }
			) { Spacing = 10 }
		};
	}
}
