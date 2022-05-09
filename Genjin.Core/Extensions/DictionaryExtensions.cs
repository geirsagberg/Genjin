namespace Genjin.Core.Extensions;

public static class DictionaryExtensions {
    public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
        Func<TValue> factory) where TKey : notnull {
        if (!dictionary.TryGetValue(key, out var value)) {
            dictionary[key] = value = factory();
        }

        return value;
    }

    public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
        TValue defaultValue) where TKey : notnull {
        if (!dictionary.TryGetValue(key, out var value)) {
            dictionary[key] = value = defaultValue;
        }

        return value;
    }
}
