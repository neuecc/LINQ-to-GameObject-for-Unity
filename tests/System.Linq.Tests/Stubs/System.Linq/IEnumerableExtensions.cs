using System.Diagnostics;

namespace System.Linq;

#if NET10_0_OR_GREATER

#else
public static partial class IEnumerableExtensions
{
    public static IEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> source)
    {
        throw new NotImplementedException("Requires .NET 10");
    }

    public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector) =>
            LeftJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer: null);

    public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
    {
        throw new NotImplementedException("Requires .NET 10");
    }

    public static IEnumerable<TResult> RightJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector) =>
            RightJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer: null);

    public static IEnumerable<TResult> RightJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
    {
        throw new NotImplementedException("Requires .NET 10");
    }
}
#endif
