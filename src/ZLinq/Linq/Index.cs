namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<Index<TEnumerator, TSource>, (int Index, TSource Item)> Index<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source)
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
    struct Index<TEnumerator, TSource>(in TEnumerator source)
        : IValueEnumerator<(int Index, TSource Item)>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            return source.TryGetNonEnumeratedCount(out count);
        }

        public bool TryGetSpan(out ReadOnlySpan<(int Index, TSource Item)> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<(int Index, TSource Item)> destination)
        {
            destination = default;
            return false;
        }

        public bool TryGetNext(out (int Index, TSource Item) current)
        {
            if (source.TryGetNext(out var value))
            {
                current = (index, value);
                checked { index++; }
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
