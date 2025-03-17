namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static OfType<TEnumerator, TSource, TResult> OfType<TEnumerator, TSource, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, TResult typeHint)
            where TEnumerator : struct, IValueEnumerator<TSource>
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
    struct OfType<TEnumerator, TSource, TResult>(TEnumerator source)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        public ValueEnumerator<OfType<TEnumerator, TSource, TResult>, TResult> GetEnumerator()
        {
            return new(this);
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TResult> dest) => false;

        public bool TryGetNext(out TResult current)
        {
            while (source.TryGetNext(out var value))
            {
                if (value is TResult v)
                {
                    current = v;
                    return true;
                }
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
