using System.Runtime.CompilerServices;

namespace Genjin.Core.Extensions; 

public static class NumberExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToFloat(this double d) => (float) d;
}
