using System.Numerics;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Select<TEnumerator, TSource, TResult> Select<TEnumerator, TSource, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TResult> selector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, selector);

        public static Select2<TEnumerator, TSource, TResult> Select<TEnumerator, TSource, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Int32, TResult> selector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, selector);


        // TODO: test impl


        public static ValueEnumerable<Select<TEnumerator, TSource, TResult>, TResult> Select2<TEnumerator, TSource, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TResult> selector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, selector));

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
    struct Select<TEnumerator, TSource, TResult>(TEnumerator source, Func<TSource, TResult> selector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        public ValueEnumerator<Select<TEnumerator, TSource, TResult>, TResult> GetEnumerator()
        {
            return new(this);
        }

        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TResult> destination)
        {
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

        // Optimize for common pattern: Select().Where()
        public SelectWhere<TEnumerator, TSource, TResult> Where(Func<TResult, bool> predicate)
            => new(source, selector, predicate);
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct Select2<TEnumerator, TSource, TResult>(TEnumerator source, Func<TSource, Int32, TResult> selector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        int index = 0;

        public ValueEnumerator<Select2<TEnumerator, TSource, TResult>, TResult> GetEnumerator()
        {
            return new(this);
        }

        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        // TODO: Optimize
        public bool TryCopyTo(Span<TResult> destination) => false;

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

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct SelectWhere<TEnumerator, TSource, TResult>(TEnumerator source, Func<TSource, TResult> selector, Func<TResult, bool> predicate)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        public ValueEnumerator<SelectWhere<TEnumerator, TSource, TResult>, TResult> GetEnumerator()
        {
            return new(this);
        }

        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TResult> destination) => false;

        public bool TryGetNext(out TResult current)
        {
            while (source.TryGetNext(out var value))
            {
                var result = selector(value);
                if (predicate(result))
                {
                    current = result;
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
