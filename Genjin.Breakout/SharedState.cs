using System.Drawing;

namespace Genjin.Breakout.Systems;

public class SharedState {
    public GameState GameState { get; set; }
    public SizeF GameSize { get; set; }
}
