namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        // currently source-generator only infer first argument type so can not define `TEnumerator source, TEnumerator2 second`.

        public static Concat<TEnumerator, TSource> Concat<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TSource> second)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, second);
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
    struct Concat<TEnumerator, TSource>(TEnumerator source, IEnumerable<TSource> second)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        IEnumerator<TSource>? secondEnumerator;
        bool firstCompleted;

        public ValueEnumerator<Concat<TEnumerator, TSource>, TSource> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out var count1) && second.TryGetNonEnumeratedCount(out var count2))
            {
                count = count1 + count2;
                return true;
            }
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> dest)
        {
            // return false because source1 succeeded but source2 failed, it is high-cost operation
            // Also, if source1 succeeds and source2 fails, there is a possibility that source1's TryCopyTo completes and TryGetNext stops working.
            // ZLinq's semantics do not guarantee the operation of other operations if TryCopyTo succeeds.
            // Therefore, it must be set to false.
            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (!firstCompleted)
            {
                // iterate first
                if (source.TryGetNext(out current))
                {
                    return true;
                }
                source.Dispose();
                firstCompleted = true;
            }

            // iterate second
            if (secondEnumerator == null)
            {
                secondEnumerator = second.GetEnumerator();
            }

            if (secondEnumerator.MoveNext())
            {
                current = secondEnumerator.Current;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (secondEnumerator != null)
            {
                secondEnumerator.Dispose();
            }
            source.Dispose();
        }
    }
}
