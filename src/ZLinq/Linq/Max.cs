//namespace ZLinq
//{
//    partial class ValueEnumerableExtensions
//    {
//        public static Int32 Max<TEnumerator>(in this ValueEnumerable<TEnumerator, TSource> source)
//            where TEnumerator : struct, IValueEnumerable<System.Int32>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Int64 Max<TEnumerator>(in this ValueEnumerable<TEnumerator, TSource> source)
//            where TEnumerator : struct, IValueEnumerable<System.Int64>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Int32> Max<TEnumerator>(in this ValueEnumerable<TEnumerator, TSource> source)
//            where TEnumerator : struct, IValueEnumerable<System.Nullable`1[System.Int32]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Int64> Max<TEnumerator>(in this ValueEnumerable<TEnumerator, TSource> source)
//            where TEnumerator : struct, IValueEnumerable<System.Nullable`1[System.Int64]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Double Max<TEnumerator>(in this ValueEnumerable<TEnumerator, TSource> source)
//            where TEnumerator : struct, IValueEnumerable<System.Double>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Double> Max<TEnumerator>(in this ValueEnumerable<TEnumerator, TSource> source)
//            where TEnumerator : struct, IValueEnumerable<System.Nullable`1[System.Double]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Single Max<TEnumerator>(in this ValueEnumerable<TEnumerator, TSource> source)
//            where TEnumerator : struct, IValueEnumerable<System.Single>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Single> Max<TEnumerator>(in this ValueEnumerable<TEnumerator, TSource> source)
//            where TEnumerator : struct, IValueEnumerable<System.Nullable`1[System.Single]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Decimal Max<TEnumerator>(in this ValueEnumerable<TEnumerator, TSource> source)
//            where TEnumerator : struct, IValueEnumerable<System.Decimal>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Decimal> Max<TEnumerator>(in this ValueEnumerable<TEnumerator, TSource> source)
//            where TEnumerator : struct, IValueEnumerable<System.Nullable`1[System.Decimal]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static TSource Max<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source)
//            where TEnumerator : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static TSource Max<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, IComparer<TSource> comparer)
//            where TEnumerator : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Int32 Max<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Int32> selector)
//            where TEnumerator : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Int32> Max<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Nullable<Int32>> selector)
//            where TEnumerator : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Int64 Max<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Int64> selector)
//            where TEnumerator : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Int64> Max<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Nullable<Int64>> selector)
//            where TEnumerator : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Single Max<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Single> selector)
//            where TEnumerator : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Single> Max<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Nullable<Single>> selector)
//            where TEnumerator : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Double Max<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Double> selector)
//            where TEnumerator : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Double> Max<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Nullable<Double>> selector)
//            where TEnumerator : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Decimal Max<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Decimal> selector)
//            where TEnumerator : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Decimal> Max<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Nullable<Decimal>> selector)
//            where TEnumerator : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static TResult Max<TEnumerator, TSource, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TResult> selector)
//            where TEnumerator : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//    }
//}
