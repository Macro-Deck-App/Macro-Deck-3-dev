using MacroDeck.SDK.PluginSDK.Actions;

namespace ExamplePlugin.Actions;

public class MyCustomAction : SimpleMacroDeckAction
{
	public override string Name => "My Custom Action";

	public override string Id => "app.macrodeck.exampleplugin.mycustomaction";

	public override Task OnInvoke()
	{
		Console.WriteLine("Hello World!");

		return Task.CompletedTask;
	}
}
