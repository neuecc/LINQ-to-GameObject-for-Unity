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

//        public static GroupByValueEnumerable2<TEnumerable, TSource, TKey> GroupBy<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, comparer);

//        public static GroupByValueEnumerable3<TEnumerable, TSource, TKey, TElement> GroupBy<TEnumerable, TSource, TKey, TElement>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, elementSelector);

//        public static GroupByValueEnumerable4<TEnumerable, TSource, TKey, TElement> GroupBy<TEnumerable, TSource, TKey, TElement>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, elementSelector, comparer);

//        public static GroupByValueEnumerable5<TEnumerable, TSource, TKey, TResult> GroupBy<TEnumerable, TSource, TKey, TResult>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, resultSelector);

//        public static GroupByValueEnumerable6<TEnumerable, TSource, TKey, TResult> GroupBy<TEnumerable, TSource, TKey, TResult>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, resultSelector, comparer);

//        public static GroupByValueEnumerable7<TEnumerable, TSource, TKey, TElement, TResult> GroupBy<TEnumerable, TSource, TKey, TElement, TResult>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, elementSelector, resultSelector);

//        public static GroupByValueEnumerable8<TEnumerable, TSource, TKey, TElement, TResult> GroupBy<TEnumerable, TSource, TKey, TElement, TResult>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, elementSelector, resultSelector, comparer);

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

//        public ValueEnumerator<GroupByValueEnumerable<TEnumerable, TSource, TKey>, IGrouping`2> GetEnumerator() => new(this);

//        public bool TryGetNonEnumeratedCount(out int count)
//        {
//            throw new NotImplementedException();
//            // return source.TryGetNonEnumeratedCount(count);
//            // count = 0;
//            // return false;
//        }

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

//        public ValueEnumerator<GroupByValueEnumerable2<TEnumerable, TSource, TKey>, IGrouping`2> GetEnumerator() => new(this);

//        public bool TryGetNonEnumeratedCount(out int count)
//        {
//            throw new NotImplementedException();
//            // return source.TryGetNonEnumeratedCount(count);
//            // count = 0;
//            // return false;
//        }

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

//        public ValueEnumerator<GroupByValueEnumerable3<TEnumerable, TSource, TKey, TElement>, IGrouping`2> GetEnumerator() => new(this);

//        public bool TryGetNonEnumeratedCount(out int count)
//        {
//            throw new NotImplementedException();
//            // return source.TryGetNonEnumeratedCount(count);
//            // count = 0;
//            // return false;
//        }

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

//        public ValueEnumerator<GroupByValueEnumerable4<TEnumerable, TSource, TKey, TElement>, IGrouping`2> GetEnumerator() => new(this);

//        public bool TryGetNonEnumeratedCount(out int count)
//        {
//            throw new NotImplementedException();
//            // return source.TryGetNonEnumeratedCount(count);
//            // count = 0;
//            // return false;
//        }

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

//        public ValueEnumerator<GroupByValueEnumerable5<TEnumerable, TSource, TKey, TResult>, TResult> GetEnumerator() => new(this);

//        public bool TryGetNonEnumeratedCount(out int count)
//        {
//            throw new NotImplementedException();
//            // return source.TryGetNonEnumeratedCount(count);
//            // count = 0;
//            // return false;
//        }

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

//        public ValueEnumerator<GroupByValueEnumerable6<TEnumerable, TSource, TKey, TResult>, TResult> GetEnumerator() => new(this);

//        public bool TryGetNonEnumeratedCount(out int count)
//        {
//            throw new NotImplementedException();
//            // return source.TryGetNonEnumeratedCount(count);
//            // count = 0;
//            // return false;
//        }

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

//        public ValueEnumerator<GroupByValueEnumerable7<TEnumerable, TSource, TKey, TElement, TResult>, TResult> GetEnumerator() => new(this);

//        public bool TryGetNonEnumeratedCount(out int count)
//        {
//            throw new NotImplementedException();
//            // return source.TryGetNonEnumeratedCount(count);
//            // count = 0;
//            // return false;
//        }

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

//        public ValueEnumerator<GroupByValueEnumerable8<TEnumerable, TSource, TKey, TElement, TResult>, TResult> GetEnumerator() => new(this);

//        public bool TryGetNonEnumeratedCount(out int count)
//        {
//            throw new NotImplementedException();
//            // return source.TryGetNonEnumeratedCount(count);
//            // count = 0;
//            // return false;
//        }

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
