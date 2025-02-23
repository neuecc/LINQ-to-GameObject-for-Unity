namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static SelectValueEnumerable<TEnumerable, T, TResult> Select<TEnumerable, T, TResult>(this TEnumerable source, Func<T, TResult> selector)
            where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
             => new(source, selector);

        public static SelectValueEnumerable2<TEnumerable, T, TResult> Select<TEnumerable, T, TResult>(this TEnumerable source, Func<T, int, TResult> selector)
    where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
             => new(source, selector);

        public static ValueEnumerator<SelectValueEnumerable<TEnumerable, T, TResult>, TResult> GetEnumerator<TEnumerable, T, TResult>(
            this SelectValueEnumerable<TEnumerable, T, TResult> source)
            where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
             => new(source);

        public static ValueEnumerator<SelectValueEnumerable2<TEnumerable, T, TResult>, TResult> GetEnumerator<TEnumerable, T, TResult>(
            this SelectValueEnumerable2<TEnumerable, T, TResult> source)
            where TEnumerable : struct, IValueEnumerable<T>
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
    struct SelectValueEnumerable<TEnumerable, T, TResult>(TEnumerable source, Func<T, TResult> selector) : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<T>
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
    struct SelectValueEnumerable2<TEnumerable, T, TResult>(TEnumerable source, Func<T, int, TResult> selector) : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        int index;

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