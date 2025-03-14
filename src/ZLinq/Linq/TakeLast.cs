namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TakeLast<TEnumerable, TSource> TakeLast<TEnumerable, TSource>(this TEnumerable source, Int32 count)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, count);

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
    struct TakeLast<TEnumerable, TSource>(TEnumerable source, Int32 count)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        readonly int takeCount = Math.Max(0, count);
        Queue<TSource>? q;

        public ValueEnumerator<TakeLast<TEnumerable, TSource>, TSource> GetEnumerator() => new(this);

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

        public bool TryCopyTo(Span<TSource> destination)
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
