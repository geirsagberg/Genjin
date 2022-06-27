using Veldrid;

namespace Genjin.Core;

public class InputState<T> where T : struct, Enum {
    private readonly IKeyMap<T> keyMap;
    private readonly HashSet<T> pressed = new();
    public IReadOnlySet<T> Pressed => pressed;

    public bool IsKeyDown(T key) => pressed.Contains(key);

    public InputState(IKeyMap<T> keyMap) {
        this.keyMap = keyMap;
    }

    public void Update(InputSnapshot input) {
        foreach (var inputKeyEvent in input.KeyEvents) {
            var gameKey = keyMap[inputKeyEvent.Key];
            if (gameKey != null) {
                if (inputKeyEvent.Down) {
                    pressed.Add(gameKey.Value);
                } else {
                    pressed.Remove(gameKey.Value);
                }
            }
        }
    }
}
