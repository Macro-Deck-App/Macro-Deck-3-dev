using System.Collections.Concurrent;
using MacroDeck.SDK.UI.Core;
using MacroDeck.SDK.UI.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace MacroDeck.Server.Application.UI.Registry;

/// <summary>
/// Implementation of the UI registry for the server
/// </summary>
public class MdUiRegistry : IMdUiRegistry
{
    private readonly ConcurrentDictionary<string, MdUiViewMetadata> _views = new();
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Event fired when a view is registered
    /// </summary>
    public event Action<MdUiViewMetadata>? ViewRegistered;

    /// <summary>
    /// Event fired when a view is unregistered
    /// </summary>
    public event Action<string>? ViewUnregistered;

    public MdUiRegistry(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void RegisterView(MdUiViewMetadata metadata)
    {
        if (string.IsNullOrWhiteSpace(metadata.ViewId))
        {
            throw new ArgumentException("ViewId cannot be null or empty", nameof(metadata));
        }

        _views[metadata.ViewId] = metadata;
        ViewRegistered?.Invoke(metadata);
    }

    public void UnregisterView(string viewId)
    {
        if (_views.TryRemove(viewId, out _))
        {
            ViewUnregistered?.Invoke(viewId);
        }
    }

    public void UnregisterPluginViews(string pluginId)
    {
        var pluginViews = _views.Values.Where(v => v.PluginId == pluginId).ToList();
        foreach (var view in pluginViews)
        {
            if (_views.TryRemove(view.ViewId, out _))
            {
                ViewUnregistered?.Invoke(view.ViewId);
            }
        }
    }

    public MdUiViewMetadata? GetViewMetadata(string viewId)
    {
        _views.TryGetValue(viewId, out var metadata);
        return metadata;
    }

    public IEnumerable<MdUiViewMetadata> GetAllViews()
    {
        return _views.Values.ToList();
    }

    public IEnumerable<MdUiViewMetadata> GetAllRegisteredViews()
    {
        return _views.Values.ToList();
    }

    public MdUiView CreateViewInstance(string viewId)
    {
        var metadata = GetViewMetadata(viewId);
        if (metadata == null)
        {
            throw new InvalidOperationException($"View '{viewId}' not found in registry");
        }

        // Create instance using DI
        var view = ActivatorUtilities.CreateInstance(_serviceProvider, metadata.ViewType) as MdUiView;
        if (view == null)
        {
            throw new InvalidOperationException($"Failed to create instance of view type '{metadata.ViewType.FullName}'");
        }

        view.ViewId = viewId;
        return view;
    }
}
