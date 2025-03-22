using System.Diagnostics.CodeAnalysis;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<Shuffle<TEnumerator, TSource>, TSource> Shuffle<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator));
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
    struct Shuffle<TEnumerator, TSource>(TEnumerator source)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        TSource[]? buffer;
        int index = 0;

        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            InitBuffer();
            span = buffer;
            return true;
        }

        public bool TryCopyTo(Span<TSource> destination, Index offset)
        {
            // in-place shuffle needs full src buffer(no offset)
            if (source.TryGetNonEnumeratedCount(out var count) && offset.GetOffset(count) == 0)
            {
                // destination must be larger than source
                if (destination.Length >= count)
                {
                    // and ok to copy then shuffle.
                    if (source.TryCopyTo(destination, 0))
                    {
                        RandomShared.Shuffle(destination.Slice(0, count));
                        return true;
                    }
                }
            }

            InitBuffer();
            if (IterateHelper.TryGetSlice<TSource>(buffer, offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }

            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            InitBuffer();

            if (index < buffer.Length)
            {
                current = buffer[index++];
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }

        [MemberNotNull(nameof(buffer))]
        void InitBuffer()
        {
            if (buffer == null)
            {
                // do not use pool(struct field can't gurantees state of reference)
                // TODO: in-future use SafeBox
                buffer = new ValueEnumerable<TEnumerator, TSource>(source).ToArray<TEnumerator, TSource>();
                RandomShared.Shuffle(buffer);
            }
        }
    }
}
