namespace MacroDeck.SDK.PluginSDK.Actions;

public abstract class SimpleMacroDeckAction : MacroDeckAction
{
	public virtual Task OnInvoke()
	{
		return Task.CompletedTask;
	}

	public virtual Task OnInvoke<T>(T configuration)
		where T : class
	{
		return Task.CompletedTask;
	}
}
