//namespace ZLinq
//{
//    partial class ValueEnumerableExtensions
//    {
//        public static Zip<TEnumerator, TFirst, TSecond, TResult> Zip<TEnumerator, TFirst, TSecond, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
//            where TEnumerator : struct, IValueEnumerable<TFirst>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, second, resultSelector);

//        public static Zip2<TEnumerator, TFirst, TSecond> Zip<TEnumerator, TFirst, TSecond>(in this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TSecond> second)
//            where TEnumerator : struct, IValueEnumerable<TFirst>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, second);

//        public static Zip3<TEnumerator, TFirst, TSecond, TThird> Zip<TEnumerator, TFirst, TSecond, TThird>(in this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TSecond> second, IEnumerable<TThird> third)
//            where TEnumerator : struct, IValueEnumerable<TFirst>
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
//    struct Zip<TEnumerator, TFirst, TSecond, TResult>(TEnumerator source, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
//        : IValueEnumerable<TResult>
//        where TEnumerator : struct, IValueEnumerable<TFirst>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerator source = source;

//        public ValueEnumerator<Zip<TEnumerator, TFirst, TSecond, TResult>, TResult> GetEnumerator() => new(this);

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
//    struct Zip2<TEnumerator, TFirst, TSecond>(TEnumerator source, IEnumerable<TSecond> second)
//        : IValueEnumerable<ValueTuple`2>
//        where TEnumerator : struct, IValueEnumerable<TFirst>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerator source = source;

//        public ValueEnumerator<Zip2<TEnumerator, TFirst, TSecond>, ValueTuple`2> GetEnumerator() => new(this);

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
//    struct Zip3<TEnumerator, TFirst, TSecond, TThird>(TEnumerator source, IEnumerable<TSecond> second, IEnumerable<TThird> third)
//        : IValueEnumerable<ValueTuple`3>
//        where TEnumerator : struct, IValueEnumerable<TFirst>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerator source = source;

//        public ValueEnumerator<Zip3<TEnumerator, TFirst, TSecond, TThird>, ValueTuple`3> GetEnumerator() => new(this);

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
