//namespace ZLinq
//{
//    partial class ValueEnumerableExtensions
//    {
//        public static Int32 Max<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Int32>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Int64 Max<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Int64>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Int32> Max<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Nullable`1[System.Int32]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Int64> Max<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Nullable`1[System.Int64]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Double Max<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Double>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Double> Max<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Nullable`1[System.Double]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Single Max<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Single>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Single> Max<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Nullable`1[System.Single]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Decimal Max<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Decimal>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Decimal> Max<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Nullable`1[System.Decimal]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static TSource Max<TEnumerable, TSource>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static TSource Max<TEnumerable, TSource>(this TEnumerable source, IComparer<TSource> comparer)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Int32 Max<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Int32> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Int32> Max<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Nullable<Int32>> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Int64 Max<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Int64> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Int64> Max<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Nullable<Int64>> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Single Max<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Single> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Single> Max<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Nullable<Single>> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Double Max<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Double> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Double> Max<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Nullable<Double>> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Decimal Max<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Decimal> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Decimal> Max<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Nullable<Decimal>> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static TResult Max<TEnumerable, TSource, TResult>(this TEnumerable source, Func<TSource, TResult> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//    }
//}
