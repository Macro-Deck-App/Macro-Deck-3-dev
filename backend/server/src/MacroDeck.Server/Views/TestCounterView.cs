using MacroDeck.SDK.UI.Components;
using MacroDeck.SDK.UI.Components.Input;
using MacroDeck.SDK.UI.Components.Layout;
using MacroDeck.SDK.UI.Core;
using MacroDeck.SDK.UI.Registry;
using Serilog;

namespace MacroDeck.Server.Views;

[MdUiView(ViewId = "server.TestCounterView")]
public class TestCounterView : StatefulMdUiView
{
	public override MdUiState CreateState()
	{
		return new TestCounterState();
	}
}

public class TestCounterState : MdUiState
{
	private State<int> _counter = null!;
	private Computed<string> _displayText = null!;
	private State<bool> _isLoading = null!;
	private State<string> _message = null!;

	public override void InitState()
	{
		_counter = CreateState(0);
		_message = CreateState("Welcome to MacroDeck UI!");
		_displayText = CreateComputed(() => $"Counter: {_counter.Value}", _counter);
		_isLoading = CreateState(false);
	}

	public override MdUiView Build()
	{
		return new MdContainer
		{
			Child = new MdColumn(new MdText("MacroDeck UI Framework - Test View")
				{
					FontSize = 28,
					FontWeight = FontWeight.Bold,
					Padding = EdgeInsets.Only(bottom: 4)
				},
				new MdContainer
				{
					Padding = EdgeInsets.Symmetric(horizontal: 3, vertical: 0),
					BorderRadius = BorderRadius.Circular(8),
					Child = new MdText(_displayText.Value)
					{
						FontSize = 18,
						FontWeight = FontWeight.Bold
					}
				},
				new MdContainer
				{
					Padding = EdgeInsets.Symmetric(horizontal: 3, vertical: 0),
					BorderRadius = BorderRadius.Circular(8),
					Margin = EdgeInsets.Only(bottom: 20),
					Child = new MdText(_message.Value)
					{
						FontSize = 16,
						Margin = EdgeInsets.Only(bottom: 20)
					}
				},
				new MdRow(new MdButton("Increment",
						() =>
						{
							_counter.Value++;
							_message.Value = $"Counter incremented to {_counter.Value}!";
							Log.Logger.Information("Increment button clicked, value {Value}", _counter.Value);
						})
					{
						Role = ButtonRole.Success,
						Margin = EdgeInsets.Only(right: 10)
					},
					new MdButton("Decrement",
						() =>
						{
							_counter.Value--;
							_message.Value = $"Counter decremented to {_counter.Value}!";
							Log.Logger.Information("Decrement button clicked, value {Value}", _counter.Value);
						})
					{
						Role = ButtonRole.Danger,
						Margin = EdgeInsets.Only(right: 10)
					},
					new MdButton("Reset",
						() =>
						{
							_counter.Value = 0;
							_message.Value = "Counter reset!";
						})
					{
						Role = ButtonRole.Warning
					})
				{
					Margin = EdgeInsets.Only(bottom: 20)
				},
				new MdTextField
				{
					Padding = EdgeInsets.Only(20),
					Value = _message.Value,
					Label = "Message",
					Placeholder = "Enter a message...",
					OnChanged = value => _message.Value = value
				},
				new MdText("Loading Spinner Demo")
				{
					FontSize = 20,
					FontWeight = FontWeight.Bold,
					Margin = EdgeInsets.Only(30, bottom: 10)
				},
				new MdRow(new MdButton(_isLoading.Value ? "Stop Loading" : "Start Loading",
					() =>
					{
						Log.Logger.Information("Button clicked, isLoading before: {IsLoading}", _isLoading.Value);
						_isLoading.Value = !_isLoading.Value;
						Log.Logger.Information("Button clicked, isLoading after: {IsLoading}", _isLoading.Value);
						_message.Value = _isLoading.Value ? "Loading started..." : "Loading stopped!";
					})
				{
					Role = ButtonRole.Primary,
					Margin = EdgeInsets.Only(right: 10)
				})
				{
					Margin = EdgeInsets.Only(bottom: 20)
				},
				new MdLoading
				{
					Visible = _isLoading.Value,
					Size = LoadingSize.Large
				})

			{
				Spacing = 10
			}
		};
	}
}
