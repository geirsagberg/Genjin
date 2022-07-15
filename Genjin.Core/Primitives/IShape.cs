using System.Numerics;

namespace Genjin.Core.Primitives;

public interface IShape {
    public Vector2 Position { get; set; }
    
    public Size Size { get; }
    public Box Bounds { get; }
    public Vector2 Center { get; }
}
