namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<Reverse<TEnumerator, TSource>, TSource> Reverse<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
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
    struct Reverse<TEnumerator, TSource>(TEnumerator source)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        TSource[]? array;
        int index;

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
                array = new ValueEnumerable<TEnumerator, TSource>(source).ToArray();
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
