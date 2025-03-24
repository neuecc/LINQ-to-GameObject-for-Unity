using System;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TSource ElementAt<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Int32 index)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetElementAt<TEnumerator, TSource>(ref enumerator, index, out var value)
                    ? value
                    : Throws.ArgumentOutOfRange<TSource>(nameof(index));
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static TSource ElementAt<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Index index)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetElementAt<TEnumerator, TSource>(ref enumerator, index, out var value)
                    ? value
                    : Throws.ArgumentOutOfRange<TSource>(nameof(index));
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static TSource? ElementAtOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Int32 index)
          where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (index < 0) return default; // allows negative index

            var enumerator = source.Enumerator;
            try
            {
                return TryGetElementAt<TEnumerator, TSource>(ref enumerator, index, out var value)
                ? value
                : default!;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static TSource? ElementAtOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Index index)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetElementAt<TEnumerator, TSource>(ref enumerator, index, out var value)
                ? value
                : default!;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        static bool TryGetElementAt<TEnumerator, TSource>(ref TEnumerator source, Index index, out TSource value)
           where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
           , allows ref struct
#endif
        {
            var current = default(TSource)!;
            if (source.TryCopyTo(SingleSpan.Create(ref current), index))
            {
                value = current;
                return true;
            }
            else if (EnumeratorHelper.TryConsumeGetAt<TEnumerator, TSource>(ref source, index, out current))
            {
                value = current!;
                return true;
            }

            value = default!;
            return false;
        }
    }
}
