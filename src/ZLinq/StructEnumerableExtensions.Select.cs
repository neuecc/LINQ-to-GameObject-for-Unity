namespace ZLinq;

partial class StructEnumerableExtensions
{
    public static SelectStructEnumerable<T, TEnumerable, TEnumerator, TResult> Select<T, TEnumerable, TEnumerator, TResult>(
        this TEnumerable source, Func<T, TResult> selector)
        where TEnumerable : struct, IStructEnumerable<T, TEnumerator>
        where TEnumerator : struct, IStructEnumerator<T>
    {
        return new(source, selector);
    }
}

public readonly struct SelectStructEnumerable<T, TEnumerable, TEnumerator, TResult>(TEnumerable source, Func<T, TResult> selector)
    : IStructEnumerable<TResult, SelectStructEnumerable<T, TEnumerable, TEnumerator, TResult>.Enumerator>
    where TEnumerable : struct, IStructEnumerable<T, TEnumerator>
    where TEnumerator : struct, IStructEnumerator<T>
{
    public bool IsNull => source.IsNull;

    public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

    public Enumerator GetEnumerator() => new(source, selector);

    public struct Enumerator(TEnumerable source, Func<T, TResult> selector) : IStructEnumerator<TResult>
    {
        TEnumerator enumerator;
        TResult current = default!;

        public bool IsNull => enumerator.IsNull;
        public TResult Current => current;

        public bool MoveNext()
        {
            if (enumerator.IsNull)
            {
                this.enumerator = source.GetEnumerator();
            }

            if (enumerator.MoveNext())
            {
                current = selector(enumerator.Current);
                return true;
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
