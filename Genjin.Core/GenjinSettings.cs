namespace Genjin.Core;

public enum DisplayMode {
    Windowed,
    Fullscreen,
    Borderless
}

public record GenjinSettings {
    public int Width { get; init; }
    public int Height { get; init; }
    public DisplayMode Display { get; init; }
    public bool VSync { get; init; }
}
