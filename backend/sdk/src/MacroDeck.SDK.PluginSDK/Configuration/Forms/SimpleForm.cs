using MacroDeck.SDK.PluginSDK.Configuration.Forms.Controls;

namespace MacroDeck.SDK.PluginSDK.Configuration.Forms;

public abstract class SimpleForm
{
	public abstract List<ISimpleFormControl> GetControls();

	public abstract Task OnResult(object? result);
}
