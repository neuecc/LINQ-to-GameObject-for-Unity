namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        // Because the original implementation uses a Dictionary, I used a Dictionary with TKey : notnull in the same way.
        // However, I think it would be better without any null constraints.
        // https://github.com/dotnet/runtime/issues/98259
        // The enumeration order of Dictionary is guaranteed to be in the order of addition unless items are removed (according to comments in the issue).

        public static ValueEnumerable<AggregateBy<TEnumerator, TSource, TKey, TAccumulate>, KeyValuePair<TKey, TAccumulate>> AggregateBy<TEnumerator, TSource, TKey, TAccumulate>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TKey : notnull
            => new(new(source.Enumerator, keySelector, seed, func, null));

        public static ValueEnumerable<AggregateBy<TEnumerator, TSource, TKey, TAccumulate>, KeyValuePair<TKey, TAccumulate>> AggregateBy<TEnumerator, TSource, TKey, TAccumulate>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey>? keyComparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TKey : notnull
            => new(new(source.Enumerator, keySelector, seed, func, keyComparer));

        public static ValueEnumerable<AggregateBy2<TEnumerator, TSource, TKey, TAccumulate>, KeyValuePair<TKey, TAccumulate>> AggregateBy<TEnumerator, TSource, TKey, TAccumulate>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TAccumulate> seedSelector, Func<TAccumulate, TSource, TAccumulate> func)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TKey : notnull
            => new(new(source.Enumerator, keySelector, seedSelector, func, null));

        public static ValueEnumerable<AggregateBy2<TEnumerator, TSource, TKey, TAccumulate>, KeyValuePair<TKey, TAccumulate>> AggregateBy<TEnumerator, TSource, TKey, TAccumulate>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TAccumulate> seedSelector, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey>? keyComparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TKey : notnull
            => new(new(source.Enumerator, keySelector, seedSelector, func, keyComparer));
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
    struct AggregateBy<TEnumerator, TSource, TKey, TAccumulate>(TEnumerator source, Func<TSource, TKey> keySelector, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey>? keyComparer)
        : IValueEnumerator<KeyValuePair<TKey, TAccumulate>>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TKey : notnull
    {
        TEnumerator source = source;

        Dictionary<TKey, TAccumulate>? dictionary;
        Dictionary<TKey, TAccumulate>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<KeyValuePair<TKey, TAccumulate>> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<KeyValuePair<TKey, TAccumulate>> destination)
        {
            destination = default;
            return false;
        }

        public bool TryGetNext(out KeyValuePair<TKey, TAccumulate> current)
        {
            if (dictionary == null)
            {
                // if empty, don't create dictionary.
                if (source.TryGetNonEnumeratedCount(out var count) && count == 0)
                {
                    Unsafe.SkipInit(out current);
                    return false;
                }

                Initialize();
            }

            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        void Initialize()
        {
            var dict = keyComparer != null
                ? new Dictionary<TKey, TAccumulate>(keyComparer)
                : new Dictionary<TKey, TAccumulate>();

            using (source)
            {
                if (source.TryGetSpan(out var span))
                {
                    foreach (var item in span)
                    {
                        var key = keySelector(item);
#if NETSTANDARD2_1
                        if (dict.TryGetValue(key, out var accumulate))
                        {
                            dict[key] = func(accumulate, item);
                        }
                        else
                        {
                            dict[key] = func(seed, item);
                        }
#else
                        ref var accumulate = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists);
                        accumulate = func(exists ? accumulate! : seed, item);
#endif
                    }
                }
                else
                {
                    while (source.TryGetNext(out var item))
                    {
                        var key = keySelector(item);
#if NETSTANDARD2_1
                        if (dict.TryGetValue(key, out var accumulate))
                        {
                            dict[key] = func(accumulate, item);
                        }
                        else
                        {
                            dict[key] = func(seed, item);
                        }
#else
                        ref var accumulate = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists);
                        accumulate = func(exists ? accumulate! : seed, item);
#endif
                    }
                }
            }

            this.dictionary = dict;
            this.enumerator = dict.GetEnumerator();
        }

        public void Dispose()
        {
            if (dictionary != null)
            {
                enumerator.Dispose();
            }
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
    struct AggregateBy2<TEnumerator, TSource, TKey, TAccumulate>(TEnumerator source, Func<TSource, TKey> keySelector, Func<TKey, TAccumulate> seedSelector, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey>? keyComparer)
        : IValueEnumerator<KeyValuePair<TKey, TAccumulate>>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TKey : notnull
    {
        TEnumerator source = source;

        Dictionary<TKey, TAccumulate>? dictionary;
        Dictionary<TKey, TAccumulate>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<KeyValuePair<TKey, TAccumulate>> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<KeyValuePair<TKey, TAccumulate>> destination)
        {
            destination = default;
            return false;
        }

        public bool TryGetNext(out KeyValuePair<TKey, TAccumulate> current)
        {
            if (dictionary == null)
            {
                // if empty, don't create dictionary.
                if (source.TryGetNonEnumeratedCount(out var count) && count == 0)
                {
                    Unsafe.SkipInit(out current);
                    return false;
                }

                Initialize();
            }

            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        void Initialize()
        {
            var dict = keyComparer != null
                ? new Dictionary<TKey, TAccumulate>(keyComparer)
                : new Dictionary<TKey, TAccumulate>();

            using (source)
            {
                if (source.TryGetSpan(out var span))
                {
                    foreach (var item in span)
                    {
                        var key = keySelector(item);
#if NETSTANDARD2_1
                        if (dict.TryGetValue(key, out var accumulate))
                        {
                            dict[key] = func(accumulate, item);
                        }
                        else
                        {
                            dict[key] = func(seedSelector(key), item);
                        }
#else
                        ref var accumulate = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists);
                        accumulate = func(exists ? accumulate! : seedSelector(key), item);
#endif
                    }
                }
                else
                {
                    while (source.TryGetNext(out var item))
                    {
                        var key = keySelector(item);
#if NETSTANDARD2_1
                        if (dict.TryGetValue(key, out var accumulate))
                        {
                            dict[key] = func(accumulate, item);
                        }
                        else
                        {
                            dict[key] = func(seedSelector(key), item);
                        }
#else
                        ref var accumulate = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists);
                        accumulate = func(exists ? accumulate! : seedSelector(key), item);
#endif
                    }
                }
            }

            this.dictionary = dict;
            this.enumerator = dict.GetEnumerator();
        }

        public void Dispose()
        {
            if (dictionary != null)
            {
                enumerator.Dispose();
            }
            source.Dispose();
        }
    }
}
