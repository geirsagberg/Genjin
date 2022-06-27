using Genjin.Core;
using Veldrid;

namespace Genjin.Breakout;

public class BreakoutKeyMap : IKeyMap<GameKeys> {
    public GameKeys? this[Key key] =>
        key switch {
            Key.Left => GameKeys.Left,
            Key.A => GameKeys.Left,
            Key.Right => GameKeys.Right,
            Key.D => GameKeys.Right,
            _ => null
        };
}
