//namespace ZLinq
//{
//    partial class ValueEnumerableExtensions
//    {
//        public static CountByValueEnumerable<TEnumerable, TSource, TKey> CountBy<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, keyComparer);
            
//        public static ValueEnumerator<CountByValueEnumerable<TEnumerable, TSource, TKey>, KeyValuePair`2> GetEnumerator<TEnumerable, TSource, TKey>(this CountByValueEnumerable<TEnumerable, TSource, TKey> source)
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
//    struct CountByValueEnumerable<TEnumerable, TSource, TKey>(TEnumerable source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
//        : IValueEnumerable<KeyValuePair`2>
//        where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

//        public bool TryGetSpan(out ReadOnlySpan<KeyValuePair`2> span)
//        {
//            throw new NotImplementedException();
//            // span = default;
//            // return false;
//        }

//        public bool TryGetNext(out KeyValuePair`2 current)
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
