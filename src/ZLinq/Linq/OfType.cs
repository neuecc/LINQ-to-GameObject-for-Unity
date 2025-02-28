namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static OfType<TEnumerable, TSource, TResult> OfType<TEnumerable, TSource, TResult>(this TEnumerable source, TResult typeHint)
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
    struct OfType<TEnumerable, TSource, TResult>(TEnumerable source)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public ValueEnumerator<OfType<TEnumerable, TSource, TResult>, TResult> GetEnumerator()
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
