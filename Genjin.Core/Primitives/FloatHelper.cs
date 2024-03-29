﻿using System.Runtime.CompilerServices;

namespace Genjin.Core.Primitives; 

public static class FloatHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref float value1, ref float value2)
    {
        (value1, value2) = (value2, value1);
    }
}
