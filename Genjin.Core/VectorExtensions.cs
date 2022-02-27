using System.Numerics;
using System.Runtime.CompilerServices;

namespace Genjin.Core;

public static class VectorExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 NormalizeOrZero(this Vector2 vector2) =>
        vector2 == Vector2.Zero ? vector2 : Vector2.Normalize(vector2);
}