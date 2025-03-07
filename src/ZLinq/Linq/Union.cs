namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Union<TEnumerable, TSource> Union<TEnumerable, TSource>(this TEnumerable source, IEnumerable<TSource> second)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, second, null);

        public static Union<TEnumerable, TSource> Union<TEnumerable, TSource>(this TEnumerable source, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, second, comparer);

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
   struct Union<TEnumerable, TSource>(TEnumerable source, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
       : IValueEnumerable<TSource>
       where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        HashSet<TSource>? set;
        IEnumerator<TSource>? secondEnumerator;
        byte state = 0;

        public ValueEnumerator<Union<TEnumerable, TSource>, TSource> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> dest) => false;

        public bool TryGetNext(out TSource current)
        {
            if (state == 0)
            {
                set = new HashSet<TSource>(comparer ?? EqualityComparer<TSource>.Default);
                state = 1;
            }

            if (state == 1)
            {
                while (source.TryGetNext(out var value) && set!.Add(value))
                {
                    current = value;
                    return true;
                }
                state = 2;
            }

            if (state == 2)
            {
                if (secondEnumerator is null)
                {
                    secondEnumerator = second.GetEnumerator();
                }

                while (secondEnumerator.MoveNext())
                {
                    if (set!.Add(secondEnumerator.Current))
                    {
                        current = secondEnumerator.Current;
                        return true;
                    }
                }

                state = 3;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            state = 3;
            secondEnumerator?.Dispose();
            source.Dispose();
        }
    }
}
