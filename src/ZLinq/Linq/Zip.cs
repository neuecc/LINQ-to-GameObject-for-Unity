//namespace ZLinq
//{
//    partial class ValueEnumerableExtensions
//    {
//        public static ZipValueEnumerable<TEnumerable, TFirst, TSecond, TResult> Zip<TEnumerable, TFirst, TSecond, TResult>(this TEnumerable source, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
//            where TEnumerable : struct, IValueEnumerable<TFirst>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, second, resultSelector);

//        public static ZipValueEnumerable2<TEnumerable, TFirst, TSecond> Zip<TEnumerable, TFirst, TSecond>(this TEnumerable source, IEnumerable<TSecond> second)
//            where TEnumerable : struct, IValueEnumerable<TFirst>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, second);

//        public static ZipValueEnumerable3<TEnumerable, TFirst, TSecond, TThird> Zip<TEnumerable, TFirst, TSecond, TThird>(this TEnumerable source, IEnumerable<TSecond> second, IEnumerable<TThird> third)
//            where TEnumerable : struct, IValueEnumerable<TFirst>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, second, third);

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
//    struct ZipValueEnumerable<TEnumerable, TFirst, TSecond, TResult>(TEnumerable source, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
//        : IValueEnumerable<TResult>
//        where TEnumerable : struct, IValueEnumerable<TFirst>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public ValueEnumerator<ZipValueEnumerable<TEnumerable, TFirst, TSecond, TResult>, TResult> GetEnumerator() => new(this);

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
//    struct ZipValueEnumerable2<TEnumerable, TFirst, TSecond>(TEnumerable source, IEnumerable<TSecond> second)
//        : IValueEnumerable<ValueTuple`2>
//        where TEnumerable : struct, IValueEnumerable<TFirst>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public ValueEnumerator<ZipValueEnumerable2<TEnumerable, TFirst, TSecond>, ValueTuple`2> GetEnumerator() => new(this);

//        public bool TryGetNonEnumeratedCount(out int count)
//        {
//            throw new NotImplementedException();
//            // return source.TryGetNonEnumeratedCount(count);
//            // count = 0;
//            // return false;
//        }

//        public bool TryGetSpan(out ReadOnlySpan<ValueTuple`2> span)
//        {
//            throw new NotImplementedException();
//            // span = default;
//            // return false;
//        }

//        public bool TryGetNext(out ValueTuple`2 current)
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
//    struct ZipValueEnumerable3<TEnumerable, TFirst, TSecond, TThird>(TEnumerable source, IEnumerable<TSecond> second, IEnumerable<TThird> third)
//        : IValueEnumerable<ValueTuple`3>
//        where TEnumerable : struct, IValueEnumerable<TFirst>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public ValueEnumerator<ZipValueEnumerable3<TEnumerable, TFirst, TSecond, TThird>, ValueTuple`3> GetEnumerator() => new(this);

//        public bool TryGetNonEnumeratedCount(out int count)
//        {
//            throw new NotImplementedException();
//            // return source.TryGetNonEnumeratedCount(count);
//            // count = 0;
//            // return false;
//        }

//        public bool TryGetSpan(out ReadOnlySpan<ValueTuple`3> span)
//        {
//            throw new NotImplementedException();
//            // span = default;
//            // return false;
//        }

//        public bool TryGetNext(out ValueTuple`3 current)
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
