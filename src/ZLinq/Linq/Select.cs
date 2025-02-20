namespace ZLinq
{
    partial class StructEnumerableExtensions
    {
        public static SelectStructEnumerable<TEnumerable, T, TResult> Select<TEnumerable, T, TResult>(this TEnumerable source, Func<T, TResult> selector)
            where TEnumerable : struct, IStructEnumerable<T>
        {
            return new(source, selector);
        }

        public static StructEnumerator<SelectStructEnumerable<TEnumerable, T, TResult>, TResult> GetEnumerator<TEnumerable, T, TResult>(
            this SelectStructEnumerable<TEnumerable, T, TResult> source)
            where TEnumerable : struct, IStructEnumerable<T>
        {
            return new(source);
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct SelectStructEnumerable<TEnumerable, T, TResult>(TEnumerable source, Func<T, TResult> selector) : IStructEnumerable<TResult>
        where TEnumerable : struct, IStructEnumerable<T>
    {
        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetNext(out TResult current)
        {
            if (source.TryGetNext(out var value))
            {
                current = selector(value);
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}