using ZLinq.Linq;

namespace ZLinq
{
    partial class StructEnumerableExtensions
    {
        public static SelectStructEnumerable<TEnumerable, T, TResult> Select<TEnumerable, T, TResult>(
            this TEnumerable source, Func<T, TResult> selector)
            where TEnumerable : struct, IStructEnumerable<TEnumerable, T>
        {
            return new(source, selector);
        }
    }
}

namespace ZLinq.Linq
{
    public struct SelectStructEnumerable<TEnumerable, T, TResult>(TEnumerable source, Func<T, TResult> selector)
        : IStructEnumerable<SelectStructEnumerable<TEnumerable, T, TResult>, TResult>
        where TEnumerable : struct, IStructEnumerable<TEnumerable, T>
    {
        public bool IsNull => source.IsNull;
        public StructEnumerator<SelectStructEnumerable<TEnumerable, T, TResult>, TResult> GetEnumerator() => new(this);
        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetNext(out TResult value)
        {
            if (source.TryGetNext(out var v))
            {
                value = selector(v);
                return true;
            }

            value = default!;
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}