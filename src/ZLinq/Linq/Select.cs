namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static SelectValueEnumerable<TEnumerable, TSource, TResult> Select<TEnumerable, TSource, TResult>(this TEnumerable source, Func<TSource, TResult> selector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, selector);

        public static ValueEnumerator<SelectValueEnumerable<TEnumerable, TSource, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TResult>(this SelectValueEnumerable<TEnumerable, TSource, TResult> source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source);

        public static SelectValueEnumerable2<TEnumerable, TSource, TResult> Select<TEnumerable, TSource, TResult>(this TEnumerable source, Func<TSource, Int32, TResult> selector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, selector);

        public static ValueEnumerator<SelectValueEnumerable2<TEnumerable, TSource, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TResult>(this SelectValueEnumerable2<TEnumerable, TSource, TResult> source)
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
    struct SelectValueEnumerable<TEnumerable, TSource, TResult>(TEnumerable source, Func<TSource, TResult> selector)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TSource>
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

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct SelectValueEnumerable2<TEnumerable, TSource, TResult>(TEnumerable source, Func<TSource, Int32, TResult> selector)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        int index = 0;

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
                current = selector(value, index++);
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