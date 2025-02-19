using ZLinq.Linq;

namespace ZLinq
{
    partial class StructEnumerableExtensions
    {
        public static WhereStructEnumerable<T, TEnumerable, TEnumerator> Where<T, TEnumerable, TEnumerator>(
            this TEnumerable source, Func<T, bool> predicate)
            where TEnumerable : struct, IStructEnumerable<T, TEnumerator>
            where TEnumerator : struct, IStructEnumerator<T>
        {
            return new(source, predicate);
        }
    }
}

namespace ZLinq.Linq
{
    public readonly struct WhereStructEnumerable<T, TEnumerable, TEnumerator>(TEnumerable source, Func<T, bool> predicate)
        : IStructEnumerable<T, WhereStructEnumerable<T, TEnumerable, TEnumerator>.Enumerator>
        where TEnumerable : struct, IStructEnumerable<T, TEnumerator>
        where TEnumerator : struct, IStructEnumerator<T>
    {
        public bool IsNull => source.IsNull;

        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public Enumerator GetEnumerator() => new(source, predicate);

        public struct Enumerator(TEnumerable source, Func<T, bool> predicate) : IStructEnumerator<T>
        {
            TEnumerator enumerator;
            T current = default!;

            public bool IsNull => enumerator.IsNull;
            public T Current => current;

            public bool MoveNext()
            {
                if (enumerator.IsNull)
                {
                    this.enumerator = source.GetEnumerator();
                }

                while (enumerator.MoveNext())
                {
                    var value = enumerator.Current;
                    if (predicate(value))
                    {
                        current = value;
                        return true;
                    }
                }
                return false;
            }

            public void Dispose()
            {
                if (!enumerator.IsNull)
                {
                    enumerator.Dispose();
                }
            }
        }
    }
}