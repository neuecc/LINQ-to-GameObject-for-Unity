namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TSource ElementAt<TEnumerable, TSource>(this TEnumerable source, Int32 index)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return TryGetElementAt<TEnumerable, TSource>(ref source, index, out var value)
                ? value
                : Throws.ArgumentOutOfRange<TSource>(nameof(index));
        }


        public static TSource ElementAt<TEnumerable, TSource>(this TEnumerable source, Index index)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return TryGetElementAt<TEnumerable, TSource>(ref source, index, out var value)
                ? value
                : Throws.ArgumentOutOfRange<TSource>(nameof(index));
        }

        public static TSource ElementAtOrDefault<TEnumerable, TSource>(this TEnumerable source, Int32 index)
          where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return TryGetElementAt<TEnumerable, TSource>(ref source, index, out var value)
                ? value
                : default!;
        }

        public static TSource ElementAtOrDefault<TEnumerable, TSource>(this TEnumerable source, Index index)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return TryGetElementAt<TEnumerable, TSource>(ref source, index, out var value)
                ? value
                : default!;
        }

        static bool TryGetElementAt<TEnumerable, TSource>(ref TEnumerable source, int index, out TSource value)
            where TEnumerable : struct, IValueEnumerable<TSource>
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

            using (source)
            {
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
        }

        static bool TryGetElementAt<TEnumerable, TSource>(ref TEnumerable source, Index index, out TSource value)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (!index.IsFromEnd)
            {
                return TryGetElementAt<TEnumerable, TSource>(ref source, index.Value, out value);
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

            using (source)
            {
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
            }

            value = default!;
            return false;
        }
    }
}
