namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<Append<TEnumerator, TSource>, TSource> Append<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, TSource element)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, element));
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
    struct Append<TEnumerator, TSource>(in TEnumerator source, TSource element)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        byte state;

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

            if (!source.TryCopyTo(dest))
            {
                return false;
            }
            dest[srcCount] = element;
            return true;
        }

        public bool TryGetNext(out TSource current)
        {
            if (state == 0)
            {
                if (source.TryGetNext(out current))
                {
                    return true;
                }
                state = 1;
            }

            if (state == 1)
            {
                current = element;
                state = 2;
                return true;
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
