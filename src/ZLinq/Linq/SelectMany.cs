namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static SelectMany<TEnumerable, TSource, TResult> SelectMany<TEnumerable, TSource, TResult>(this TEnumerable source, Func<TSource, IEnumerable<TResult>> selector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, selector);

        public static ValueEnumerator<SelectMany<TEnumerable, TSource, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TResult>(this SelectMany<TEnumerable, TSource, TResult> source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source);

        public static SelectMany2<TEnumerable, TSource, TResult> SelectMany<TEnumerable, TSource, TResult>(this TEnumerable source, Func<TSource, Int32, IEnumerable<TResult>> selector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, selector);

        public static ValueEnumerator<SelectMany2<TEnumerable, TSource, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TResult>(this SelectMany2<TEnumerable, TSource, TResult> source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source);

        public static SelectMany3<TEnumerable, TSource, TCollection, TResult> SelectMany<TEnumerable, TSource, TCollection, TResult>(this TEnumerable source, Func<TSource, Int32, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, collectionSelector, resultSelector);

        public static ValueEnumerator<SelectMany3<TEnumerable, TSource, TCollection, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TCollection, TResult>(this SelectMany3<TEnumerable, TSource, TCollection, TResult> source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source);

        public static SelectMany4<TEnumerable, TSource, TCollection, TResult> SelectMany<TEnumerable, TSource, TCollection, TResult>(this TEnumerable source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, collectionSelector, resultSelector);

        public static ValueEnumerator<SelectMany4<TEnumerable, TSource, TCollection, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TCollection, TResult>(this SelectMany4<TEnumerable, TSource, TCollection, TResult> source)
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
    struct SelectMany<TEnumerable, TSource, TResult>(TEnumerable source, Func<TSource, IEnumerable<TResult>> selector)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
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
    struct SelectMany2<TEnumerable, TSource, TResult>(TEnumerable source, Func<TSource, Int32, IEnumerable<TResult>> selector)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

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
    struct SelectMany3<TEnumerable, TSource, TCollection, TResult>(TEnumerable source, Func<TSource, Int32, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

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
    struct SelectMany4<TEnumerable, TSource, TCollection, TResult>(TEnumerable source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

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
