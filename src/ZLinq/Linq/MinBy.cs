namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TSource? MinBy<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return MinBy(source, keySelector, null);
        }

        public static TSource? MinBy<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            comparer ??= Comparer<TKey>.Default;

            using (source)
            {
                if (!source.TryGetNext(out var value)) // TSource value;
                {
                    if (default(TSource) is null)
                    {
                        return default;
                    }
                    else
                    {
                        Throws.NoElements();
                    }
                }

                // store first value and key.
                TKey key = keySelector(value);

                if (default(TKey) is null)
                {
                    // reference type
                    if (key is null)
                    {
                        var firstValue = value;

                        do
                        {
                            if (!source.TryGetNext(out var current))
                            {
                                return firstValue; // return first-value when all keys are null.
                            }
                            value = current;
                            key = keySelector(value);
                        }
                        while (key is null);
                    }

                    while (source.TryGetNext(out var current))
                    {
                        var currentKey = keySelector(current);
                        if (currentKey is not null && comparer.Compare(currentKey, key) < 0)
                        {
                            key = currentKey;
                            value = current;
                        }
                    }
                }
                else
                {
                    // value type
                    if (comparer == Comparer<TKey>.Default)
                    {
                        while (source.TryGetNext(out var current))
                        {
                            var currentKey = keySelector(current);
                            if (Comparer<TKey>.Default.Compare(currentKey, key) < 0)
                            {
                                key = currentKey;
                                value = current;
                            }
                        }

                        return value;
                    }
                    else
                    {
                        while (source.TryGetNext(out var current))
                        {
                            var currentKey = keySelector(current);
                            if (comparer.Compare(currentKey, key) < 0)
                            {
                                key = currentKey;
                                value = current;
                            }
                        }
                    }
                }

                return value;
            }
        }
    }
}
