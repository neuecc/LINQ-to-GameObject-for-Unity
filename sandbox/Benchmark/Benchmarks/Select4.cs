using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ZLinq;

namespace Benchmark;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class Select4
{
    int[] src = Enumerable.Range(1, 1000000).ToArray();

    [Benchmark(Baseline = true)]
    [BenchmarkCategory(Categories.LINQ)]
    public void LinqSelect1()
    {
        foreach (var _ in src.Select(x => x + x))
        {

        }
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public void LinqSelect2()
    {
        foreach (var _ in src.Select(x => x + x).Select(x => x + x))
        {

        }
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public void LinqSelect3()
    {
        foreach (var _ in src.Select(x => x + x).Select(x => x + x).Select(x => x + x))
        {

        }
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public void LinqSelect4()
    {
        foreach (var _ in src.Select(x => x + x).Select(x => x + x).Select(x => x + x).Select(x => x + x))
        {

        }
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory(Categories.ZLinq)]
    public void ZLinqSelect1()
    {
        foreach (var _ in src.AsValueEnumerable().Select(x => x + x))
        {

        }
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public void ZLinqSelect2()
    {
        foreach (var _ in src.AsValueEnumerable().Select(x => x + x).Select(x => x + x))
        {

        }
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public void ZLinqSelect3()
    {
        foreach (var _ in src.AsValueEnumerable().Select(x => x + x).Select(x => x + x).Select(x => x + x))
        {

        }
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public void ZLinqSelect4()
    {
        foreach (var _ in src.AsValueEnumerable().Select(x => x + x).Select(x => x + x).Select(x => x + x).Select(x => x + x))
        {

        }
    }
}
