




using ZLinq;
using ZLinq.Linq;


(string, int)[] xs = [("foo", 100), ("bar", 200), ("baz", 300)];

xs.AsValueEnumerable().ToDictionary(x => x.Item1, StringComparer.OrdinalIgnoreCase);

internal static partial class ZLinqTypeInferenceHelper
{
    //public static global::System.Collections.Generic.Dictionary<int, TValue> ToDictionary<TValue>(this FromSpan<int> source)
    //    where TValue : notnull
    //{
    //    return ValueEnumerableExtensions.ToDictionary<FromSpan<KeyValuePair<int, TValue>>, int, TValue>(source);
    //}
    //public static global::System.Collections.Generic.Dictionary<int, TValue> ToDictionary<TValue>(this FromSpan<int> source, global::System.Collections.Generic.IEqualityComparer<int> comparer) => ValueEnumerableExtensions.ToDictionary<FromSpan<int>, int, TValue>(source, comparer);
    //ublic static global::System.Collections.Generic.Dictionary<TKey, int> ToDictionary<TKey>(this FromSpan<int> source, Func<int, TKey> keySelector) => ValueEnumerableExtensions.ToDictionary<FromSpan<int>, int, TKey>(source, keySelector);
    //public static global::System.Collections.Generic.Dictionary<TKey, int> ToDictionary<TKey>(this FromSpan<int> source, Func<int, TKey> keySelector, global::System.Collections.Generic.IEqualityComparer<TKey> comparer) => ValueEnumerableExtensions.ToDictionary<FromSpan<int>, int, TKey>(source, keySelector, comparer);
    //public static global::System.Collections.Generic.Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(this FromSpan<int> source, Func<int, TKey> keySelector, Func<int, TElement> elementSelector) => ValueEnumerableExtensions.ToDictionary<FromSpan<int>, int, TKey, TElement>(source, keySelector, elementSelector);

    //public static global::System.Collections.Generic.Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(this FromSpan<int> source, Func<int, TKey> keySelector, Func<int, TElement> elementSelector, global::System.Collections.Generic.IEqualityComparer<TKey> comparer) => ValueEnumerableExtensions.ToDictionary<FromSpan<int>, int, TKey, TElement>(source, keySelector, elementSelector, comparer);


    //public static global::System.Collections.Generic.Dictionary<TKey, (string, int)> ToDictionary<TKey>(this FromArray<(string, int)> source, Func<(string, int), TKey> keySelector)
    //{
    //    return ValueEnumerableExtensions.ToDictionary<FromArray<(string, int)>, (string, int), TKey>(source, keySelector);
    //}

    public static global::System.Collections.Generic.Dictionary<TKey, (string, int)> ToDictionary<TKey>(this FromArray<(string, int)> source, Func<(string, int), TKey> keySelector, global::System.Collections.Generic.IEqualityComparer<TKey>? comparer = default)
    {
        return ValueEnumerableExtensions.ToDictionary<FromArray<(string, int)>, (string, int), TKey>(source, keySelector, comparer);
    }
}
