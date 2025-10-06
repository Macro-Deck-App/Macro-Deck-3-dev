# MacroDeck.SDK.UI

Declarative UI framework for MacroDeck. Define UIs in C#, render in Angular.

## Quick Start

### Stateless View

```csharp
using MacroDeck.SDK.UI.Components;
using MacroDeck.SDK.UI.Components.Layout;
using MacroDeck.SDK.UI.Core;
using MacroDeck.SDK.UI.Registry;

[MdUiView(ViewId = "myplugin.SimpleView")]
public class SimpleView : StatelessMdUiView
{
    public string Title { get; set; } = "Hello";
    
    public override MdUiView Build()
    {
        return new MdContainer
        {
            Padding = EdgeInsets.All(20),
            Child = new MdText(Title)
            {
                FontSize = 24,
                FontWeight = FontWeight.Bold
            }
        };
    }
}
```

### Stateful View

```csharp
[MdUiView(ViewId = "myplugin.CounterView")]
public class CounterView : StatefulMdUiView
{
    public override MdUiState CreateState() => new CounterState();
}

public class CounterState : MdUiState
{
    private State<int> _counter;
    
    public override void InitState()
    {
        _counter = CreateState(0);
    }
    
    public override MdUiView Build()
    {
        return new MdColumn(
            new MdText($"Count: {_counter.Value}")
            {
                FontSize = 20
            },
            new MdButton("Increment", () => _counter.Value++)
        )
        {
            Spacing = 10
        };
    }
}
```

## Available Components

### Layout

- **MdColumn** - Vertical layout
- **MdRow** - Horizontal layout
- **MdStack** - Z-axis stacking
- **MdGrid** - Grid layout
- **MdContainer** - Styled container
- **MdScrollView** - Scrollable container

### Basic

- **MdText** - Text display
- **MdButton** - Clickable button
- **MdImage** - Image display (asset/URL)

### Input

- **MdTextField** - Text input
- **MdCheckbox** - Checkbox
- **MdSwitch** - Toggle switch
- **MdRadio<T>** - Radio button group

## State Management

### Simple State

```csharp
private State<string> _name = CreateState("John");

// Update state
_name.Value = "Jane"; // Automatically triggers rebuild
```

### Computed State

```csharp
private State<string> firstName = CreateState("John");
private State<string> lastName = CreateState("Doe");
private Computed<string> fullName = CreateComputed(
    () => $"{firstName.Value} {lastName.Value}",
    firstName, lastName
);

// fullName automatically updates when firstName or lastName changes
```

## Styling

### Padding & Margin

```csharp
// All sides equal
Padding = EdgeInsets.All(20)

// Symmetric
Padding = EdgeInsets.Symmetric(vertical: 10, horizontal: 20)

// Individual sides
Margin = EdgeInsets.Only(top: 10, bottom: 5, left: 15, right: 15)
```

### Container Styling

```csharp
new MdContainer
{
    BackgroundColor = "#f0f0f0",
    Width = 300,
    Height = 200,
    BorderRadius = BorderRadius.Circular(8),
    Border = new Border(2, "#cccccc"),
    Padding = EdgeInsets.All(15),
    Child = // ...
}
```

## Navigation

```csharp
// Get navigator from DI
var navigator = GetService<MdNavigator>();

// Navigate (clear stack)
navigator.Set("myplugin.SettingsView");

// Push (add to stack)
navigator.Push("myplugin.DetailView");

// Pop (go back)
navigator.Pop();
```

## Assets

### Register Asset

```csharp
// In plugin initialization
AssetRegistry.RegisterAsset(
    "myplugin.icon",
    "MyPlugin.Resources.icon.png",
    "image/png"
);
```

### Use Asset

```csharp
new MdImage.FromAsset("myplugin.icon")
{
    Width = 64,
    Height = 64
}
```

## Events

### Button Click

```csharp
new MdButton("Click Me", () =>
{
    // Handle click
    _counter.Value++;
})
```

### Input Changes

```csharp
new MdTextField
{
    Value = _textValue.Value,
    OnChanged = value =>
    {
        _textValue.Value = value;
    }
}
```

### Switch/Checkbox

```csharp
new MdSwitch(_enabled.Value)
{
    OnChanged = value =>
    {
        _enabled.Value = value;
    }
}
```

## Dependency Injection

Access services from views:

```csharp
public override MdUiView Build()
{
    var myService = GetService<IMyService>();
    var data = myService.GetData();
    
    return new MdText(data);
}
```

## Plugin Integration

```csharp
// In Program.cs
await PluginSdk.CreatePluginBuilder(pluginOptions)
    .AddPluginConfiguration<MyConfigView>()
    .Build()
    .Run(args);
```

## Best Practices

1. **Use Stateless when possible** - Less overhead, simpler logic
2. **Keep Build() pure** - No side effects, only UI construction
3. **Use Computed for derived values** - Automatic dependency tracking
4. **Batch state updates** - Multiple changes = one rebuild
5. **Use meaningful ViewIds** - Format: `{pluginId}.{ViewName}`

## Architecture

```
View Definition (C#)
    ↓
ViewTree Serialization
    ↓
SignalR Transport
    ↓
Angular Rendering
    ↓
User Interaction
    ↓
Event → Backend
    ↓
State Update → Rebuild
```