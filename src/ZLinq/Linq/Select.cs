namespace ZLinq
{
    partial class StructEnumerableExtensions
    {
        public static SelectStructEnumerable<TEnumerable, T, TResult> Select<TEnumerable, T, TResult>(this TEnumerable source, Func<T, TResult> selector)
            where TEnumerable : struct, IStructEnumerable<T>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(source, selector);
        }

        public static StructEnumerator<SelectStructEnumerable<TEnumerable, T, TResult>, TResult> GetEnumerator<TEnumerable, T, TResult>(
            this SelectStructEnumerable<TEnumerable, T, TResult> source)
            where TEnumerable : struct, IStructEnumerable<T>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(source);
        }
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
    struct SelectStructEnumerable<TEnumerable, T, TResult>(TEnumerable source, Func<T, TResult> selector) : IStructEnumerable<TResult>
        where TEnumerable : struct, IStructEnumerable<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryGetNext(out TResult current)
        {
            if (source.TryGetNext(out var value))
            {
                current = selector(value);
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