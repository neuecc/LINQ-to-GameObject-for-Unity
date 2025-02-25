//namespace ZLinq
//{
//    partial class ValueEnumerableExtensions
//    {
//        public static GroupByValueEnumerable<TEnumerable, TSource, TKey> GroupBy<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector);
            
//        public static ValueEnumerator<GroupByValueEnumerable<TEnumerable, TSource, TKey>, IGrouping`2> GetEnumerator<TEnumerable, TSource, TKey>(this GroupByValueEnumerable<TEnumerable, TSource, TKey> source)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source);

//        public static GroupByValueEnumerable2<TEnumerable, TSource, TKey> GroupBy<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, comparer);
            
//        public static ValueEnumerator<GroupByValueEnumerable2<TEnumerable, TSource, TKey>, IGrouping`2> GetEnumerator<TEnumerable, TSource, TKey>(this GroupByValueEnumerable2<TEnumerable, TSource, TKey> source)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source);

//        public static GroupByValueEnumerable3<TEnumerable, TSource, TKey, TElement> GroupBy<TEnumerable, TSource, TKey, TElement>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, elementSelector);
            
//        public static ValueEnumerator<GroupByValueEnumerable3<TEnumerable, TSource, TKey, TElement>, IGrouping`2> GetEnumerator<TEnumerable, TSource, TKey, TElement>(this GroupByValueEnumerable3<TEnumerable, TSource, TKey, TElement> source)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source);

//        public static GroupByValueEnumerable4<TEnumerable, TSource, TKey, TElement> GroupBy<TEnumerable, TSource, TKey, TElement>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, elementSelector, comparer);
            
//        public static ValueEnumerator<GroupByValueEnumerable4<TEnumerable, TSource, TKey, TElement>, IGrouping`2> GetEnumerator<TEnumerable, TSource, TKey, TElement>(this GroupByValueEnumerable4<TEnumerable, TSource, TKey, TElement> source)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source);

//        public static GroupByValueEnumerable5<TEnumerable, TSource, TKey, TResult> GroupBy<TEnumerable, TSource, TKey, TResult>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, resultSelector);
            
//        public static ValueEnumerator<GroupByValueEnumerable5<TEnumerable, TSource, TKey, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TKey, TResult>(this GroupByValueEnumerable5<TEnumerable, TSource, TKey, TResult> source)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source);

//        public static GroupByValueEnumerable6<TEnumerable, TSource, TKey, TResult> GroupBy<TEnumerable, TSource, TKey, TResult>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, resultSelector, comparer);
            
//        public static ValueEnumerator<GroupByValueEnumerable6<TEnumerable, TSource, TKey, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TKey, TResult>(this GroupByValueEnumerable6<TEnumerable, TSource, TKey, TResult> source)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source);

//        public static GroupByValueEnumerable7<TEnumerable, TSource, TKey, TElement, TResult> GroupBy<TEnumerable, TSource, TKey, TElement, TResult>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, elementSelector, resultSelector);
            
//        public static ValueEnumerator<GroupByValueEnumerable7<TEnumerable, TSource, TKey, TElement, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TKey, TElement, TResult>(this GroupByValueEnumerable7<TEnumerable, TSource, TKey, TElement, TResult> source)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source);

//        public static GroupByValueEnumerable8<TEnumerable, TSource, TKey, TElement, TResult> GroupBy<TEnumerable, TSource, TKey, TElement, TResult>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, elementSelector, resultSelector, comparer);
            
//        public static ValueEnumerator<GroupByValueEnumerable8<TEnumerable, TSource, TKey, TElement, TResult>, TResult> GetEnumerator<TEnumerable, TSource, TKey, TElement, TResult>(this GroupByValueEnumerable8<TEnumerable, TSource, TKey, TElement, TResult> source)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source);

//    }
//}

//namespace ZLinq.Linq
//{
//    [StructLayout(LayoutKind.Auto)]
//    [EditorBrowsable(EditorBrowsableState.Never)]
//#if NET9_0_OR_GREATER
//    public ref
//#else
//    public
//#endif
//    struct GroupByValueEnumerable<TEnumerable, TSource, TKey>(TEnumerable source, Func<TSource, TKey> keySelector)
//        : IValueEnumerable<IGrouping`2>
//        where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

//        public bool TryGetSpan(out ReadOnlySpan<IGrouping`2> span)
//        {
//            throw new NotImplementedException();
//            // span = default;
//            // return false;
//        }

//        public bool TryGetNext(out IGrouping`2 current)
//        {
//            throw new NotImplementedException();
//            // Unsafe.SkipInit(out current);
//            // return false;
//        }

//        public void Dispose()
//        {
//            source.Dispose();
//        }
//    }

//    [StructLayout(LayoutKind.Auto)]
//    [EditorBrowsable(EditorBrowsableState.Never)]
//#if NET9_0_OR_GREATER
//    public ref
//#else
//    public
//#endif
//    struct GroupByValueEnumerable2<TEnumerable, TSource, TKey>(TEnumerable source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
//        : IValueEnumerable<IGrouping`2>
//        where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

//        public bool TryGetSpan(out ReadOnlySpan<IGrouping`2> span)
//        {
//            throw new NotImplementedException();
//            // span = default;
//            // return false;
//        }

//        public bool TryGetNext(out IGrouping`2 current)
//        {
//            throw new NotImplementedException();
//            // Unsafe.SkipInit(out current);
//            // return false;
//        }

//        public void Dispose()
//        {
//            source.Dispose();
//        }
//    }

//    [StructLayout(LayoutKind.Auto)]
//    [EditorBrowsable(EditorBrowsableState.Never)]
//#if NET9_0_OR_GREATER
//    public ref
//#else
//    public
//#endif
//    struct GroupByValueEnumerable3<TEnumerable, TSource, TKey, TElement>(TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
//        : IValueEnumerable<IGrouping`2>
//        where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

//        public bool TryGetSpan(out ReadOnlySpan<IGrouping`2> span)
//        {
//            throw new NotImplementedException();
//            // span = default;
//            // return false;
//        }

//        public bool TryGetNext(out IGrouping`2 current)
//        {
//            throw new NotImplementedException();
//            // Unsafe.SkipInit(out current);
//            // return false;
//        }

//        public void Dispose()
//        {
//            source.Dispose();
//        }
//    }

//    [StructLayout(LayoutKind.Auto)]
//    [EditorBrowsable(EditorBrowsableState.Never)]
//#if NET9_0_OR_GREATER
//    public ref
//#else
//    public
//#endif
//    struct GroupByValueEnumerable4<TEnumerable, TSource, TKey, TElement>(TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
//        : IValueEnumerable<IGrouping`2>
//        where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

//        public bool TryGetSpan(out ReadOnlySpan<IGrouping`2> span)
//        {
//            throw new NotImplementedException();
//            // span = default;
//            // return false;
//        }

//        public bool TryGetNext(out IGrouping`2 current)
//        {
//            throw new NotImplementedException();
//            // Unsafe.SkipInit(out current);
//            // return false;
//        }

//        public void Dispose()
//        {
//            source.Dispose();
//        }
//    }

//    [StructLayout(LayoutKind.Auto)]
//    [EditorBrowsable(EditorBrowsableState.Never)]
//#if NET9_0_OR_GREATER
//    public ref
//#else
//    public
//#endif
//    struct GroupByValueEnumerable5<TEnumerable, TSource, TKey, TResult>(TEnumerable source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
//        : IValueEnumerable<TResult>
//        where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

//        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
//        {
//            throw new NotImplementedException();
//            // span = default;
//            // return false;
//        }

//        public bool TryGetNext(out TResult current)
//        {
//            throw new NotImplementedException();
//            // Unsafe.SkipInit(out current);
//            // return false;
//        }

//        public void Dispose()
//        {
//            source.Dispose();
//        }
//    }

//    [StructLayout(LayoutKind.Auto)]
//    [EditorBrowsable(EditorBrowsableState.Never)]
//#if NET9_0_OR_GREATER
//    public ref
//#else
//    public
//#endif
//    struct GroupByValueEnumerable6<TEnumerable, TSource, TKey, TResult>(TEnumerable source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
//        : IValueEnumerable<TResult>
//        where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

//        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
//        {
//            throw new NotImplementedException();
//            // span = default;
//            // return false;
//        }

//        public bool TryGetNext(out TResult current)
//        {
//            throw new NotImplementedException();
//            // Unsafe.SkipInit(out current);
//            // return false;
//        }

//        public void Dispose()
//        {
//            source.Dispose();
//        }
//    }

//    [StructLayout(LayoutKind.Auto)]
//    [EditorBrowsable(EditorBrowsableState.Never)]
//#if NET9_0_OR_GREATER
//    public ref
//#else
//    public
//#endif
//    struct GroupByValueEnumerable7<TEnumerable, TSource, TKey, TElement, TResult>(TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
//        : IValueEnumerable<TResult>
//        where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

//        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
//        {
//            throw new NotImplementedException();
//            // span = default;
//            // return false;
//        }

//        public bool TryGetNext(out TResult current)
//        {
//            throw new NotImplementedException();
//            // Unsafe.SkipInit(out current);
//            // return false;
//        }

//        public void Dispose()
//        {
//            source.Dispose();
//        }
//    }

//    [StructLayout(LayoutKind.Auto)]
//    [EditorBrowsable(EditorBrowsableState.Never)]
//#if NET9_0_OR_GREATER
//    public ref
//#else
//    public
//#endif
//    struct GroupByValueEnumerable8<TEnumerable, TSource, TKey, TElement, TResult>(TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
//        : IValueEnumerable<TResult>
//        where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

//        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
//        {
//            throw new NotImplementedException();
//            // span = default;
//            // return false;
//        }

//        public bool TryGetNext(out TResult current)
//        {
//            throw new NotImplementedException();
//            // Unsafe.SkipInit(out current);
//            // return false;
//        }

//        public void Dispose()
//        {
//            source.Dispose();
//        }
//    }

//}
