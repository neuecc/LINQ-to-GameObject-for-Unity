using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;
using Cathei.LinqGen;
using SpanLinq;
using StructLinq;
using ZLinq;
using ZLinq.Linq;
using Benchmark;

#if !DEBUG

//BenchmarkRunner.Run<IterateBenchmark>(DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)), args);
//BenchmarkRunner.Run<SimdRange>(DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)), args);
//BenchmarkRunner.Run<SimdSelect>(DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)), args);
//BenchmarkRunner.Run<LookupBattle>(DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)), args);
// BenchmarkRunner.Run<CopyBattle>(DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)), args);

// BenchmarkRunner.Run<SimdSum>(DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default), args);
//BenchmarkRunner.Run<SimdSumUnsigned>(DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default), args);

//BenchmarkRunner.Run<SimdAny>(DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default), args);


//BenchmarkRunner.Run<ReadMeBenchmark>(DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default), args);

//BenchmarkRunner.Run<LinqPerfBenchmarks.AggregateBy00>(DefaultConfig.Instance, args);
//BenchmarkRunner.Run<LinqPerfBenchmarks.Count00>(DefaultConfig.Instance, args);
//BenchmarkRunner.Run<LinqPerfBenchmarks.CountBy00>(DefaultConfig.Instance, args);
//BenchmarkRunner.Run<LinqPerfBenchmarks.GroupBy00>(DefaultConfig.Instance, args);
//BenchmarkRunner.Run<LinqPerfBenchmarks.Order00>(DefaultConfig.Instance, args);
BenchmarkRunner.Run<LinqPerfBenchmarks.Where00>(DefaultConfig.Instance, args);
//BenchmarkRunner.Run<LinqPerfBenchmarks.Where01>(DefaultConfig.Instance, args);
#else

BenchmarkRunner.Run<IterateBenchmark>(DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)), args);

var i = 0;
foreach (var item in typeof(Enumerable).GetMethods().GroupBy(x => x.Name))
{
    Console.WriteLine($"- [ ] {item.Key}");
    i++;
}
Console.WriteLine(i);


#endif

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
