namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Skip<TEnumerator, TSource> Skip<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Int32 count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, count);

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
    struct Skip<TEnumerator, TSource>(TEnumerator source, Int32 count)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        public ValueEnumerator<Skip<TEnumerator, TSource>, TSource> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            throw new NotImplementedException();
            // return source.TryGetNonEnumeratedCount(count);
            // count = 0;
            // return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

        public bool TryCopyTo(Span<TSource> destination) => false;

        public bool TryGetNext(out TSource current)
        {
            throw new NotImplementedException();
            // Unsafe.SkipInit(out current);
            // return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }

}
