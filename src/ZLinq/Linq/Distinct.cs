namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Distinct<TEnumerable, TSource> Distinct<TEnumerable, TSource>(this TEnumerable source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, null!);

        public static Distinct<TEnumerable, TSource> Distinct<TEnumerable, TSource>(this TEnumerable source, IEqualityComparer<TSource> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, comparer);

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
    struct Distinct<TEnumerable, TSource>(TEnumerable source, IEqualityComparer<TSource>? comparer)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        IEqualityComparer<TSource> comparer = comparer ?? EqualityComparer<TSource>.Default;
        HashSet<TSource>? set;

        public ValueEnumerator<Distinct<TEnumerable, TSource>, TSource> GetEnumerator() => new(this);

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

        public bool TryGetNext(out TSource current)
        {
            if (set == null)
            {
                set = new HashSet<TSource>(comparer);
            }

            if (source.TryGetNext(out var value) && set.Add(value))
            {
                current = value;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }

        // Optimize

        public TSource[] ToArray()
        {
            var set = new HashSet<TSource>(comparer);
            while (source.TryGetNext(out var value) && set.Add(value))
            {
            }
            return set.ToArray();
        }

        public List<TSource> ToList()
        {
            return ListMarshal.AsList(ToArray());
        }

        public int CopyTo(TSource[] dest)
        {
            var set = new HashSet<TSource>(comparer);
            while (source.TryGetNext(out var value) && set.Add(value))
            {
            }
            set.CopyTo(dest);
            return set.Count;
        }
    }

}
