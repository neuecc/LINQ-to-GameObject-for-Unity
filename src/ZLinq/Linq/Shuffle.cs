using System.Buffers;
using System.Drawing;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Shuffle<TEnumerable, TSource> Shuffle<TEnumerable, TSource>(this TEnumerable source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source);
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
    struct Shuffle<TEnumerable, TSource>(TEnumerable source)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        TSource[]? temp;
        int index = 0;

        public ValueEnumerator<Shuffle<TEnumerable, TSource>, TSource> GetEnumerator()
        {
            return new(this);
        }

        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination)
        {
            if (source.TryGetNonEnumeratedCount(out var count) && count <= destination.Length)
            {
                if (source.TryCopyTo(destination))
                {
                    RandomShared.Shuffle(destination.Slice(0, count));
                    return true;
                }
            }

            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (temp == null)
            {
                temp = source.ToArray<TEnumerable, TSource>(); // do not use pool(struct field can't gurantees state of reference)
                RandomShared.Shuffle(temp);
            }

            if (index < temp.Length)
            {
                current = temp[index++];
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
