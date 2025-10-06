namespace MacroDeck.SDK.UI.Core;

/// <summary>
///     Represents a computed state value that depends on other states
/// </summary>
public class Computed<T> : IState
{
	private readonly Func<T> _compute;
	private T _cached;
	private bool _dirty;

	public Computed(Func<T> compute, params IState[] dependencies)
	{
		_compute = compute;

		foreach (var dep in dependencies)
		{
			dep.Changed += () =>
			{
				_dirty = true;
				Changed?.Invoke();
			};
		}

		_cached = compute();
		_dirty = false;
	}

	public T Value
	{
		get
		{
			if (_dirty)
			{
				_cached = _compute();
				_dirty = false;
			}

			return _cached;
		}
	}

	public event Action? Changed;

	public object? GetValue()
	{
		return Value;
	}
}
