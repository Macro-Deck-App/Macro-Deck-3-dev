using ExamplePlugin.Actions;
using ExamplePlugin.Services;
using MacroDeck.SDK.PluginSDK;
using MacroDeck.SDK.PluginSDK.Options;
using Microsoft.Extensions.DependencyInjection;

namespace ExamplePlugin;

public static class Program
{
	public static async Task Main(string[] args)
	{
		var pluginOptions = new PluginOptions
		{
			PluginId = "app.macrodeck.exampleplugin",
			PluginName = "Example Plugin",
			PluginWebsite = "https://macro-deck.app",
			Author = "Macro Deck",
			AuthorWebsite = "https://macro-deck.app",
			Version = new PluginVersion(1, 0, 0)
		};

		await PluginSdk.CreatePluginBuilder(pluginOptions)
			.ConfigureServices(services =>
			{
				services.AddScoped<SomeHttpClient>();
			})
			.RegisterActions(actions =>
			{
				actions.Add<MyCustomAction>();
			})
			.Build()
			.Run(args);
	}
}
