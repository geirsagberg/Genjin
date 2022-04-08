using ImGuiNET;

namespace Genjin.Core;

public static class Gui {
    public static IDisposable MainMenuBar() =>
        ImGui.BeginMainMenuBar() ? new Disposable(ImGui.EndMainMenuBar) : new Disposable();

    public static IDisposable Menu(string label, bool enabled = true) =>
        ImGui.BeginMenu(label, enabled) ? new Disposable(ImGui.EndMenu) : new Disposable();

    public static IDisposable Window(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None) =>
        ImGui.Begin(name, flags) ? new Disposable(ImGui.End) : new Disposable();
}
