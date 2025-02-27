//namespace ZLinq
//{
//    partial class ValueEnumerableExtensions
//    {
//        public static IndexValueEnumerable<TEnumerable, TSource> Index<TEnumerable, TSource>(this TEnumerable source)
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
//    struct IndexValueEnumerable<TEnumerable, TSource>(TEnumerable source)
//        : IValueEnumerable<ValueTuple`2>
//        where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public ValueEnumerator<IndexValueEnumerable<TEnumerable, TSource>, ValueTuple`2> GetEnumerator() => new(this);

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

//}
