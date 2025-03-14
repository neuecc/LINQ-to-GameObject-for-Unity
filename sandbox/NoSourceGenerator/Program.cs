




using ZLinq;
using ZLinq.Linq;


Span<int> xs = [1, 2, 3, 4, 5];


var seq = xs.AsValueEnumerable().OrderBy(x => x);

internal static partial class ZLinqTypeInferenceHelper
{
    /* 2025/03/06 3:44:14 */
    public static OrderBy<FromSpan<int>, int, TKey> OrderBy<TKey>(this FromSpan<int> source, Func<int, TKey> keySelector) => ValueEnumerableExtensions.OrderBy<FromSpan<int>, int, TKey>(source, keySelector);
    /* 2025/03/06 3:44:14 */
    public static OrderBy<FromSpan<int>, int, TKey> OrderBy<TKey>(this FromSpan<int> source, Func<int, TKey> keySelector, global::System.Collections.Generic.IComparer<TKey> comparer) => ValueEnumerableExtensions.OrderBy<FromSpan<int>, int, TKey>(source, keySelector, comparer);
}