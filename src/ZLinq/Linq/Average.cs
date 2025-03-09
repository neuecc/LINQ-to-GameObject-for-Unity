using System.Numerics;

//namespace ZLinq
//{
//#if NET8_0_OR_GREATER

//    partial class ValueEnumerableExtensions
//    {
//        public static Double Average<TEnumerable, TNumber>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<TNumber>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            where TNumber : INumber<TNumber>
//        {
//            double count = 0;
//            var sum = TNumber.Zero;
//            while (source.TryGetNext(out var value))
//            {
//                sum += value;
//                count++;
//            }

//            // var a = sum / count;

//            // Sum / Count
//            // return T.CreateChecked(SumCore(source)) / T.CreateChecked(source.Length);

//            if (source.TryGetSpan(out var span))
//            {

//                // Enumerable.Range(1, 10).Average();

//            }

//            throw new NotImplementedException();
//        }
//    }
//}

//#endif

//namespace ZLinq
//{
//    partial class ValueEnumerableExtensions
//    {
//        public static Double Average<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Int32>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Double Average<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Int64>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Single Average<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Single>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Double Average<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Double>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Decimal Average<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Decimal>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Double> Average<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Nullable`1[System.Int32]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Double> Average<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Nullable`1[System.Int64]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Single> Average<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Nullable`1[System.Single]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Double> Average<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Nullable`1[System.Double]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Decimal> Average<TEnumerable>(this TEnumerable source)
//            where TEnumerable : struct, IValueEnumerable<System.Nullable`1[System.Decimal]>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Double Average<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Int32> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Double Average<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Int64> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Single Average<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Single> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Double Average<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Double> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Decimal Average<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Decimal> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Double> Average<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Nullable<Int32>> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Double> Average<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Nullable<Int64>> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Single> Average<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Nullable<Single>> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Double> Average<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Nullable<Double>> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//        public static Nullable<Decimal> Average<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Nullable<Decimal>> selector)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            throw new NotImplementedException();
//        }

//    }
//}
