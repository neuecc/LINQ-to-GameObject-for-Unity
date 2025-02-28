namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Boolean SequenceEqual<TEnumerable, TSource>(this TEnumerable source, IEnumerable<TSource> second)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return SequenceEqual(source, second, null!);
        }

        public static Boolean SequenceEqual<TEnumerable, TSource>(this TEnumerable source, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) // comaprer is nullable
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
#if NET8_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool EnumerableTryGetSpan(IEnumerable<TSource> source, out ReadOnlySpan<TSource> span)
            {
                bool result = true;
                if (source.GetType() == typeof(TSource[]))
                {
                    span = Unsafe.As<TSource[]>(source);
                }
                else if (source.GetType() == typeof(List<TSource>))
                {
                    span = CollectionsMarshal.AsSpan(Unsafe.As<List<TSource>>(source));
                }
                else
                {
                    span = default;
                    result = false;
                }

                return result;
            }

            if (source.TryGetSpan(out var sourceSpan) && EnumerableTryGetSpan(second, out var secondSpan))
            {
                return sourceSpan.SequenceEqual(secondSpan, comparer);
            }
#endif

            if (source.TryGetNonEnumeratedCount(out var count1) && second.TryGetNonEnumeratedCount(out var count2) && count1 != count2)
            {
                return false;
            }

            comparer ??= EqualityComparer<TSource>.Default;
            using var e1 = source.GetEnumerator<TEnumerable, TSource>();
            using var e2 = second.GetEnumerator();
            while (e1.MoveNext())
            {
                if (!(e2.MoveNext() && comparer.Equals(e1.Current, e2.Current)))
                {
                    return false;
                }
            }
            return !e2.MoveNext();
        }
    }
}
