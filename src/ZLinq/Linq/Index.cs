namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Index<TEnumerable, TSource> Index<TEnumerable, TSource>(this TEnumerable source)
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
    struct Index<TEnumerable, TSource>(TEnumerable source)
        : IValueEnumerable<(int Index, TSource Item)>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        int index;

        public ValueEnumerator<Index<TEnumerable, TSource>, (int Index, TSource Item)> GetEnumerator() => new(this);

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
