namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<SkipWhile<TEnumerator, TSource>, TSource> SkipWhile<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, predicate));

        public static ValueEnumerable<SkipWhile2<TEnumerator, TSource>, TSource> SkipWhile<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Int32, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, predicate));

    }
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
    struct SkipWhile<TEnumerator, TSource>(TEnumerator source, Func<TSource, Boolean> predicate)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        bool skippingDone = false;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination, int offset) => false;

        public bool TryGetNext(out TSource current)
        {
            // If we've already found an element that doesn't match the predicate,
            // we can just return elements directly from the source
            if (skippingDone)
            {
                return source.TryGetNext(out current);
            }

            // Skip all elements that match the predicate
            while (source.TryGetNext(out current))
            {
                if (!predicate(current))
                {
                    skippingDone = true;
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct SkipWhile2<TEnumerator, TSource>(TEnumerator source, Func<TSource, Int32, Boolean> predicate)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        bool skippingDone = false;
        int index = 0;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination, int offset) => false;

        public bool TryGetNext(out TSource current)
        {
            // If we've already found an element that doesn't match the predicate,
            // we can just return elements directly from the source
            if (skippingDone)
            {
                if (source.TryGetNext(out current))
                {
                    index++;
                    return true;
                }

                Unsafe.SkipInit(out current);
                return false;
            }

            // Skip all elements that match the predicate
            while (source.TryGetNext(out current))
            {
                if (!predicate(current, index))
                {
                    skippingDone = true;
                    index++;
                    return true;
                }
                index++;
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
