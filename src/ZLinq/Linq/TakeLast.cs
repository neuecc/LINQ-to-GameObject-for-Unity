namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<TakeLast<TEnumerator, TSource>, TSource> TakeLast<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Int32 count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, count));

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
    struct TakeLast<TEnumerator, TSource>(TEnumerator source, Int32 count)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        readonly int takeCount = Math.Max(0, count);
        Queue<TSource>? q;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                count = Math.Min(count, takeCount);
                return true;
            }

            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            if (source.TryGetSpan(out span))
            {
                if (span.Length > takeCount)
                {
                    span = span[^takeCount..];
                }
                return true;
            }
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination, Index offset) // TODO: impl
        {
            if (TryGetSpan(out var span) && span.Length <= destination.Length)
            {
                span.CopyTo(destination);
                return true;
            }
            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (takeCount == 0)
            {
                Unsafe.SkipInit(out current);
                return false;
            }

            if (q == null)
            {
                q = new Queue<TSource>();
            }

        DEQUEUE:
            if (q.Count != 0)
            {
                current = q.Dequeue();
                return true;
            }

            while (source.TryGetNext(out current))
            {
                if (q.Count == takeCount)
                {
                    q.Dequeue();
                }
                q.Enqueue(current);
            }

            if (q.Count != 0) goto DEQUEUE;

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}
