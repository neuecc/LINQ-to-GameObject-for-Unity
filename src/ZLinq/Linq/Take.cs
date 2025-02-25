namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TakeValueEnumerable<TEnumerable, TSource> Take<TEnumerable, TSource>(this TEnumerable source, Int32 count)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, count);
            
        public static ValueEnumerator<TakeValueEnumerable<TEnumerable, TSource>, TSource> GetEnumerator<TEnumerable, TSource>(this TakeValueEnumerable<TEnumerable, TSource> source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source);

        public static TakeValueEnumerable2<TEnumerable, TSource> Take<TEnumerable, TSource>(this TEnumerable source, Range range)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, range);
            
        public static ValueEnumerator<TakeValueEnumerable2<TEnumerable, TSource>, TSource> GetEnumerator<TEnumerable, TSource>(this TakeValueEnumerable2<TEnumerable, TSource> source)
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
    struct TakeValueEnumerable<TEnumerable, TSource>(TEnumerable source, Int32 count)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

        public bool TryGetNext(out TSource current)
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
    struct TakeValueEnumerable2<TEnumerable, TSource>(TEnumerable source, Range range)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

        public bool TryGetNext(out TSource current)
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


//using System.Collections;
//using System.Dynamic;

//// TODO: impl test(not-checked)

//namespace ZLinq
//{
//    partial class ValueEnumerableExtensions
//    {
//        public static TakeValueEnumerable<TEnumerable, T> Take<TEnumerable, T>(this TEnumerable source, int count)
//            where TEnumerable : struct, IValueEnumerable<T>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//        {
//            return new(source, count);
//        }

//        //        public static StructEnumerator<TakeValueEnumerable<TEnumerable, T>> GetEnumerator<TEnumerable, T>(
//        //            this TakeValueEnumerable<TEnumerable, T> source)
//        //            where TEnumerable : struct, IValueEnumerable<T>
//        //#if NET9_0_OR_GREATER
//        //            , allows ref struct
//        //#endif
//        //        {
//        //            return new(source);
//        //        }
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
//    struct TakeValueEnumerable<TEnumerable, T>(TEnumerable source, int takeCount) : IValueEnumerable<T>
//        where TEnumerable : struct, IValueEnumerable<T>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;
//        int rest = takeCount;

//#if NET9_0_OR_GREATER
//        BitBool flags;
//        ReadOnlySpan<T> currentSpan;
//#endif

//        public bool TryGetNonEnumeratedCount(out int count)
//        {
//            if (source.TryGetNonEnumeratedCount(out var sourceCount))
//            {
//                count = Math.Min(takeCount, sourceCount);
//                return true;
//            }
//            count = default;
//            return false;
//        }

//        public bool TryGetSpan(out ReadOnlySpan<T> span)
//        {
//            if (source.TryGetSpan(out var sourceSpan))
//            {
//                var length = Math.Min(takeCount, sourceSpan.Length);
//                span = sourceSpan.Slice(0, length);
//                return true;
//            }

//            span = default;
//            return false;
//        }

//        public bool TryGetNext(out T current)
//        {

//#if NET9_0_OR_GREATER
//            if (flags.IsZero) // init
//            {
//                if (TryGetSpan(out currentSpan))
//                {
//                    flags.SetTrueToBit1();
//                    rest = Math.Min(takeCount, currentSpan.Length);
//                }
//                else
//                {
//                    flags.SetTrueToBit8(); // set dummy, IsZero and Bit1 is false
//                    rest = takeCount;
//                }
//            }

//            if (flags.IsBit1) // use Span
//            {
//                if (rest-- != 0)
//                {
//                    current = currentSpan[rest];
//                    return true;
//                }
//                else
//                {
//                    Unsafe.SkipInit(out current);
//                    return false;
//                }
//            }
//#endif

//            if (source.TryGetNext(out var value))
//            {
//                if (rest-- != 0)
//                {
//                    current = value;
//                    return true;
//                }
//            }

//            Unsafe.SkipInit(out current);
//            return false;
//        }

//        public void Dispose()
//        {
//            source.Dispose();
//        }
//    }
//}