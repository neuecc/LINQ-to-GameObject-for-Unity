namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static SelectManyValueEnumerable<TEnumerable, TSource, TResult> SelectMany<TEnumerable, TSource, TResult>(this TEnumerable source, Func<TSource, IEnumerable<TResult>> selector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, selector);
            
        public static ValueEnumerator<SelectManyValueEnumerable<TEnumerable, TSource, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TResult>(this SelectManyValueEnumerable<TEnumerable, TSource, TResult> source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source);

        public static SelectManyValueEnumerable2<TEnumerable, TSource, TResult> SelectMany<TEnumerable, TSource, TResult>(this TEnumerable source, Func<TSource, Int32, IEnumerable<TResult>> selector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, selector);
            
        public static ValueEnumerator<SelectManyValueEnumerable2<TEnumerable, TSource, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TResult>(this SelectManyValueEnumerable2<TEnumerable, TSource, TResult> source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source);

        public static SelectManyValueEnumerable3<TEnumerable, TSource, TCollection, TResult> SelectMany<TEnumerable, TSource, TCollection, TResult>(this TEnumerable source, Func<TSource, Int32, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, collectionSelector, resultSelector);
            
        public static ValueEnumerator<SelectManyValueEnumerable3<TEnumerable, TSource, TCollection, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TCollection, TResult>(this SelectManyValueEnumerable3<TEnumerable, TSource, TCollection, TResult> source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source);

        public static SelectManyValueEnumerable4<TEnumerable, TSource, TCollection, TResult> SelectMany<TEnumerable, TSource, TCollection, TResult>(this TEnumerable source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, collectionSelector, resultSelector);
            
        public static ValueEnumerator<SelectManyValueEnumerable4<TEnumerable, TSource, TCollection, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TCollection, TResult>(this SelectManyValueEnumerable4<TEnumerable, TSource, TCollection, TResult> source)
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
    struct SelectManyValueEnumerable<TEnumerable, TSource, TResult>(TEnumerable source, Func<TSource, IEnumerable<TResult>> selector)
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
    struct SelectManyValueEnumerable2<TEnumerable, TSource, TResult>(TEnumerable source, Func<TSource, Int32, IEnumerable<TResult>> selector)
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
    struct SelectManyValueEnumerable3<TEnumerable, TSource, TCollection, TResult>(TEnumerable source, Func<TSource, Int32, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
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
    struct SelectManyValueEnumerable4<TEnumerable, TSource, TCollection, TResult>(TEnumerable source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
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
