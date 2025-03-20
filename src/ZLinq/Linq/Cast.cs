using System.Security;

namespace ZLinq
{
    // Cast has been moved to instance method

    //    partial class ValueEnumerableExtensions
    //    {
    //        public static ValueEnumerable<Cast<TEnumerator, TSource, TResult>, TResult> Cast<TEnumerator, TSource, TResult>(this ValueEnumerable<TEnumerator, TSource> source, TResult typeHint)
    //            where TEnumerator : struct, IValueEnumerator<TSource>
    //#if NET9_0_OR_GREATER
    //            , allows ref struct
    //#endif
    //            => new(new(source.Enumerator));
    //    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct Cast<TEnumerator, TSource, TResult>(TEnumerator source)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            return source.TryGetNonEnumeratedCount(out count);
        }

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TResult> dest) => false;

        public bool TryGetNext(out TResult current)
        {
            while (source.TryGetNext(out var value))
            {
                current = (TResult)(object)value!;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}
