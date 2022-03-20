using System.Runtime.CompilerServices;

namespace Genjin.Core;

public static class RangeExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLength(this Range range) => 1 + range.End.Value - range.Start.Value;
}
