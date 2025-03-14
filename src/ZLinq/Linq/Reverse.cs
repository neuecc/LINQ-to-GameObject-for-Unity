namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Reverse<TEnumerable, TSource> Reverse<TEnumerable, TSource>(this TEnumerable source)
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
    struct Reverse<TEnumerable, TSource>(TEnumerable source)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        TSource[]? array;
        int index;

        public ValueEnumerator<Reverse<TEnumerable, TSource>, TSource> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            return source.TryGetNonEnumeratedCount(out count);
        }

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
                    destination.Slice(0, count).Reverse();
                    return true;
                }
            }

            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (array == null)
            {
                array = source.ToArray<TEnumerable, TSource>();
                Array.Reverse(array);
            }

            if (index < array.Length)
            {
                current = array[index];
                index++;
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
