namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static SelectMany<TEnumerator, TSource, TResult> SelectMany<TEnumerator, TSource, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, IEnumerable<TResult>> selector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, selector);

        public static ValueEnumerator<SelectMany<TEnumerator, TSource, TResult>, TResult> GetEnumerator<TEnumerator, TSource, TResult>(this SelectMany<TEnumerator, TSource, TResult> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source);

        public static SelectMany2<TEnumerator, TSource, TResult> SelectMany<TEnumerator, TSource, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Int32, IEnumerable<TResult>> selector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, selector);

        public static ValueEnumerator<SelectMany2<TEnumerator, TSource, TResult>, TResult> GetEnumerator<TEnumerator, TSource, TResult>(this SelectMany2<TEnumerator, TSource, TResult> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source);

        public static SelectMany3<TEnumerator, TSource, TCollection, TResult> SelectMany<TEnumerator, TSource, TCollection, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Int32, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, collectionSelector, resultSelector);

        public static ValueEnumerator<SelectMany3<TEnumerator, TSource, TCollection, TResult>, TResult> GetEnumerator<TEnumerator, TSource, TCollection, TResult>(this SelectMany3<TEnumerator, TSource, TCollection, TResult> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source);

        public static SelectMany4<TEnumerator, TSource, TCollection, TResult> SelectMany<TEnumerator, TSource, TCollection, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, collectionSelector, resultSelector);

        public static ValueEnumerator<SelectMany4<TEnumerator, TSource, TCollection, TResult>, TResult> GetEnumerator<TEnumerator, TSource, TCollection, TResult>(this SelectMany4<TEnumerator, TSource, TCollection, TResult> source)
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
    struct SelectMany<TEnumerator, TSource, TResult>(TEnumerator source, Func<TSource, IEnumerable<TResult>> selector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        IEnumerator<TResult>? innerEnumerator;

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

        public bool TryCopyTo(Span<TResult> destination) => false;

        public bool TryGetNext(out TResult current)
        {
        BEGIN:
            if (innerEnumerator != null)
            {
                if (innerEnumerator.MoveNext())
                {
                    current = innerEnumerator.Current;
                    return true;
                }

                innerEnumerator.Dispose();
                innerEnumerator = null;
            }

            if (source.TryGetNext(out var value))
            {
                innerEnumerator = selector(value).GetEnumerator();
                goto BEGIN;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (innerEnumerator != null)
            {
                innerEnumerator.Dispose();
            }
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
    struct SelectMany2<TEnumerator, TSource, TResult>(TEnumerator source, Func<TSource, Int32, IEnumerable<TResult>> selector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

        public bool TryCopyTo(Span<TResult> destination) => false;

        public bool TryGetNext(out TResult current)
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

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct SelectMany3<TEnumerator, TSource, TCollection, TResult>(TEnumerator source, Func<TSource, Int32, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

        public bool TryCopyTo(Span<TResult> destination) => false;

        public bool TryGetNext(out TResult current)
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

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct SelectMany4<TEnumerator, TSource, TCollection, TResult>(TEnumerator source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

        public bool TryCopyTo(Span<TResult> destination) => false;

        public bool TryGetNext(out TResult current)
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
