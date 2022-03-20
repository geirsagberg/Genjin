using System.Runtime.CompilerServices;

namespace Genjin.Core;

public static class MathHelpers {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Wrap(float position, float max)
        => position < 0 ? position + max : position >= max ? position - max : position;
}
