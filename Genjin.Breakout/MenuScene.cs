using System.Numerics;
using Genjin.Core;
using Genjin.Example;
using ImGuiNET;

namespace Genjin.Breakout;

internal class MenuScene : IScene {
    private readonly MessageHub hub;

    public MenuScene(MessageHub hub) {
        this.hub = hub;
    }

    public void Draw() {
        ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2, ImGuiCond.Always, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(new Vector2(400, 200));
        using (Gui.Window("",
                   ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar |
                   ImGuiWindowFlags.NoMove)) {
            var size = ImGui.GetWindowSize();
            const int buttonHeight = 30;
            ImGui.SetCursorPos(new Vector2(size.X / 4, (size.Y / 2) - buttonHeight));
            if (ImGui.Button("Start game", size with { X = size.X / 2, Y = buttonHeight })) {
                Console.WriteLine("Start game");
            }

            ImGui.SetCursorPos(new Vector2(size.X / 4, ImGui.GetCursorPosY()));
            if (ImGui.Button("Exit", size with { X = size.X / 2, Y = buttonHeight })) {
                hub.Publish(new StopMessage());
            }
        }
    }
}
