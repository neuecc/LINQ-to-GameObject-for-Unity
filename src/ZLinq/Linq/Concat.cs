namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        // currently source-generator only infer first argument type so can not define `TEnumerable source, TEnumerable2 second`.

        public static Concat<TEnumerable, TSource> Concat<TEnumerable, TSource>(this TEnumerable source, IEnumerable<TSource> second)
            where TEnumerable : struct, IValueEnumerable<TSource>
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
    struct Concat<TEnumerable, TSource>(TEnumerable source, IEnumerable<TSource> second)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        IEnumerator<TSource>? secondEnumerator;
        bool firstCompleted;

        public ValueEnumerator<Concat<TEnumerable, TSource>, TSource> GetEnumerator() => new(this);

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
