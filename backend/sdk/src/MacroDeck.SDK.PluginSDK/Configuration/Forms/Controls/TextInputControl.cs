using MacroDeck.SDK.PluginSDK.Configuration.Enums;

namespace MacroDeck.SDK.PluginSDK.Configuration.Forms.Controls;

public abstract class TextInputControl : ISimpleFormControl
{
	public abstract string Label { get; }

	public abstract TextInputType Type { get; }

	public virtual string? Placeholder => null;
}
