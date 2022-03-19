using System.Numerics;
using System.Runtime.CompilerServices;

namespace Genjin.Core;

public static class VectorExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 NormalizeOrZero(this Vector2 vector2) =>
        vector2 == Vector2.Zero ? vector2 : Vector2.Normalize(vector2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Round(this Vector2 vector) => new((int)vector.X, (int)vector.Y);
}