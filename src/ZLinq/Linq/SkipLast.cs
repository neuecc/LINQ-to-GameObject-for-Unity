namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<SkipLast<TEnumerator, TSource>, TSource> SkipLast<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Int32 count)
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
    struct SkipLast<TEnumerator, TSource>(in TEnumerator source, Int32 count)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        readonly int skipCount = Math.Max(0, count);
        ValueQueue<TSource> buffer;
        bool isBufferInitialized;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                count = Math.Max(0, count - skipCount);
                return true;
            }

            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            if (source.TryGetSpan(out span))
            {
                if (span.Length <= skipCount)
                {
                    span = default;
                    return true;
                }
                span = span[..^skipCount]; // Slice from start to (length - skipCount)
                return true;
            }

            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination) => false;

        public bool TryGetNext(out TSource current)
        {
            if (skipCount == 0)
            {
                return source.TryGetNext(out current);
            }

            if (!isBufferInitialized)
            {
                isBufferInitialized = true;
                buffer = new ValueQueue<TSource>(4);

                // Fill the buffer with initial items
                while (buffer.Count < skipCount && source.TryGetNext(out var item))
                {
                    buffer.Enqueue(item);
                }

                // If we couldn't fill the buffer, there aren't enough items
                if (buffer.Count < skipCount)
                {
                    Unsafe.SkipInit(out current);
                    return false;
                }
            }

            // Try to get the next item from the source
            if (source.TryGetNext(out var next))
            {
                // Return the oldest item from buffer and add the new one
                current = buffer.Dequeue();
                buffer.Enqueue(next);
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (isBufferInitialized)
            {
                buffer.Dispose();
            }
            source.Dispose();
        }
    }
}
