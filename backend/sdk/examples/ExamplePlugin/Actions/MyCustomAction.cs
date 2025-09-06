using MacroDeck.SDK.PluginSDK.Actions;

namespace ExamplePlugin.Actions;

public class MyCustomAction : SimpleMacroDeckAction
{
	public override string Name => "My Custom Action";

	public override string Id => "com.example.plugin.mycustomaction";

	public override Task OnInvoke()
	{
		Console.WriteLine("Hello World!");

		return Task.CompletedTask;
	}
}
