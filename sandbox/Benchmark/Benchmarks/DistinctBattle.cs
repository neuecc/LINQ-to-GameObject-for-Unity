using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Cathei.LinqGen;
using SpanLinq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ZLinq;
using ZLinq.Linq;

namespace Benchmark
{
    [ShortRunJob]
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class DistinctBattle
    {
        int[] array;

        public DistinctBattle()
        {
            var rand = new Random(42);
            array = Enumerable.Range(1, 10000)
                .Select(x => rand.Next(1000))
                .ToArray();
        }

        [Benchmark]
        public void SystemLinq()
        {
            var seq = array.Distinct();
            foreach (var item in seq)
            {
            }
        }

        [Benchmark]
        public void ZLinq()
        {
            var seq = array.AsValueEnumerable().Distinct();
            foreach (var item in seq)
            {
            }
        }

        [Benchmark]
        public void LegacyZLinq()
        {
            var seq = array.AsValueEnumerable().LegacyDistinct();
            foreach (var item in seq)
            {
            }
        }
    }
}

namespace ZLinq.Linq
{
    internal static class LegacyExtensions
    {
        public static ValueEnumerable<LegacyDistinct<TEnumerator, TSource>, TSource> LegacyDistinct<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, null!));
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct LegacyDistinct<TEnumerator, TSource>(TEnumerator source, IEqualityComparer<TSource>? comparer)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        HashSet<TSource>? set;

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

        public bool TryCopyTo(Span<TSource> destination, Index offset) => false;

        public bool TryGetNext(out TSource current)
        {
            if (set == null)
            {
                set = new HashSet<TSource>(comparer ?? EqualityComparer<TSource>.Default);
            }

            while (source.TryGetNext(out var value))
            {
                if (set.Add(value))
                {
                    current = value;
                    return true;
                }
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
