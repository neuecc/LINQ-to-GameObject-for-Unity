using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Cathei.LinqGen;
using SpanLinq;
using StructLinq;
using ZLinq;

namespace Benchmark;

[ShortRunJob]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class IterateBenchmark
{
    int[] array = Enumerable.Range(1, 10000).ToArray();

    public IterateBenchmark()
    {

    }

    [Benchmark]
    public void SystemLinq()
    {
        var seq = array
            .Select(x => x * 3)
            .Where(x => x % 2 == 0);

        foreach (var item in seq)
        {
        }
    }

    [Benchmark]
    public void ZLinq()
    {
        var seq = array.AsValueEnumerable()
            .Select(x => x * 3)
            .Where(x => x % 2 == 0);

        foreach (var item in seq) { }
    }

    //[Benchmark]
    //public void ZLinqSpan()
    //{
    //    var seq = array.AsSpan().AsValueEnumerable()
    //        .Select<SpanValueEnumerable<int>, int, int>(x => x * 3)
    //        .Where<SelectValueEnumerable<SpanValueEnumerable<int>, int, int>, int>(x => x % 2 == 0);

    //    foreach (var item in seq)
    //    {

    //    }
    //}

    [Benchmark]
    public void LinqGen()
    {
        var seq = array.Gen()
            .Select(x => x * 3)
            .Where(x => x % 2 == 0);

        foreach (var item in seq)
        {

        }
    }

    [Benchmark]
    public void LinqAf()
    {
        var seq = LinqAF.ArrayExtensionMethods
            .Select(array, x => x * 3)
            .Where(x => x % 2 == 0);

        foreach (var item in seq)
        {

        }
    }

    //[Benchmark]
    //public void StructLinq()
    //{
    //    var seq = array.ToValueEnumerable()
    //        .Select(x => x * 3, x => x)
    //        .Where(x => x % 2 == 0, x => x);

    //    foreach (var item in seq)
    //    {

    //    }
    //}

    [Benchmark]
    public void SpanLinq()
    {
        var seq = array.AsSpan()
            .Select(x => x * 3)
            .Where(x => x % 2 == 0);

        foreach (var item in seq)
        {

        }
    }
}
