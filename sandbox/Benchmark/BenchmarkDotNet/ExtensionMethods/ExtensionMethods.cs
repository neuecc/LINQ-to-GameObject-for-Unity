using BenchmarkDotNet.Engines;
using ZLinq;

namespace Benchmark;

internal static class ExtensionMethods
{
    /// <summary>
    /// Executes and consumes given <see cref="ValueEnumerable{TEnumerator, T}"/>.
    /// </summary>
    public static void Consume<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> enumerable, Consumer consumer)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
        foreach (T item in enumerable)
        {
            consumer.Consume(in item);
        }
    }
}
