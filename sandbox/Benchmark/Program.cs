using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;
using ZLinq;
using ZLinq.Linq;
using StructLinq;
using Cathei.LinqGen;
using SpanLinq;

BenchmarkRunner.Run<IterateBenchmark>(DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)), args);

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
        var seq = array.AsStructEnumerable()
            .Select<ArrayStructEnumerable<int>, int, int>(x => x * 3)
            .Where<SelectStructEnumerable<ArrayStructEnumerable<int>, int, int>, int>(x => x % 2 == 0);

        foreach (var item in seq)
        {

        }
    }

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

    [Benchmark]
    public void StructLinq()
    {
        var seq = array.ToStructEnumerable()
            .Select(x => x * 3, x => x)
            .Where(x => x % 2 == 0, x => x);

        foreach (var item in seq)
        {

        }
    }

    [Benchmark]
    public void SpanLinq()
    {
        //string[] array =
        var seq = array.AsSpan()
            .Select(x => x * 3)
            .Where(x => x % 2 == 0);


        seq.GetEnumerator();

        foreach (var item in seq)
        {

        }
    }
}