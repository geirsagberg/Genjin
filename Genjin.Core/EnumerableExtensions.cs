namespace Genjin.Core;

public static class EnumerableExtensions {
    public static string ToJoinedString<T>(this IEnumerable<T> enumerable, string separator = ", ") =>
        string.Join(separator, enumerable);
}
