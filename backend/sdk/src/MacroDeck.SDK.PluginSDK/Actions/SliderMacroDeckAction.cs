namespace MacroDeck.SDK.PluginSDK.Actions;

public abstract class SliderMacroDeckAction : MacroDeckAction
{
	public abstract double StepSize { get; set; }

	public virtual Task OnSlide(int value)
	{
		return Task.CompletedTask;
	}

	public virtual Task OnSlide(float value)
	{
		return Task.CompletedTask;
	}

	public virtual Task OnSlide(double value)
	{
		return Task.CompletedTask;
	}

	public virtual Task OnSlide(long value)
	{
		return Task.CompletedTask;
	}
}
