namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Prepend<TEnumerable, TSource> Prepend<TEnumerable, TSource>(this TEnumerable source, TSource element)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, element);

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
    struct Prepend<TEnumerable, TSource>(TEnumerable source, TSource element)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        byte state;

        public ValueEnumerator<Prepend<TEnumerable, TSource>, TSource> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                count++;
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
            if (!source.TryGetNonEnumeratedCount(out var srcCount)) return false;
            if (srcCount + 1 > dest.Length) return false;

            if (!source.TryCopyTo(dest.Slice(1)))
            {
                return false;
            }
            dest[0] = element;
            return true;
        }

        public bool TryGetNext(out TSource current)
        {
            if (state == 0)
            {
                current = element;
                state = 1;
                return true;
            }

            if (state == 1)
            {
                if (source.TryGetNext(out current))
                {
                    return true;
                }
                state = 2;
            }

            current = default!;
            return false;
        }

        public void Dispose()
        {
            state = 2;
            source.Dispose();
        }
    }

}
