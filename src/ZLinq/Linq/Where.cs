namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Where<TEnumerable, TSource> Where<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, predicate);

        public static WhereValueEnumerable2<TEnumerable, TSource> Where<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Int32, Boolean> predicate)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, predicate);
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
    struct Where<TEnumerable, TSource>(TEnumerable source, Func<TSource, Boolean> predicate)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public ValueEnumerator<Where<TEnumerable, TSource>, TSource> GetEnumerator()
        {
            return new(this);
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            while (source.TryGetNext(out var value))
            {
                if (predicate(value))
                {
                    current = value;
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

        // Optimize for common pattern: Where().Select()
        public WhereSelect<TEnumerable, TSource, TResult> Select<TResult>(Func<TSource, TResult> selector)
            => new(source, predicate, selector);
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct WhereValueEnumerable2<TEnumerable, TSource>(TEnumerable source, Func<TSource, Int32, Boolean> predicate)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        int index = 0;

        public ValueEnumerator<WhereValueEnumerable2<TEnumerable, TSource>, TSource> GetEnumerator()
        {
            return new(this);
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            while (source.TryGetNext(out var value))
            {
                if (predicate(value, index++))
                {
                    current = value;
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

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct WhereSelect<TEnumerable, TSource, TResult>(TEnumerable source, Func<TSource, Boolean> predicate, Func<TSource, TResult> selector)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public ValueEnumerator<WhereSelect<TEnumerable, TSource, TResult>, TResult> GetEnumerator()
        {
            return new(this);
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
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
                if (predicate(value))
                {
                    current = selector(value);
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
