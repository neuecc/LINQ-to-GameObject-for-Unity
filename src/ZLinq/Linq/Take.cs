using System;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Take<TEnumerable, TSource> Take<TEnumerable, TSource>(this TEnumerable source, Int32 count)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, count);

        public static TakeRange<TEnumerable, TSource> Take<TEnumerable, TSource>(this TEnumerable source, Range range)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, range);
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
    struct Take<TEnumerable, TSource>(TEnumerable source, Int32 count)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        readonly int takeCount = (count < 0) ? 0 : count; // allows negative count
        int index;

        public ValueEnumerator<Take<TEnumerable, TSource>, TSource> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                count = Math.Min(count, takeCount); // take smaller
                return true;
            }

            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            if (source.TryGetSpan(out span))
            {
                span = span.Slice(0, Math.Min(span.Length, takeCount));
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
            if (index++ < takeCount && source.TryGetNext(out current))
            {
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

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct TakeRange<TEnumerable, TSource>
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source;
        readonly Range range;

        int index;
        int remains;
        readonly int skipIndex;
        readonly int fromEndQueueCount; // 0 is not use q
        Queue<TSource>? q;

        public TakeRange(TEnumerable source, Range range)
        {
            // initialize before run.
            this.source = source;
            this.range = range;
            this.fromEndQueueCount = 0;
            this.remains = -1; // unknown

            if (source.TryGetNonEnumeratedCount(out var count))
            {
                var start = Math.Max(0, range.Start.GetOffset(count));
                var end = Math.Min(count, range.End.GetOffset(count));

                this.skipIndex = start;
                this.remains = end - start;
                if (remains < 0)
                {
                    this.remains = 0; // isEmpty
                }
            }
            else
            {
                if (!range.Start.IsFromEnd && !range.End.IsFromEnd) // both fromstart
                {
                    this.skipIndex = range.Start.Value;
                    this.remains = range.End.Value - range.Start.Value;
                    if (remains < 0)
                    {
                        this.remains = 0; // isEmpty
                    }
                }
                else if (!range.Start.IsFromEnd && range.End.IsFromEnd) // end-fromend
                {
                    // unknown remains
                    this.skipIndex = range.Start.Value;
                    if (range.End.Value == 0)
                    {
                        this.remains = int.MaxValue; // get all
                        return;
                    }

                    this.fromEndQueueCount = int.MaxValue; // unknown queue count
                    this.q = new();
                }
                else if (range.Start.IsFromEnd && !range.End.IsFromEnd) // start-fromend
                {
                    // unknown skipIndex and remains
                    this.skipIndex = 0;
                    this.fromEndQueueCount = range.Start.Value; //queue size is fixed from end-of-start
                    if (this.fromEndQueueCount == 0) fromEndQueueCount = 1;
                    this.q = new();
                }
                else if (range.Start.IsFromEnd && range.End.IsFromEnd) // both fromend
                {
                    // unknown skipIndex and remains but maxCount can calc
                    this.skipIndex = 0;
                    var maxCount = range.Start.Value - range.End.Value; // maxCount but remains is unknown.
                    if (maxCount <= 0)
                    {
                        // empty
                        this.remains = 0;
                        return;
                    }
                    this.fromEndQueueCount = range.Start.Value;
                    if (this.fromEndQueueCount == 0) fromEndQueueCount = 1;
                    this.q = new();
                }
            }
        }

        public ValueEnumerator<TakeRange<TEnumerable, TSource>, TSource> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out _))
            {
                count = remains;
                return true;
            }
            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            if (source.TryGetSpan(out span))
            {
                span = span.Slice(skipIndex, remains);
                return true;
            }

            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination)
        {
            if (TryGetSpan(out var span) && span.Length <= destination.Length) // get self Span
            {
                span.CopyTo(destination);
                return true;
            }

            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (remains == 0)
            {
                goto END;
            }

        DEQUEUE:
            if (q != null && q.Count != 0)
            {
                if (remains == -1)
                {
                    // calculate remains
                    var count = index;
                    var start = Math.Max(0, range.Start.GetOffset(count));
                    var end = Math.Min(count, range.End.GetOffset(count));

                    this.remains = end - start;
                    if (remains < 0)
                    {
                        goto END;
                    }

                    // q.Count is fromEnd
                    var offset = count - q.Count;
                    var skipIndex = Math.Max(0, start - offset);
                    while (skipIndex > 0)
                    {
                        q.Dequeue();
                        skipIndex--;
                    }
                }

                if (remains-- > 0)
                {
                    current = q.Dequeue();
                    return true;
                }
                else
                {
                    goto END;
                }
            }

            while (source.TryGetNext(out current))
            {
                if (q == null)
                {
                    if (index++ < skipIndex)
                    {
                        continue; // skip
                    }

                    // take
                    if (remains > 0)
                    {
                        remains--;
                        return true;
                    }
                    return false;
                }
                else
                {
                    // from-last
                    if (index++ < skipIndex)
                    {
                        continue;
                    }

                    if (q.Count == fromEndQueueCount)
                    {
                        q.Dequeue();
                    }
                    q.Enqueue(current);
                }
            }

            if (q != null && q.Count != 0)
            {
                goto DEQUEUE;
            }

        END:
            this.remains = 0;
            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}
