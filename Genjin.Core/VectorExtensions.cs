using System.Numerics;
using System.Runtime.CompilerServices;

namespace Genjin.Core;

public static class VectorExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 NormalizedOrZero(this Vector2 vector2) =>
        vector2 == Vector2.Zero ? vector2 : Vector2.Normalize(vector2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Round(this Vector2 vector) => new((int)vector.X, (int)vector.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Wrap(this Vector2 position, Vector2 size)
        => new(MathHelpers.Wrap(position.X, size.X), MathHelpers.Wrap(position.Y, size.Y));
}
