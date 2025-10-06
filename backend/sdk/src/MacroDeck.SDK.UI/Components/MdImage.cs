using MacroDeck.SDK.UI.Core;

namespace MacroDeck.SDK.UI.Components;

/// <summary>
/// Displays an image
/// </summary>
public class MdImage : StatelessMdUiView
{
    public string? AssetId { get; set; }
    public string? Url { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }
    public ImageFit? Fit { get; set; }
    
    private MdImage() { }
    
    public static MdImage FromAsset(string assetId)
    {
        return new MdImage { AssetId = assetId };
    }
    
    public static MdImage FromUrl(string url)
    {
        return new MdImage { Url = url };
    }
    
    public override MdUiView Build()
    {
        return this;
    }
}

public enum ImageFit
{
    Fill,
    Contain,
    Cover,
    None,
    ScaleDown
}
