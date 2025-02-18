using System;
using System.Collections.Generic;
using System.Text;

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

public readonly struct SelectStructEnumerable<T, TEnumerable, TEnumerator, TResult>
    : IStructEnumerable<TResult, SelectStructEnumerable<T, TEnumerable, TEnumerator, TResult>.Enumerator>
    where TEnumerable : struct, IStructEnumerable<T, TEnumerator>
    where TEnumerator : struct, IStructEnumerator<T>
{
    readonly TEnumerable source;
    readonly Func<T, TResult> selector;

    public SelectStructEnumerable(TEnumerable source, Func<T, TResult> selector)
    {
        this.source = source;
        this.selector = selector;
    }

    public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

    public Enumerator GetEnumerator() => new();

    public struct Enumerator : IStructEnumerator<TResult>
    {
        readonly TEnumerable source;
        readonly Func<T, TResult> selector;

        bool isInit;
        TEnumerator enumerator;

        public TResult Current { get; private set; }

        public Enumerator(TEnumerable source, Func<T, TResult> selector) : this()
        {
            this.source = source;
            this.selector = selector;
            this.isInit = false;
            this.Current = default!;
            this.enumerator = default!;
        }

        public bool MoveNext()
        {
            if (!isInit)
            {
                isInit = true;
                this.enumerator = source.GetEnumerator();
            }

            if (enumerator.MoveNext())
            {
                Current = selector(enumerator.Current);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            if (isInit)
            {
                enumerator.Dispose();
            }
        }
    }
}
