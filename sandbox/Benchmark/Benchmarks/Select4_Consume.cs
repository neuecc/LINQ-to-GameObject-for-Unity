using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using ZLinq;

namespace Benchmark;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class Select4_Consume
{
    readonly Consumer consumer = new Consumer();
    int[] src = Enumerable.Range(1, 1000000).ToArray();

    [Benchmark(Baseline = true)]
    [BenchmarkCategory(Categories.LINQ)]
    public void LinqSelect1()
    {
        src.Select(x => x + x).Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public void LinqSelect2()
    {
        src.Select(x => x + x)
           .Select(x => x + x)
           .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public void LinqSelect3()
    {
        src.Select(x => x + x)
           .Select(x => x + x)
           .Select(x => x + x)
           .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public void LinqSelect4()
    {
        src.Select(x => x + x)
           .Select(x => x + x)
           .Select(x => x + x)
           .Select(x => x + x)
           .Consume(consumer);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory(Categories.ZLinq)]
    public void ZLinqSelect1()
    {
        src.AsValueEnumerable()
           .Select(x => x + x)
           .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public void ZLinqSelect2()
    {
        src.AsValueEnumerable()
           .Select(x => x + x)
           .Select(x => x + x)
           .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public void ZLinqSelect3()
    {
        src.AsValueEnumerable()
           .Select(x => x + x)
           .Select(x => x + x)
           .Select(x => x + x)
           .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public void ZLinqSelect4()
    {
        src.AsValueEnumerable()
           .Select(x => x + x)
           .Select(x => x + x)
           .Select(x => x + x)
           .Select(x => x + x)
           .Consume(consumer);
    }
}
