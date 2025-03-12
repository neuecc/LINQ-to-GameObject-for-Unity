namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TakeWhile<TEnumerable, TSource> TakeWhile<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, predicate);

        public static TakeWhile2<TEnumerable, TSource> TakeWhile<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Int32, Boolean> predicate)
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
    struct TakeWhile<TEnumerable, TSource>(TEnumerable source, Func<TSource, Boolean> predicate)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public ValueEnumerator<TakeWhile<TEnumerable, TSource>, TSource> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination) => false;

        public bool TryGetNext(out TSource current)
        {
            if (source.TryGetNext(out current) && predicate(current))
            {
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
    struct TakeWhile2<TEnumerable, TSource>(TEnumerable source, Func<TSource, Int32, Boolean> predicate)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        int index = 0;

        public ValueEnumerator<TakeWhile2<TEnumerable, TSource>, TSource> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination) => false;

        public bool TryGetNext(out TSource current)
        {
            if (source.TryGetNext(out current) && predicate(current, index++))
            {
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
