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

#if !NETSTANDARD2_0

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

#endif

        public static TSource ElementAtOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Int32 index)
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

#if !NETSTANDARD2_0

        public static TSource ElementAtOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Index index)
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

#endif

        static bool TryGetElementAt<TEnumerator, TSource>(ref TEnumerator source, int index, out TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                if (index < 0 || index >= span.Length)
                {
                    value = default!;
                    return false;
                }

                value = span[index];
                return true;
            }

            if (index >= 0)
            {
                while (source.TryGetNext(out var current))
                {
                    if (index == 0)
                    {
                        value = current;
                        return true;
                    }
                    index--;
                }
            }

            value = default!;
            return false;
        }

        static bool TryGetElementAt<TEnumerator, TSource>(ref TEnumerator source, Index index, out TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (!index.IsFromEnd)
            {
                return TryGetElementAt<TEnumerator, TSource>(ref source, index.Value, out value);
            }

            var indexFromEnd = index.Value;

            if (source.TryGetSpan(out var span))
            {
                if (indexFromEnd < 0 || indexFromEnd >= span.Length)
                {
                    value = default!;
                    return false;
                }

                value = span[index];
                return true;
            }

            using var q = new ValueQueue<TSource>(4);
            while (source.TryGetNext(out var current))
            {
                if (q.Count == indexFromEnd)
                {
                    q.Dequeue();
                }
                q.Enqueue(current);
            }

            if (q.Count == indexFromEnd)
            {
                value = q.Dequeue();
                return true;
            }

            value = default!;
            return false;
        }
    }
}
